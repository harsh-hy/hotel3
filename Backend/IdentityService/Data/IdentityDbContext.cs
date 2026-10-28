using IdentityService.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Data;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(e => e.UserId);

            entity.Property(e => e.UserId)
                  .ValueGeneratedOnAdd();

            entity.Property(e => e.Username)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.HasIndex(e => e.Username)
                  .IsUnique();

            entity.Property(e => e.PasswordHash)
                  .IsRequired()
                  .HasMaxLength(255);

            entity.Property(e => e.FullName)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(e => e.Role)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(e => e.IsActive)
                  .IsRequired()
                  .HasDefaultValue(true);

            entity.Property(e => e.CreatedAt)
                  .IsRequired();
        });
    }
}
