using HotelService.DTOs;
using HotelService.Entities;
using HotelService.Repositories;
using HotelManagement.Shared.Exceptions;

namespace HotelService.Services;

public interface IRoomTypeService
{
    Task<IEnumerable<RoomTypeResponseDto>> GetAllAsync();
    Task<RoomTypeResponseDto> GetByIdAsync(int id);
    Task<RoomTypeResponseDto> CreateAsync(CreateRoomTypeRequestDto request);
    Task<RoomTypeResponseDto> UpdateAsync(int id, UpdateRoomTypeRequestDto request);
}

public class RoomTypeService : IRoomTypeService
{
    private readonly IRoomTypeRepository _repository;

    public RoomTypeService(IRoomTypeRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<RoomTypeResponseDto>> GetAllAsync()
    {
        var types = await _repository.GetAllAsync();
        return types.Select(MapToDto);
    }

    public async Task<RoomTypeResponseDto> GetByIdAsync(int id)
    {
        var type = await _repository.GetByIdAsync(id);
        if (type == null)
        {
            throw new NotFoundException($"Room type with ID {id} not found.");
        }

        return MapToDto(type);
    }

    public async Task<RoomTypeResponseDto> CreateAsync(CreateRoomTypeRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.TypeName))
        {
            throw new BadRequestException("Room type name is required.");
        }

        if (request.Capacity <= 0)
        {
            throw new BadRequestException("Capacity must be greater than zero.");
        }

        var normalizedName = request.TypeName.Trim();
        if (await _repository.ExistsByNameAsync(normalizedName))
        {
            throw new ConflictException($"Room type '{normalizedName}' already exists.");
        }

        var roomType = new RoomType
        {
            TypeName = normalizedName,
            Capacity = request.Capacity,
            Description = request.Description?.Trim()
        };

        var created = await _repository.AddAsync(roomType);
        return MapToDto(created);
    }

    public async Task<RoomTypeResponseDto> UpdateAsync(int id, UpdateRoomTypeRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.TypeName))
        {
            throw new BadRequestException("Room type name is required.");
        }

        if (request.Capacity <= 0)
        {
            throw new BadRequestException("Capacity must be greater than zero.");
        }

        var roomType = await _repository.GetByIdAsync(id);
        if (roomType == null)
        {
            throw new NotFoundException($"Room type with ID {id} not found.");
        }

        var normalizedName = request.TypeName.Trim();
        var existing = await _repository.GetByNameAsync(normalizedName);
        if (existing != null && existing.RoomTypeId != id)
        {
            throw new ConflictException($"Room type '{normalizedName}' already exists.");
        }

        roomType.TypeName = normalizedName;
        roomType.Capacity = request.Capacity;
        roomType.Description = request.Description?.Trim();

        await _repository.UpdateAsync(roomType);
        return MapToDto(roomType);
    }

    private static RoomTypeResponseDto MapToDto(RoomType roomType) =>
        new(roomType.RoomTypeId, roomType.TypeName, roomType.Capacity, roomType.Description);
}
