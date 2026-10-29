using BookingService.DTOs;
using BookingService.Entities;
using BookingService.Repositories;
using BookingService.Common.Exceptions;

namespace BookingService.Services;

public interface IGuestService
{
    Task<IEnumerable<GuestResponseDto>> GetAllAsync(string? search = null);
    Task<GuestResponseDto> GetByIdAsync(int id);
    Task<GuestResponseDto> CreateAsync(CreateGuestRequestDto request);
    Task<GuestResponseDto> UpdateAsync(int id, UpdateGuestRequestDto request);
}

public class GuestService : IGuestService
{
    private readonly IGuestRepository _repository;

    public GuestService(IGuestRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<GuestResponseDto>> GetAllAsync(string? search = null)
    {
        var guests = await _repository.GetAllAsync(search);
        return guests.Select(MapToDto);
    }

    public async Task<GuestResponseDto> GetByIdAsync(int id)
    {
        var guest = await _repository.GetByIdAsync(id);
        if (guest == null)
        {
            throw new NotFoundException($"Guest with ID {id} not found.");
        }

        return MapToDto(guest);
    }

    public async Task<GuestResponseDto> CreateAsync(CreateGuestRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            throw new BadRequestException("Guest full name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            throw new BadRequestException("Guest phone number is required.");
        }

        var guest = new Guest
        {
            FullName = request.FullName.Trim(),
            PhoneNumber = request.PhoneNumber.Trim(),
            Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim(),
            Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        var created = await _repository.AddAsync(guest);
        return MapToDto(created);
    }

    public async Task<GuestResponseDto> UpdateAsync(int id, UpdateGuestRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
        {
            throw new BadRequestException("Guest full name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            throw new BadRequestException("Guest phone number is required.");
        }

        var guest = await _repository.GetByIdAsync(id);
        if (guest == null)
        {
            throw new NotFoundException($"Guest with ID {id} not found.");
        }

        guest.FullName = request.FullName.Trim();
        guest.PhoneNumber = request.PhoneNumber.Trim();
        guest.Email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
        guest.Address = string.IsNullOrWhiteSpace(request.Address) ? null : request.Address.Trim();

        await _repository.UpdateAsync(guest);
        return MapToDto(guest);
    }

    private static GuestResponseDto MapToDto(Guest guest) =>
        new(guest.GuestId, guest.FullName, guest.PhoneNumber, guest.Email, guest.Address, guest.CreatedAt);
}
