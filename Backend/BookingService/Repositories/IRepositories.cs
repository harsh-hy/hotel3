using BookingService.Entities;

namespace BookingService.Repositories;

public interface IGuestRepository
{
    Task<Guest?> GetByIdAsync(int id);
    Task<IEnumerable<Guest>> GetAllAsync(string? search = null);
    Task<Guest> AddAsync(Guest guest);
    Task UpdateAsync(Guest guest);
}

public interface IReservationRepository
{
    Task<Reservation?> GetByIdAsync(int id, bool includeDetails = true);
    Task<IEnumerable<Reservation>> GetAllAsync(string? status = null);
    Task<IEnumerable<Reservation>> GetConflictingReservationsAsync(int roomId, DateOnly checkIn, DateOnly checkOut, int? excludeReservationId = null);
    Task<List<int>> GetConflictingRoomIdsAsync(DateOnly checkIn, DateOnly checkOut);
    Task<bool> HasActiveReservationsForRoomAsync(int roomId);
    Task<IEnumerable<Reservation>> GetTodayReservationsAsync();
    Task<Reservation> AddAsync(Reservation reservation);
    Task UpdateAsync(Reservation reservation);
    Task<IEnumerable<Reservation>> GetReservationsForReportAsync(DateOnly? startDate, DateOnly? endDate);
}

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(int id);
    Task<IEnumerable<Payment>> GetByReservationIdAsync(int reservationId);
    Task<Payment> AddAsync(Payment payment);
    Task<bool> ExistsByTransactionReferenceAsync(string reference);
    Task<decimal> GetTotalPaidForReservationAsync(int reservationId);
    Task<IEnumerable<Payment>> GetPaymentsForReportAsync(DateOnly? startDate, DateOnly? endDate);
}
