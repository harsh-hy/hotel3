using BookingService.Data;
using BookingService.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Repositories;

public class GuestRepository : IGuestRepository
{
    private readonly BookingDbContext _context;

    public GuestRepository(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<Guest?> GetByIdAsync(int id)
    {
        return await _context.Guests.FindAsync(id);
    }

    public async Task<IEnumerable<Guest>> GetAllAsync(string? search = null)
    {
        var query = _context.Guests.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(g => g.FullName.ToLower().Contains(term) || g.PhoneNumber.Contains(term));
        }

        return await query.AsNoTracking().ToListAsync();
    }

    public async Task<Guest> AddAsync(Guest guest)
    {
        await _context.Guests.AddAsync(guest);
        await _context.SaveChangesAsync();
        return guest;
    }

    public async Task UpdateAsync(Guest guest)
    {
        _context.Guests.Update(guest);
        await _context.SaveChangesAsync();
    }
}
