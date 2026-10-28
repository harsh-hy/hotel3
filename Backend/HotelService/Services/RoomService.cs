using HotelService.DTOs;
using HotelService.Entities;
using HotelService.Repositories;
using HotelService.Services.Http;
using HotelManagement.Shared.Constants;
using HotelManagement.Shared.Exceptions;

namespace HotelService.Services;

public interface IRoomService
{
    Task<IEnumerable<RoomResponseDto>> GetAllAsync(string? status = null, int? roomTypeId = null);
    Task<RoomResponseDto> GetByIdAsync(int id);
    Task<RoomResponseDto> CreateAsync(CreateRoomRequestDto request);
    Task<RoomResponseDto> UpdateAsync(int id, UpdateRoomRequestDto request);
    Task<RoomResponseDto> UpdatePriceAsync(int id, UpdateRoomPriceRequestDto request);
    Task DeleteAsync(int id);
}

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepository;
    private readonly IRoomTypeRepository _roomTypeRepository;
    private readonly IBookingServiceClient _bookingServiceClient;

    public RoomService(
        IRoomRepository roomRepository, 
        IRoomTypeRepository roomTypeRepository,
        IBookingServiceClient bookingServiceClient)
    {
        _roomRepository = roomRepository;
        _roomTypeRepository = roomTypeRepository;
        _bookingServiceClient = bookingServiceClient;
    }

    public async Task<IEnumerable<RoomResponseDto>> GetAllAsync(string? status = null, int? roomTypeId = null)
    {
        var rooms = await _roomRepository.GetAllAsync(status, roomTypeId);
        return rooms.Select(MapToDto);
    }

    public async Task<RoomResponseDto> GetByIdAsync(int id)
    {
        var room = await _roomRepository.GetByIdAsync(id);
        if (room == null)
        {
            throw new NotFoundException($"Room with ID {id} not found.");
        }

        return MapToDto(room);
    }

    public async Task<RoomResponseDto> CreateAsync(CreateRoomRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.RoomNumber))
        {
            throw new BadRequestException("Room number is required.");
        }

        if (request.PricePerNight < 0)
        {
            throw new BadRequestException("Price per night cannot be negative.");
        }

        if (!RoomStatuses.IsValid(request.Status))
        {
            throw new BadRequestException($"Invalid room status. Allowed statuses are: {string.Join(", ", RoomStatuses.All)}.");
        }

        var roomType = await _roomTypeRepository.GetByIdAsync(request.RoomTypeId);
        if (roomType == null)
        {
            throw new NotFoundException($"Room type with ID {request.RoomTypeId} does not exist.");
        }

        var normalizedNumber = request.RoomNumber.Trim();
        if (await _roomRepository.ExistsByRoomNumberAsync(normalizedNumber))
        {
            throw new ConflictException($"Room number '{normalizedNumber}' already exists.");
        }

        var room = new Room
        {
            RoomNumber = normalizedNumber,
            RoomTypeId = request.RoomTypeId,
            PricePerNight = request.PricePerNight,
            Status = request.Status,
            Description = request.Description?.Trim()
        };

        var created = await _roomRepository.AddAsync(room);
        created.RoomType = roomType;
        return MapToDto(created);
    }

    public async Task<RoomResponseDto> UpdateAsync(int id, UpdateRoomRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.RoomNumber))
        {
            throw new BadRequestException("Room number is required.");
        }

        if (request.PricePerNight < 0)
        {
            throw new BadRequestException("Price per night cannot be negative.");
        }

        if (!RoomStatuses.IsValid(request.Status))
        {
            throw new BadRequestException($"Invalid room status. Allowed statuses are: {string.Join(", ", RoomStatuses.All)}.");
        }

        var room = await _roomRepository.GetByIdAsync(id);
        if (room == null)
        {
            throw new NotFoundException($"Room with ID {id} not found.");
        }

        var roomType = await _roomTypeRepository.GetByIdAsync(request.RoomTypeId);
        if (roomType == null)
        {
            throw new NotFoundException($"Room type with ID {request.RoomTypeId} does not exist.");
        }

        var normalizedNumber = request.RoomNumber.Trim();
        var existing = await _roomRepository.GetByRoomNumberAsync(normalizedNumber);
        if (existing != null && existing.RoomId != id)
        {
            throw new ConflictException($"Room number '{normalizedNumber}' is already in use.");
        }

        room.RoomNumber = normalizedNumber;
        room.RoomTypeId = request.RoomTypeId;
        room.PricePerNight = request.PricePerNight;
        room.Status = request.Status;
        room.Description = request.Description?.Trim();
        room.RoomType = roomType;

        await _roomRepository.UpdateAsync(room);
        return MapToDto(room);
    }

    public async Task<RoomResponseDto> UpdatePriceAsync(int id, UpdateRoomPriceRequestDto request)
    {
        if (request.PricePerNight < 0)
        {
            throw new BadRequestException("Price per night cannot be negative.");
        }

        var room = await _roomRepository.GetByIdAsync(id);
        if (room == null)
        {
            throw new NotFoundException($"Room with ID {id} not found.");
        }

        room.PricePerNight = request.PricePerNight;
        await _roomRepository.UpdateAsync(room);
        return MapToDto(room);
    }

    public async Task DeleteAsync(int id)
    {
        var room = await _roomRepository.GetByIdAsync(id);
        if (room == null)
        {
            throw new NotFoundException($"Room with ID {id} not found.");
        }

        if (room.Status == RoomStatuses.Occupied)
        {
            throw new ConflictException("Cannot delete room while it is currently occupied.");
        }

        var hasReservations = await _bookingServiceClient.HasActiveReservationsForRoomAsync(id);
        if (hasReservations)
        {
            throw new ConflictException("Cannot delete room with active or future reservations. Set its status to 'Inactive' instead.");
        }

        await _roomRepository.DeleteAsync(room);
    }

    private static RoomResponseDto MapToDto(Room room) =>
        new(room.RoomId, 
            room.RoomNumber, 
            room.RoomTypeId, 
            room.RoomType?.TypeName ?? string.Empty, 
            room.RoomType?.Capacity ?? 0, 
            room.PricePerNight, 
            room.Status, 
            room.Description);
}
