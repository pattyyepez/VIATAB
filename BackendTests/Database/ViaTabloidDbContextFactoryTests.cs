using System;
using Database;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;

public class ViaTabloidDbContextFactoryTests
{
    [Fact]
    public void CreateDbContext_UsesEnvironmentVariables()
    {
        // Arrange: set environment variables
        Environment.SetEnvironmentVariable("DB_HOST", "testhost");
        Environment.SetEnvironmentVariable("DB_PORT", "1234");
        Environment.SetEnvironmentVariable("DB_USER", "testuser");
        Environment.SetEnvironmentVariable("DB_PASSWORD", "testpass");
        Environment.SetEnvironmentVariable("DB_NAME", "testdb");

        var factory = new ViaTabloidDbContextFactory();

        // Act
        var context = factory.CreateDbContext(Array.Empty<string>());

        // Assert
        context.Should().NotBeNull();
        context.Database.GetDbConnection().ConnectionString.Should().Contain("Host=testhost");
        context.Database.GetDbConnection().ConnectionString.Should().Contain("Port=1234");
        context.Database.GetDbConnection().ConnectionString.Should().Contain("Username=testuser");
        context.Database.GetDbConnection().ConnectionString.Should().Contain("Password=testpass");
        context.Database.GetDbConnection().ConnectionString.Should().Contain("Database=testdb");
    }

    [Fact]
    public void CreateDbContext_UsesDefaults_WhenEnvironmentVariablesMissing()
    {
        // Arrange: clear environment variables
        Environment.SetEnvironmentVariable("DB_HOST", null);
        Environment.SetEnvironmentVariable("DB_PORT", null);
        Environment.SetEnvironmentVariable("DB_USER", null);
        Environment.SetEnvironmentVariable("DB_PASSWORD", null);
        Environment.SetEnvironmentVariable("DB_NAME", null);

        var factory = new ViaTabloidDbContextFactory();

        // Act
        var context = factory.CreateDbContext(Array.Empty<string>());

        // Assert
        var connectionString = context.Database.GetDbConnection().ConnectionString;

        connectionString.Should().Contain("Host=localhost");
        connectionString.Should().Contain("Port=5432");
        connectionString.Should().Contain("Username=postgres");
        connectionString.Should().Contain("Password=calculadora11");
        connectionString.Should().Contain("Database=VIATAB");
    }
}
