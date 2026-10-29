using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace HotelService.Common.Security;

public static class JwtConfiguration
{
    public const string DefaultSecretKey = "HotelManagementSystemSecretKey2026!AuthoritativeJwtSigningKey#SecureHotel123456";
    public const string DefaultIssuer = "HotelManagement.IdentityService";
    public const string DefaultAudience = "HotelManagement.Clients";

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var secretKey = configuration["Jwt:SecretKey"] ?? DefaultSecretKey;
        var issuer = configuration["Jwt:Issuer"] ?? DefaultIssuer;
        var audience = configuration["Jwt:Audience"] ?? DefaultAudience;

        var key = Encoding.UTF8.GetBytes(secretKey);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                NameClaimType = ClaimTypes.Name,
                RoleClaimType = ClaimTypes.Role
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(authHeader))
                    {
                        if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        {
                            context.Token = authHeader["Bearer ".Length..].Trim();
                        }
                        else if (!authHeader.Contains(' '))
                        {
                            // Handles user pasting token directly into Swagger without typing "Bearer "
                            context.Token = authHeader.Trim();
                        }
                    }
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    if (context.Principal?.Identity is ClaimsIdentity identity)
                    {
                        // Ensure both ClaimTypes.Role and "role" claims exist for RBAC
                        var roles = identity.FindAll(c => c.Type == ClaimTypes.Role || c.Type == "role").Select(c => c.Value).Distinct().ToList();
                        foreach (var role in roles)
                        {
                            if (!identity.HasClaim(ClaimTypes.Role, role))
                            {
                                identity.AddClaim(new Claim(ClaimTypes.Role, role));
                            }
                            if (!identity.HasClaim("role", role))
                            {
                                identity.AddClaim(new Claim("role", role));
                            }
                        }
                    }
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization();

        return services;
    }

    public static IServiceCollection AddSharedJwtAuthentication(this IServiceCollection services, IConfiguration configuration) =>
        AddJwtAuthentication(services, configuration);

    public static int? GetUserId(this ClaimsPrincipal principal)
    {
        var idClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                      ?? principal.FindFirst("sub")?.Value 
                      ?? principal.FindFirst("userId")?.Value;

        return int.TryParse(idClaim, out var id) ? id : null;
    }

    public static string? GetUsername(this ClaimsPrincipal principal) =>
        principal.FindFirst(ClaimTypes.Name)?.Value 
        ?? principal.FindFirst("username")?.Value;

    public static string? GetRole(this ClaimsPrincipal principal) =>
        principal.FindFirst(ClaimTypes.Role)?.Value 
        ?? principal.FindFirst("role")?.Value;
}
