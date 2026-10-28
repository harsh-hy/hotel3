using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace HotelService.Services.Http;

public interface IBookingServiceClient
{
    Task<HashSet<int>> GetConflictingRoomIdsAsync(DateOnly checkIn, DateOnly checkOut);
    Task<bool> HasActiveReservationsForRoomAsync(int roomId);
}

public class BookingServiceClient : IBookingServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BookingServiceClient> _logger;

    public BookingServiceClient(HttpClient httpClient, ILogger<BookingServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<HashSet<int>> GetConflictingRoomIdsAsync(DateOnly checkIn, DateOnly checkOut)
    {
        try
        {
            var url = $"/api/booking/internal/reservations/conflicting-room-ids?checkIn={checkIn:yyyy-MM-dd}&checkOut={checkOut:yyyy-MM-dd}";
            var response = await _httpClient.GetFromJsonAsync<List<int>>(url);
            return response != null ? new HashSet<int>(response) : new HashSet<int>();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to query Booking Service for conflicting room IDs. Assuming no active conflicts.");
            return new HashSet<int>();
        }
    }

    public async Task<bool> HasActiveReservationsForRoomAsync(int roomId)
    {
        try
        {
            var url = $"/api/booking/internal/rooms/{roomId}/has-active-reservations";
            var response = await _httpClient.GetFromJsonAsync<bool>(url);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to query Booking Service for active reservations for room {RoomId}.", roomId);
            return false;
        }
    }
}
