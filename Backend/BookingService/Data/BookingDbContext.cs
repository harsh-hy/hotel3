using BookingService.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Data;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options)
    {
    }

    public DbSet<Guest> Guests => Set<Guest>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Guest>(entity =>
        {
            entity.ToTable("Guests");

            entity.HasKey(e => e.GuestId);

            entity.Property(e => e.GuestId)
                  .ValueGeneratedOnAdd();

            entity.Property(e => e.FullName)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(e => e.PhoneNumber)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(e => e.Email)
                  .HasMaxLength(100);

            entity.Property(e => e.Address)
                  .HasMaxLength(255);

            entity.Property(e => e.CreatedAt)
                  .IsRequired();

            entity.HasMany(e => e.Reservations)
                  .WithOne(r => r.Guest)
                  .HasForeignKey(r => r.GuestId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.ToTable("Reservations");

            entity.HasKey(e => e.ReservationId);

            entity.Property(e => e.ReservationId)
                  .ValueGeneratedOnAdd();

            entity.Property(e => e.GuestId)
                  .IsRequired();

            // RoomId is a logical reference only; NO SQL FK constraint
            entity.Property(e => e.RoomId)
                  .IsRequired();

            entity.Property(e => e.CheckInDate)
                  .IsRequired();

            entity.Property(e => e.CheckOutDate)
                  .IsRequired();

            entity.Property(e => e.NumberOfAdults)
                  .IsRequired();

            entity.Property(e => e.NumberOfChildren)
                  .IsRequired();

            entity.Property(e => e.PricePerNightSnapshot)
                  .HasColumnType("decimal(10,2)")
                  .IsRequired();

            entity.Property(e => e.TotalAmount)
                  .HasColumnType("decimal(10,2)")
                  .IsRequired();

            entity.Property(e => e.ReservationStatus)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(e => e.CreatedAt)
                  .IsRequired();

            entity.HasMany(e => e.Payments)
                  .WithOne(p => p.Reservation)
                  .HasForeignKey(p => p.ReservationId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("Payments");

            entity.HasKey(e => e.PaymentId);

            entity.Property(e => e.PaymentId)
                  .ValueGeneratedOnAdd();

            entity.Property(e => e.ReservationId)
                  .IsRequired();

            entity.Property(e => e.Amount)
                  .HasColumnType("decimal(10,2)")
                  .IsRequired();

            entity.Property(e => e.PaymentMethod)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(e => e.PaymentStatus)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(e => e.TransactionReference)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.HasIndex(e => e.TransactionReference)
                  .IsUnique();

            entity.Property(e => e.PaymentDate)
                  .IsRequired();
        });
    }
}
