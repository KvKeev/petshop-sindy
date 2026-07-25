using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SindyPetshop.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// Registra el DbContext, indicándole que use SQLite con la connection string de appsettings.json
builder.Services.AddDbContext<SindyPetshopDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); // UI interactiva en /scalar/v1
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();