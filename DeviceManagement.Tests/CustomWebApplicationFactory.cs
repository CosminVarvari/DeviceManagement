using BCrypt.Net;
using DeviceManagement.Infrastructure.Data;
using DeviceManagement.Infrastructure.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DeviceManagement.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = $"TestDb_{Guid.NewGuid()}";
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();

            if (!db.Users.Any())
                SeedTestData(db);
        });
    }

    private static void SeedTestData(AppDbContext db)
    {
        var user = new User
        {
            Id = TestConstants.UserId,
            Name = "Test User",
            Email = "test@company.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            Role = "User",
            Location = "Cluj-Napoca"
        };

        var adminUser = new User
        {
            Id = TestConstants.AdminId,
            Name = "Admin User",
            Email = "admin@company.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"),
            Role = "Admin",
            Location = "Cluj-Napoca"
        };

        var device = new Device
        {
            Id = TestConstants.DeviceId,
            Name = "iPhone 15 Pro",
            Manufacturer = "Apple",
            Type = "Phone",
            OperatingSystem = "iOS",
            OsVersion = "17.4",
            Processor = "A17 Pro",
            RamAmount = 8,
            Description = "Test device",
            AssignedUserId = null
        };

        var assignedDevice = new Device
        {
            Id = TestConstants.AssignedDeviceId,
            Name = "Galaxy S24",
            Manufacturer = "Samsung",
            Type = "Phone",
            OperatingSystem = "Android",
            OsVersion = "14.0",
            Processor = "Snapdragon 8 Gen 3",
            RamAmount = 12,
            Description = "Already assigned device",
            AssignedUserId = TestConstants.UserId
        };

        db.Users.AddRange(user, adminUser);
        db.Devices.AddRange(device, assignedDevice);
        db.SaveChanges();
    }
}