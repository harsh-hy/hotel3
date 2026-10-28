using BookingService.Data;
using BookingService.Entities;
using HotelManagement.Shared.Constants;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Repositories;

public class ReservationRepository : IReservationRepository
{
    private readonly BookingDbContext _context;

    public ReservationRepository(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<Reservation?> GetByIdAsync(int id, bool includeDetails = true)
    {
        var query = _context.Reservations.AsQueryable();

        if (includeDetails)
        {
            query = query
                .Include(r => r.Guest)
                .Include(r => r.Payments);
        }

        return await query.FirstOrDefaultAsync(r => r.ReservationId == id);
    }

    public async Task<IEnumerable<Reservation>> GetAllAsync(string? status = null)
    {
        var query = _context.Reservations
            .Include(r => r.Guest)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(r => r.ReservationStatus.ToLower() == status.ToLower());
        }

        return await query.OrderByDescending(r => r.CreatedAt).AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Reservation>> GetConflictingReservationsAsync(
        int roomId, DateOnly checkIn, DateOnly checkOut, int? excludeReservationId = null)
    {
        // Overlap: Existing.CheckInDate < Requested.CheckOutDate AND Existing.CheckOutDate > Requested.CheckInDate
        // Only non-cancelled reservations block availability
        var query = _context.Reservations
            .Where(r => r.RoomId == roomId)
            .Where(r => r.ReservationStatus != ReservationStatuses.Cancelled)
            .Where(r => r.CheckInDate < checkOut && r.CheckOutDate > checkIn);

        if (excludeReservationId.HasValue)
        {
            query = query.Where(r => r.ReservationId != excludeReservationId.Value);
        }

        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<List<int>> GetConflictingRoomIdsAsync(DateOnly checkIn, DateOnly checkOut)
    {
        return await _context.Reservations
            .Where(r => r.ReservationStatus != ReservationStatuses.Cancelled)
            .Where(r => r.CheckInDate < checkOut && r.CheckOutDate > checkIn)
            .Select(r => r.RoomId)
            .Distinct()
            .ToListAsync();
    }

    public async Task<bool> HasActiveReservationsForRoomAsync(int roomId)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        return await _context.Reservations
            .AnyAsync(r => r.RoomId == roomId 
                        && r.ReservationStatus != ReservationStatuses.Cancelled 
                        && r.ReservationStatus != ReservationStatuses.CheckedOut
                        && r.CheckOutDate >= today);
    }

    public async Task<IEnumerable<Reservation>> GetTodayReservationsAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        return await _context.Reservations
            .Include(r => r.Guest)
            .Where(r => r.ReservationStatus != ReservationStatuses.Cancelled)
            .Where(r => r.CheckInDate == today || r.CheckOutDate == today)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Reservation> AddAsync(Reservation reservation)
    {
        await _context.Reservations.AddAsync(reservation);
        await _context.SaveChangesAsync();
        return reservation;
    }

    public async Task UpdateAsync(Reservation reservation)
    {
        reservation.UpdatedAt = DateTime.UtcNow;
        _context.Reservations.Update(reservation);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Reservation>> GetReservationsForReportAsync(DateOnly? startDate, DateOnly? endDate)
    {
        var query = _context.Reservations.Include(r => r.Guest).AsQueryable();

        if (startDate.HasValue)
        {
            query = query.Where(r => r.CheckInDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(r => r.CheckOutDate <= endDate.Value);
        }

        return await query.AsNoTracking().ToListAsync();
    }
}
