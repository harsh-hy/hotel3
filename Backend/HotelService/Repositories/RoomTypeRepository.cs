using HotelService.Data;
using HotelService.Entities;
using Microsoft.EntityFrameworkCore;

namespace HotelService.Repositories;

public class RoomTypeRepository : IRoomTypeRepository
{
    private readonly HotelDbContext _context;

    public RoomTypeRepository(HotelDbContext context)
    {
        _context = context;
    }

    public async Task<RoomType?> GetByIdAsync(int id)
    {
        return await _context.RoomTypes.FindAsync(id);
    }

    public async Task<RoomType?> GetByNameAsync(string name)
    {
        return await _context.RoomTypes.FirstOrDefaultAsync(rt => rt.TypeName.ToLower() == name.ToLower());
    }

    public async Task<IEnumerable<RoomType>> GetAllAsync()
    {
        return await _context.RoomTypes.AsNoTracking().ToListAsync();
    }

    public async Task<RoomType> AddAsync(RoomType roomType)
    {
        await _context.RoomTypes.AddAsync(roomType);
        await _context.SaveChangesAsync();
        return roomType;
    }

    public async Task UpdateAsync(RoomType roomType)
    {
        _context.RoomTypes.Update(roomType);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.RoomTypes.AnyAsync(rt => rt.TypeName.ToLower() == name.ToLower());
    }

    public async Task<bool> HasAssociatedRoomsAsync(int roomTypeId)
    {
        return await _context.Rooms.AnyAsync(r => r.RoomTypeId == roomTypeId);
    }
}
