using HotelService.Data;
using HotelService.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelService.Repositories;

public class RoomRepository : IRoomRepository
{
    private readonly HotelDbContext _context;

    public RoomRepository(HotelDbContext context)
    {
        _context = context;
    }

    public async Task<Room?> GetByIdAsync(int id)
    {
        return await _context.Rooms
            .Include(r => r.RoomType)
            .FirstOrDefaultAsync(r => r.RoomId == id);
    }

    public async Task<Room?> GetByRoomNumberAsync(string roomNumber)
    {
        return await _context.Rooms
            .Include(r => r.RoomType)
            .FirstOrDefaultAsync(r => r.RoomNumber.ToLower() == roomNumber.ToLower());
    }

    public async Task<IEnumerable<Room>> GetAllAsync(string? status = null, int? roomTypeId = null)
    {
        var query = _context.Rooms.Include(r => r.RoomType).AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(r => r.Status.ToLower() == status.ToLower());
        }

        if (roomTypeId.HasValue)
        {
            query = query.Where(r => r.RoomTypeId == roomTypeId.Value);
        }

        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<Room> AddAsync(Room room)
    {
        await _context.Rooms.AddAsync(room);
        await _context.SaveChangesAsync();
        return room;
    }

    public async Task UpdateAsync(Room room)
    {
        _context.Rooms.Update(room);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Room room)
    {
        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsByRoomNumberAsync(string roomNumber)
    {
        return await _context.Rooms.AnyAsync(r => r.RoomNumber.ToLower() == roomNumber.ToLower());
    }
}
