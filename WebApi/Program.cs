using Database;
using Microsoft.EntityFrameworkCore;
using WebApi.Services;
using DotNetEnv;


Directory.SetCurrentDirectory(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory)));
Env.Load();

var builder = WebApplication.CreateBuilder(args);

var corsPolicyName = "AllowFrontend";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: corsPolicyName,
        policy =>
        {
            policy.WithOrigins("https://localhost:5001") // tu frontend Blazor
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IDepartmentService, EfcDepartmentService>();
builder.Services.AddTransient<IStoryService, EfcStoryService>();
builder.Services.AddDbContext<ViaTabloidDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        // Fallback: build from env vars
        var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
        var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
        var user = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "calculadora11";
        var database = Environment.GetEnvironmentVariable("DB_NAME") ?? "VIATAB";

        connectionString = $"Host={host};Port={port};Username={user};Password={password};Database={database}";
    }

    options.UseNpgsql(connectionString);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ViaTabloidDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseCors(corsPolicyName);

app.UseAuthorization();

app.MapControllers();

app.Run();