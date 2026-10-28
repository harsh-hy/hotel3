using HotelService.Entities;

namespace HotelService.Repositories;

public interface IRoomTypeRepository
{
    Task<RoomType?> GetByIdAsync(int id);
    Task<RoomType?> GetByNameAsync(string name);
    Task<IEnumerable<RoomType>> GetAllAsync();
    Task<RoomType> AddAsync(RoomType roomType);
    Task UpdateAsync(RoomType roomType);
    Task<bool> ExistsByNameAsync(string name);
    Task<bool> HasAssociatedRoomsAsync(int roomTypeId);
}

public interface IRoomRepository
{
    Task<Room?> GetByIdAsync(int id);
    Task<Room?> GetByRoomNumberAsync(string roomNumber);
    Task<IEnumerable<Room>> GetAllAsync(string? status = null, int? roomTypeId = null);
    Task<Room> AddAsync(Room room);
    Task UpdateAsync(Room room);
    Task DeleteAsync(Room room);
    Task<bool> ExistsByRoomNumberAsync(string roomNumber);
}
