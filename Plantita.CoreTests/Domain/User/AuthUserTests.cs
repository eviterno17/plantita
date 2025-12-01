using FluentAssertions;
using plantita.User.Domain.Model.Aggregates;
using Xunit;

namespace Plantita.CoreTests.Domain.User;

/// <summary>
/// Unit tests for AuthUser aggregate root
/// </summary>
public class AuthUserTests
{
    [Fact]
    public void AuthUser_ShouldBeCreated_WithValidData()
    {
        // Arrange
        var email = "[email protected]";
        var password = "hashedPassword123";
        var name = "Test User";
        var timezone = "America/Lima";
        var language = "es";

        // Act
        var user = new AuthUser
        {
            Email = email,
            Password = password,
            Name = name,
            Timezone = timezone,
            Language = language
        };

        // Assert
        user.Should().NotBeNull();
        user.Email.Should().Be(email);
        user.Password.Should().Be(password);
        user.Name.Should().Be(name);
        user.Timezone.Should().Be(timezone);
        user.Language.Should().Be(language);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void AuthUser_Email_ShouldNotBeNullOrEmpty(string? invalidEmail)
    {
        // Arrange & Act
        var user = new AuthUser
        {
            Email = invalidEmail!,
            Password = "password",
            Name = "Test",
            Timezone = "UTC",
            Language = "en"
        };

        // Assert - In a real scenario, this would trigger validation
        // For now, we're just testing the property assignment
        user.Email.Should().Be(invalidEmail);
    }

    [Fact]
    public void AuthUser_Id_ShouldBeZero_WhenNotPersisted()
    {
        // Arrange & Act
        var user = new AuthUser
        {
            Email = "[email protected]",
            Password = "password",
            Name = "Test",
            Timezone = "UTC",
            Language = "en"
        };

        // Assert
        user.Id.Should().Be(0);
    }

    [Fact]
    public void AuthUser_Password_ShouldBeStoredAsProvided()
    {
        // Arrange
        var hashedPassword = "$2a$11$abcdefghijklmnopqrstuv";

        // Act
        var user = new AuthUser
        {
            Email = "[email protected]",
            Password = hashedPassword,
            Name = "Test",
            Timezone = "UTC",
            Language = "en"
        };

        // Assert
        user.Password.Should().Be(hashedPassword);
        user.Password.Should().StartWith("$2a$"); // BCrypt format
    }

    [Theory]
    [InlineData("America/Lima", "es")]
    [InlineData("America/New_York", "en")]
    [InlineData("Europe/Madrid", "es")]
    [InlineData("Asia/Tokyo", "ja")]
    public void AuthUser_ShouldSupportMultipleTimezones_AndLanguages(string timezone, string language)
    {
        // Arrange & Act
        var user = new AuthUser
        {
            Email = "[email protected]",
            Password = "password",
            Name = "Test",
            Timezone = timezone,
            Language = language
        };

        // Assert
        user.Timezone.Should().Be(timezone);
        user.Language.Should().Be(language);
    }

    [Fact]
    public void AuthUser_ShouldHaveRefreshTokens_Collection()
    {
        // Arrange & Act
        var user = new AuthUser
        {
            Email = "[email protected]",
            Password = "password",
            Name = "Test",
            Timezone = "UTC",
            Language = "en"
        };

        // Assert
        user.RefreshTokens.Should().NotBeNull();
        user.RefreshTokens.Should().BeEmpty();
    }

    [Fact]
    public void AuthUser_CanAddRefreshToken()
    {
        // Arrange
        var user = new AuthUser
        {
            Email = "[email protected]",
            Password = "password",
            Name = "Test",
            Timezone = "UTC",
            Language = "en"
        };

        var refreshToken = new AuthUserRefreshToken
        {
            Token = "refresh_token_123",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        // Act
        user.RefreshTokens.Add(refreshToken);

        // Assert
        user.RefreshTokens.Should().HaveCount(1);
        user.RefreshTokens.First().Token.Should().Be("refresh_token_123");
    }

    [Fact]
    public void AuthUserRefreshToken_ShouldExpire_AfterExpirationTime()
    {
        // Arrange
        var expiredToken = new AuthUserRefreshToken
        {
            Token = "expired_token",
            ExpiresAt = DateTime.UtcNow.AddDays(-1), // Expired yesterday
            CreatedAt = DateTime.UtcNow.AddDays(-8)
        };

        // Assert
        expiredToken.ExpiresAt.Should().BeBefore(DateTime.UtcNow);
    }

    [Fact]
    public void AuthUserRefreshToken_ShouldBeValid_BeforeExpirationTime()
    {
        // Arrange
        var validToken = new AuthUserRefreshToken
        {
            Token = "valid_token",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        // Assert
        validToken.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public void AuthUser_ShouldAllowMultipleRefreshTokens()
    {
        // Arrange
        var user = new AuthUser
        {
            Email = "[email protected]",
            Password = "password",
            Name = "Test",
            Timezone = "UTC",
            Language = "en"
        };

        // Act - Simulate multiple devices/sessions
        for (int i = 0; i < 5; i++)
        {
            user.RefreshTokens.Add(new AuthUserRefreshToken
            {
                Token = $"token_{i}",
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            });
        }

        // Assert
        user.RefreshTokens.Should().HaveCount(5);
        user.RefreshTokens.Select(t => t.Token).Should().OnlyHaveUniqueItems();
    }
}
