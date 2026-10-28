using System.Net.Http.Json;
using BookingService.DTOs;
using Microsoft.Extensions.Logging;

namespace BookingService.Services.Http;

public interface IHotelServiceClient
{
    Task<ExternalRoomDto?> GetRoomDetailsAsync(int roomId);
}

public class HotelServiceClient : IHotelServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HotelServiceClient> _logger;

    public HotelServiceClient(HttpClient httpClient, ILogger<HotelServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ExternalRoomDto?> GetRoomDetailsAsync(int roomId)
    {
        try
        {
            var room = await _httpClient.GetFromJsonAsync<ExternalRoomDto>($"/api/hotel/rooms/{roomId}");
            return room;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Hotel Service for room {RoomId}.", roomId);
            return null;
        }
    }
}
