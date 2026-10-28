using HotelService.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelService.Data;

public class HotelDbContext : DbContext
{
    public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options)
    {
    }

    public DbSet<RoomType> RoomTypes => Set<RoomType>();
    public DbSet<Room> Rooms => Set<Room>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<RoomType>(entity =>
        {
            entity.ToTable("RoomTypes");

            entity.HasKey(e => e.RoomTypeId);

            entity.Property(e => e.RoomTypeId)
                  .ValueGeneratedOnAdd();

            entity.Property(e => e.TypeName)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.HasIndex(e => e.TypeName)
                  .IsUnique();

            entity.Property(e => e.Capacity)
                  .IsRequired();

            entity.Property(e => e.Description)
                  .HasMaxLength(500);

            entity.HasMany(e => e.Rooms)
                  .WithOne(r => r.RoomType)
                  .HasForeignKey(r => r.RoomTypeId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.ToTable("Rooms");

            entity.HasKey(e => e.RoomId);

            entity.Property(e => e.RoomId)
                  .ValueGeneratedOnAdd();

            entity.Property(e => e.RoomNumber)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.HasIndex(e => e.RoomNumber)
                  .IsUnique();

            entity.Property(e => e.PricePerNight)
                  .HasColumnType("decimal(10,2)")
                  .IsRequired();

            entity.Property(e => e.Status)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(e => e.Description)
                  .HasMaxLength(500);
        });
    }
}
