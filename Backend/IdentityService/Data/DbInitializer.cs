using IdentityService.Entities;
using IdentityService.Repositories;
using IdentityService.Common.Constants;
using Microsoft.Extensions.Logging;

namespace IdentityService.Data;

public static class DbInitializer
{
    public static async Task SeedOwnerAsync(IUserRepository userRepository, ILogger logger)
    {
        try
        {
            if (!await userRepository.AnyAsync())
            {
                logger.LogInformation("Seeding initial Owner user...");
                var owner = new User
                {
                    Username = "owner",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("OwnerPassword123!"),
                    FullName = "Hotel Owner",
                    Role = Roles.Owner,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await userRepository.AddAsync(owner);
                logger.LogInformation("Default Owner seeded successfully with username 'owner'.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while seeding initial Owner user.");
        }
    }
}
