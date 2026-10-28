namespace HotelService.DTOs;

public record CreateRoomTypeRequestDto(string TypeName, int Capacity, string? Description);

public record UpdateRoomTypeRequestDto(string TypeName, int Capacity, string? Description);

public record RoomTypeResponseDto(int RoomTypeId, string TypeName, int Capacity, string? Description);

public record CreateRoomRequestDto(string RoomNumber, int RoomTypeId, decimal PricePerNight, string Status, string? Description);

public record UpdateRoomRequestDto(string RoomNumber, int RoomTypeId, decimal PricePerNight, string Status, string? Description);

public record UpdateRoomPriceRequestDto(decimal PricePerNight);

public record RoomResponseDto(int RoomId, string RoomNumber, int RoomTypeId, string TypeName, int Capacity, decimal PricePerNight, string Status, string? Description);

public record RoomAvailabilityResponseDto(int RoomId, string RoomNumber, int RoomTypeId, string TypeName, int Capacity, decimal PricePerNight, string Status, bool IsAvailable, string? Reason);
