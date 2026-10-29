namespace HotelService.Common.Constants;

public static class Roles
{
    public const string Owner = "Owner";
    public const string Manager = "Manager";
    public const string Receptionist = "Receptionist";

    public static readonly string[] All = [Owner, Manager, Receptionist];

    public static bool IsValid(string role) =>
        role is Owner or Manager or Receptionist;
}

public static class RoomStatuses
{
    public const string Available = "Available";
    public const string Occupied = "Occupied";
    public const string Maintenance = "Maintenance";
    public const string Inactive = "Inactive";

    public static readonly string[] All = [Available, Occupied, Maintenance, Inactive];

    public static bool IsValid(string status) =>
        status is Available or Occupied or Maintenance or Inactive;
}
