namespace IdentityService.Common.Constants;

public static class Roles
{
    public const string Owner = "Owner";
    public const string Manager = "Manager";
    public const string Receptionist = "Receptionist";

    public static readonly string[] All = [Owner, Manager, Receptionist];

    public static bool IsValid(string role) =>
        role is Owner or Manager or Receptionist;
}
