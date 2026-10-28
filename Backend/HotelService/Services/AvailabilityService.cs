using HotelService.DTOs;
using HotelService.Repositories;
using HotelService.Services.Http;
using HotelManagement.Shared.Constants;
using HotelManagement.Shared.Exceptions;

namespace HotelService.Services;

public interface IAvailabilityService
{
    Task<IEnumerable<RoomAvailabilityResponseDto>> SearchAvailableAsync(DateOnly checkIn, DateOnly checkOut, int adults, int children);
    Task<RoomAvailabilityResponseDto> CheckRoomAvailabilityAsync(int roomId, DateOnly checkIn, DateOnly checkOut);
}

public class AvailabilityService : IAvailabilityService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IBookingServiceClient _bookingServiceClient;

    public AvailabilityService(IRoomRepository roomRepository, IBookingServiceClient bookingServiceClient)
    {
        _roomRepository = roomRepository;
        _bookingServiceClient = bookingServiceClient;
    }

    public async Task<IEnumerable<RoomAvailabilityResponseDto>> SearchAvailableAsync(
        DateOnly checkIn, DateOnly checkOut, int adults, int children)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        if (checkIn < today)
        {
            throw new BadRequestException("Check-in date cannot be in the past.");
        }

        if (checkOut <= checkIn)
        {
            throw new BadRequestException("Check-out date must be after the check-in date.");
        }

        if (adults < 1)
        {
            throw new BadRequestException("At least one adult is required.");
        }

        if (children < 0)
        {
            throw new BadRequestException("Number of children cannot be negative.");
        }

        var totalGuests = adults + children;

        // Synchronously fetch conflicting room IDs from Booking Service
        var conflictingRoomIds = await _bookingServiceClient.GetConflictingRoomIdsAsync(checkIn, checkOut);

        // Fetch rooms that are operationally Available
        var allRooms = await _roomRepository.GetAllAsync(status: RoomStatuses.Available);

        var availableRooms = allRooms
            .Where(r => (r.RoomType?.Capacity ?? 0) >= totalGuests)
            .Where(r => !conflictingRoomIds.Contains(r.RoomId))
            .Select(r => new RoomAvailabilityResponseDto(
                r.RoomId,
                r.RoomNumber,
                r.RoomTypeId,
                r.RoomType?.TypeName ?? string.Empty,
                r.RoomType?.Capacity ?? 0,
                r.PricePerNight,
                r.Status,
                true,
                null
            ));

        return availableRooms;
    }

    public async Task<RoomAvailabilityResponseDto> CheckRoomAvailabilityAsync(int roomId, DateOnly checkIn, DateOnly checkOut)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        if (checkIn < today)
        {
            throw new BadRequestException("Check-in date cannot be in the past.");
        }

        if (checkOut <= checkIn)
        {
            throw new BadRequestException("Check-out date must be after the check-in date.");
        }

        var room = await _roomRepository.GetByIdAsync(roomId);
        if (room == null)
        {
            throw new NotFoundException($"Room with ID {roomId} not found.");
        }

        if (room.Status != RoomStatuses.Available)
        {
            return new RoomAvailabilityResponseDto(
                room.RoomId,
                room.RoomNumber,
                room.RoomTypeId,
                room.RoomType?.TypeName ?? string.Empty,
                room.RoomType?.Capacity ?? 0,
                room.PricePerNight,
                room.Status,
                false,
                $"Room is currently marked as '{room.Status}'."
            );
        }

        var conflictingRoomIds = await _bookingServiceClient.GetConflictingRoomIdsAsync(checkIn, checkOut);
        var isBooked = conflictingRoomIds.Contains(roomId);

        return new RoomAvailabilityResponseDto(
            room.RoomId,
            room.RoomNumber,
            room.RoomTypeId,
            room.RoomType?.TypeName ?? string.Empty,
            room.RoomType?.Capacity ?? 0,
            room.PricePerNight,
            room.Status,
            !isBooked,
            isBooked ? "Room has an active reservation during the requested dates." : null
        );
    }
}
