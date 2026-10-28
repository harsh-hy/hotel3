namespace HotelManagement.Shared.Constants;

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

public static class ReservationStatuses
{
    public const string Booked = "Booked";
    public const string CheckedIn = "CheckedIn";
    public const string CheckedOut = "CheckedOut";
    public const string Cancelled = "Cancelled";

    public static readonly string[] All = [Booked, CheckedIn, CheckedOut, Cancelled];

    public static bool IsValid(string status) =>
        status is Booked or CheckedIn or CheckedOut or Cancelled;
}

public static class PaymentMethods
{
    public const string Cash = "Cash";
    public const string Card = "Card";
    public const string UPI = "UPI";

    public static readonly string[] All = [Cash, Card, UPI];

    public static bool IsValid(string method) =>
        method is Cash or Card or UPI;
}

public static class PaymentStatuses
{
    public const string Pending = "Pending";
    public const string Paid = "Paid";
    public const string Failed = "Failed";

    public static readonly string[] All = [Pending, Paid, Failed];

    public static bool IsValid(string status) =>
        status is Pending or Paid or Failed;
}
