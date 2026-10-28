using BookingService.DTOs;
using BookingService.Entities;
using BookingService.Repositories;
using BookingService.Services.Http;
using HotelManagement.Shared.Constants;
using HotelManagement.Shared.Exceptions;

namespace BookingService.Services;

public interface IReservationService
{
    Task<IEnumerable<ReservationResponseDto>> GetAllAsync(string? status = null);
    Task<ReservationResponseDto> GetByIdAsync(int id);
    Task<ReservationResponseDto> CreateAsync(CreateReservationRequestDto request);
    Task<ReservationResponseDto> UpdateAsync(int id, UpdateReservationRequestDto request);
    Task<ReservationResponseDto> CancelAsync(int id);
    Task<ReservationResponseDto> CheckInAsync(int id);
    Task<ReservationResponseDto> CheckOutAsync(int id);
    Task<IEnumerable<ReservationResponseDto>> GetTodayReservationsAsync();
}

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IGuestRepository _guestRepository;
    private readonly IHotelServiceClient _hotelServiceClient;

    public ReservationService(
        IReservationRepository reservationRepository,
        IGuestRepository guestRepository,
        IHotelServiceClient hotelServiceClient)
    {
        _reservationRepository = reservationRepository;
        _guestRepository = guestRepository;
        _hotelServiceClient = hotelServiceClient;
    }

    public async Task<IEnumerable<ReservationResponseDto>> GetAllAsync(string? status = null)
    {
        var reservations = await _reservationRepository.GetAllAsync(status);
        return reservations.Select(MapToDto);
    }

    public async Task<ReservationResponseDto> GetByIdAsync(int id)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id, includeDetails: true);
        if (reservation == null)
        {
            throw new NotFoundException($"Reservation with ID {id} not found.");
        }

        return MapToDto(reservation);
    }

    public async Task<ReservationResponseDto> CreateAsync(CreateReservationRequestDto request)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        if (request.CheckInDate < today)
        {
            throw new BadRequestException("Check-in date cannot be in the past.");
        }

        if (request.CheckOutDate <= request.CheckInDate)
        {
            throw new BadRequestException("Check-out date must be strictly after the check-in date.");
        }

        if (request.NumberOfAdults < 1)
        {
            throw new BadRequestException("At least one adult is required.");
        }

        if (request.NumberOfChildren < 0)
        {
            throw new BadRequestException("Number of children cannot be negative.");
        }

        var guest = await _guestRepository.GetByIdAsync(request.GuestId);
        if (guest == null)
        {
            throw new NotFoundException($"Guest with ID {request.GuestId} not found.");
        }

        // Fetch room info from Hotel Service synchronously over HTTP
        var room = await _hotelServiceClient.GetRoomDetailsAsync(request.RoomId);
        if (room == null)
        {
            throw new NotFoundException($"Room with ID {request.RoomId} not found in Hotel Service.");
        }

        if (room.Status != RoomStatuses.Available)
        {
            throw new BadRequestException($"Room {room.RoomNumber} is currently '{room.Status}' and cannot be reserved.");
        }

        var totalGuests = request.NumberOfAdults + request.NumberOfChildren;
        if (totalGuests > room.Capacity)
        {
            throw new BadRequestException($"Requested guests ({totalGuests}) exceed room capacity ({room.Capacity}).");
        }

        // Concurrency-safe final conflict check
        var conflicts = await _reservationRepository.GetConflictingReservationsAsync(
            request.RoomId, request.CheckInDate, request.CheckOutDate);

        if (conflicts.Any())
        {
            throw new ConflictException("The selected room is not available for the requested dates due to an existing booking.");
        }

        var nights = (request.CheckOutDate.ToDateTime(TimeOnly.MinValue) - request.CheckInDate.ToDateTime(TimeOnly.MinValue)).Days;
        var totalAmount = nights * room.PricePerNight;

        var reservation = new Reservation
        {
            GuestId = request.GuestId,
            RoomId = request.RoomId,
            CheckInDate = request.CheckInDate,
            CheckOutDate = request.CheckOutDate,
            NumberOfAdults = request.NumberOfAdults,
            NumberOfChildren = request.NumberOfChildren,
            PricePerNightSnapshot = room.PricePerNight,
            TotalAmount = totalAmount,
            ReservationStatus = ReservationStatuses.Booked,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _reservationRepository.AddAsync(reservation);
        created.Guest = guest;

        return MapToDto(created);
    }

    public async Task<ReservationResponseDto> UpdateAsync(int id, UpdateReservationRequestDto request)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id, includeDetails: true);
        if (reservation == null)
        {
            throw new NotFoundException($"Reservation with ID {id} not found.");
        }

        if (reservation.ReservationStatus != ReservationStatuses.Booked)
        {
            throw new BadRequestException($"Only reservations in 'Booked' status can be modified. Current status is '{reservation.ReservationStatus}'.");
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        if (request.CheckInDate < today)
        {
            throw new BadRequestException("Check-in date cannot be in the past.");
        }

        if (request.CheckOutDate <= request.CheckInDate)
        {
            throw new BadRequestException("Check-out date must be strictly after the check-in date.");
        }

        if (request.NumberOfAdults < 1)
        {
            throw new BadRequestException("At least one adult is required.");
        }

        if (request.NumberOfChildren < 0)
        {
            throw new BadRequestException("Number of children cannot be negative.");
        }

        var room = await _hotelServiceClient.GetRoomDetailsAsync(reservation.RoomId);
        if (room != null)
        {
            var totalGuests = request.NumberOfAdults + request.NumberOfChildren;
            if (totalGuests > room.Capacity)
            {
                throw new BadRequestException($"Requested guests ({totalGuests}) exceed room capacity ({room.Capacity}).");
            }
        }

        var conflicts = await _reservationRepository.GetConflictingReservationsAsync(
            reservation.RoomId, request.CheckInDate, request.CheckOutDate, excludeReservationId: id);

        if (conflicts.Any())
        {
            throw new ConflictException("The room is not available for the newly requested dates due to an existing booking.");
        }

        var nights = (request.CheckOutDate.ToDateTime(TimeOnly.MinValue) - request.CheckInDate.ToDateTime(TimeOnly.MinValue)).Days;
        
        // Use saved PricePerNightSnapshot so room price fluctuations don't alter contract price
        reservation.CheckInDate = request.CheckInDate;
        reservation.CheckOutDate = request.CheckOutDate;
        reservation.NumberOfAdults = request.NumberOfAdults;
        reservation.NumberOfChildren = request.NumberOfChildren;
        reservation.TotalAmount = nights * reservation.PricePerNightSnapshot;

        await _reservationRepository.UpdateAsync(reservation);
        return MapToDto(reservation);
    }

    public async Task<ReservationResponseDto> CancelAsync(int id)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id, includeDetails: true);
        if (reservation == null)
        {
            throw new NotFoundException($"Reservation with ID {id} not found.");
        }

        if (reservation.ReservationStatus != ReservationStatuses.Booked)
        {
            throw new BadRequestException($"Cannot cancel reservation in '{reservation.ReservationStatus}' status. Only 'Booked' reservations can be cancelled.");
        }

        reservation.ReservationStatus = ReservationStatuses.Cancelled;
        await _reservationRepository.UpdateAsync(reservation);
        return MapToDto(reservation);
    }

    public async Task<ReservationResponseDto> CheckInAsync(int id)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id, includeDetails: true);
        if (reservation == null)
        {
            throw new NotFoundException($"Reservation with ID {id} not found.");
        }

        if (reservation.ReservationStatus != ReservationStatuses.Booked)
        {
            throw new BadRequestException($"Cannot check in reservation in '{reservation.ReservationStatus}' status. Status must be 'Booked'.");
        }

        reservation.ReservationStatus = ReservationStatuses.CheckedIn;
        await _reservationRepository.UpdateAsync(reservation);
        return MapToDto(reservation);
    }

    public async Task<ReservationResponseDto> CheckOutAsync(int id)
    {
        var reservation = await _reservationRepository.GetByIdAsync(id, includeDetails: true);
        if (reservation == null)
        {
            throw new NotFoundException($"Reservation with ID {id} not found.");
        }

        if (reservation.ReservationStatus != ReservationStatuses.CheckedIn)
        {
            throw new BadRequestException($"Cannot check out reservation in '{reservation.ReservationStatus}' status. Status must be 'CheckedIn'.");
        }

        reservation.ReservationStatus = ReservationStatuses.CheckedOut;
        await _reservationRepository.UpdateAsync(reservation);
        return MapToDto(reservation);
    }

    public async Task<IEnumerable<ReservationResponseDto>> GetTodayReservationsAsync()
    {
        var reservations = await _reservationRepository.GetTodayReservationsAsync();
        return reservations.Select(MapToDto);
    }

    private static ReservationResponseDto MapToDto(Reservation reservation) =>
        new(
            reservation.ReservationId,
            reservation.GuestId,
            reservation.Guest?.FullName ?? string.Empty,
            reservation.Guest?.PhoneNumber ?? string.Empty,
            reservation.RoomId,
            reservation.CheckInDate,
            reservation.CheckOutDate,
            reservation.NumberOfAdults,
            reservation.NumberOfChildren,
            reservation.PricePerNightSnapshot,
            reservation.TotalAmount,
            reservation.ReservationStatus,
            reservation.CreatedAt,
            reservation.UpdatedAt
        );
}
