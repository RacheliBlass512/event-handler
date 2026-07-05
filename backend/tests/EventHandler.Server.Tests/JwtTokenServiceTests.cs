using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EventHandler.Domain.Entities;
using EventHandler.Domain.Enums;
using EventHandler.Server.Infrastructure.Auth;
using Microsoft.Extensions.Configuration;

namespace EventHandler.Server.Tests;

public class JwtTokenServiceTests
{
    private static IConfiguration BuildConfiguration() => new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Jwt:Secret"] = "unit-test-secret-at-least-32-characters-long",
            ["Jwt:Issuer"] = "EventHandler.Server.Tests",
            ["Jwt:Audience"] = "EventHandler.Client.Tests",
            ["Jwt:LifetimeMinutes"] = "30",
        })
        .Build();

    [Fact]
    public void GenerateToken_ProducesTokenWithExpectedClaimsAndExpiry()
    {
        var sut = new JwtTokenService(BuildConfiguration());
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "tech1",
            PasswordHash = "irrelevant",
            Role = UserRole.Technician,
            DisplayName = "Tom Technician",
        };

        var (token, expiresAt) = sut.GenerateToken(user);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Equal(user.Id.ToString(), jwt.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value);
        Assert.Equal("Technician", jwt.Claims.Single(c => c.Type == ClaimTypes.Role).Value);
        Assert.Equal("EventHandler.Server.Tests", jwt.Issuer);
        Assert.Equal("EventHandler.Client.Tests", jwt.Audiences.Single());
        Assert.Equal(expiresAt, jwt.ValidTo, TimeSpan.FromSeconds(1));
        Assert.True(expiresAt > DateTime.UtcNow.AddMinutes(29) && expiresAt <= DateTime.UtcNow.AddMinutes(30));
    }
}
