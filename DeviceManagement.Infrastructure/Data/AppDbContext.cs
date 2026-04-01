using DeviceManagement.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeviceManagement.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Device> Devices => Set<Device>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Name)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(u => u.Email)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.HasIndex(u => u.Email)
                  .IsUnique();

            entity.Property(u => u.PasswordHash)
                  .IsRequired();

            entity.Property(u => u.Role)
                  .IsRequired()
                  .HasMaxLength(20)
                  .HasDefaultValue("User");

            entity.Property(u => u.Location)
                  .HasMaxLength(200);
        });

        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(d => d.Id);

            entity.Property(d => d.Name)
                  .IsRequired()
                  .HasMaxLength(150);

            entity.Property(d => d.Manufacturer)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(d => d.Type)
                  .IsRequired()
                  .HasMaxLength(20);

            entity.Property(d => d.OperatingSystem)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(d => d.OsVersion)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(d => d.Processor)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(d => d.RamAmount)
                  .IsRequired();

            entity.Property(d => d.Description)
                  .HasMaxLength(1000);

            entity.HasOne(d => d.AssignedUser)
                  .WithMany(u => u.AssignedDevices)
                  .HasForeignKey(d => d.AssignedUserId)
                  .OnDelete(DeleteBehavior.SetNull)
                  .IsRequired(false);
        });
    }
}