using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DotNetEnv;

// Load .env at the start of your Program.cs

public class ViaTabloidDbContextFactory : IDesignTimeDbContextFactory<ViaTabloidDbContext>
{
    public ViaTabloidDbContext CreateDbContext(string[] args)
    {
        Directory.SetCurrentDirectory(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory)));
        Env.Load();

        var optionsBuilder = new DbContextOptionsBuilder<ViaTabloidDbContext>();

        // Use environment variables for local dev / migration tooling
        var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
        var port = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
        var user = Environment.GetEnvironmentVariable("DB_USER") ?? "postgres";
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "ViaViaVia";
        var database = Environment.GetEnvironmentVariable("DB_NAME") ?? "VIATAB";

        var connectionString = $"Host={host};Port={port};Username={user};Password={password};Database={database}";

        optionsBuilder.UseNpgsql(connectionString);

        return new ViaTabloidDbContext(optionsBuilder.Options);
    }
}