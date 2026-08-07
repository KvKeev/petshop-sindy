using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SindyPetshop.Infrastructure.Data;
using SindyPetshop.Domain.Interfaces;
using SindyPetshop.Infrastructure.Repositories;
using SindyPetshop.Application.Services;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SindyPetshop.Infrastructure.Services;
using SindyPetshop.Api.BackgroundServices;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// Registra el DbContext, indicándole que use SQLite con la connection string de appsettings.json
builder.Services.AddDbContext<SindyPetshopDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
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

var jwtSecret = builder.Configuration["Jwt:Secret"]!;
builder.Services.AddAuthentication(options =>
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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };
});

builder.Services.AddControllers();
const string PoliticaLoginRateLimit = "LoginPolicy";

builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy(PoliticaLoginRateLimit, httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsJsonAsync(
            new { mensaje = "Demasiados intentos de login. Esperá un minuto y probá de nuevo." },
            cancellationToken);
    };
});

var app = builder.Build();

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();