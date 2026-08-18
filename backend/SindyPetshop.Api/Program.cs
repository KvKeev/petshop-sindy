using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using SindyPetshop.Api.BackgroundServices;
using SindyPetshop.Application.Services;
using SindyPetshop.Domain.Interfaces;
using SindyPetshop.Infrastructure.Data;
using SindyPetshop.Infrastructure.Repositories;
using SindyPetshop.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// Registra el DbContext, indicándole que use SQLite con la connection string de appsettings.json
builder.Services.AddDbContext<SindyPetshopDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<ProductoService>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IMascotaRepository, MascotaRepository>();
builder.Services.AddScoped<MascotaService>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();
builder.Services.AddScoped<PedidoService>();
builder.Services.AddHostedService<LiberacionReservasBackgroundService>();
builder.Services.AddScoped<AdminProductoService>();
builder.Services.AddScoped<AdminPedidoService>();
builder.Services.AddScoped<ClientePerfilService>();
builder.Services.AddSingleton<IMercadoPagoService>(_ =>
{
    var accessToken = builder.Configuration["MercadoPago:AccessToken"] ?? "";
    var webhookUrl = builder.Configuration["MercadoPago:WebhookUrl"]; // opcional
    return new MercadoPagoService(accessToken, webhookUrl);
});
builder.Services.AddSingleton<IFileStorageService>(_ =>
{
    var wwwRootPath =
        builder.Environment.WebRootPath
        ?? Path.Combine(builder.Environment.ContentRootPath, "wwwroot");
    return new FileStorageService(wwwRootPath);
});
builder.Services.AddScoped<AdminClienteService>();
builder.Services.AddSingleton<ICostoEnvioService>(_ =>
{
    var tarifaPlana = builder.Configuration.GetValue<decimal?>("Envio:TarifaPlana") ?? 0m;
    return new TarifaPlanaCostoEnvioService(tarifaPlana);
});
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IEmailService>(sp =>
{
    var apiKey = builder.Configuration["Resend:ApiKey"] ?? "";
    var fromEmail = builder.Configuration["Resend:FromEmail"] ?? "onboarding@resend.dev";
    var fromNombre = builder.Configuration["Resend:FromNombre"] ?? "Petshop Sindy";
    var frontendBaseUrl = builder.Configuration["Frontend:BaseUrl"] ?? "http://localhost:5173";
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var logger = sp.GetRequiredService<ILogger<ResendEmailService>>();
    return new ResendEmailService(httpClientFactory.CreateClient(), apiKey, fromEmail, fromNombre, frontendBaseUrl, logger);
});

var jwtSecret = builder.Configuration["Jwt:Secret"]!;
builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        };
    });

builder.Services.AddControllers();
const string PoliticaLoginRateLimit = "LoginPolicy";

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy(
        PoliticaLoginRateLimit,
        httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 5,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0,
                }
            )
    );

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsJsonAsync(
            new { mensaje = "Demasiados intentos de login. Esperá un minuto y probá de nuevo." },
            cancellationToken
        );
    };
});

var app = builder.Build();

// Crea la estructura de carpetas de wwwroot si no existe (uploads y avatares de la galería)
var wwwRootPathRuntime =
    app.Environment.WebRootPath ?? Path.Combine(app.Environment.ContentRootPath, "wwwroot");
Directory.CreateDirectory(Path.Combine(wwwRootPathRuntime, "avatares", "clientes"));
foreach (var tipo in new[] { "Perro", "Gato", "Ave", "Conejo", "Hamster", "Otro" })
    Directory.CreateDirectory(Path.Combine(wwwRootPathRuntime, "avatares", "mascotas", tipo));
Directory.CreateDirectory(Path.Combine(wwwRootPathRuntime, "uploads", "clientes"));
Directory.CreateDirectory(Path.Combine(wwwRootPathRuntime, "uploads", "mascotas"));

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<SindyPetshopDbContext>();
    SindyPetshop.Infrastructure.Data.Seed.DataSeeder.Seed(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // UI interactiva en /scalar/v1
}

app.UseRateLimiter();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
