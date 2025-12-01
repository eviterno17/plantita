using FluentAssertions;
using plantita.User.Infraestructure.Hashing.BCrypt.Services;
using Xunit;

namespace Plantita.CoreTests.Infrastructure.User;

/// <summary>
/// Unit tests for BCrypt password hashing service
/// </summary>
public class BCryptHashingServiceTests
{
    private readonly HashingService _hashingService;

    public BCryptHashingServiceTests()
    {
        _hashingService = new HashingService();
    }

    [Fact]
    public void HashPassword_ShouldReturnHashedPassword()
    {
        // Arrange
        var plainPassword = "MySecurePassword123!";

        // Act
        var hashedPassword = _hashingService.HashPassword(plainPassword);

        // Assert
        hashedPassword.Should().NotBeNullOrEmpty();
        hashedPassword.Should().NotBe(plainPassword);
        hashedPassword.Should().StartWith("$2a$"); // BCrypt hash format
    }

    [Fact]
    public void HashPassword_ShouldProduceDifferentHashes_ForSamePassword()
    {
        // Arrange
        var plainPassword = "SamePassword123!";

        // Act
        var hash1 = _hashingService.HashPassword(plainPassword);
        var hash2 = _hashingService.HashPassword(plainPassword);

        // Assert
        hash1.Should().NotBe(hash2); // BCrypt uses different salts
    }

    [Fact]
    public void VerifyPassword_ShouldReturnTrue_ForCorrectPassword()
    {
        // Arrange
        var plainPassword = "CorrectPassword123!";
        var hashedPassword = _hashingService.HashPassword(plainPassword);

        // Act
        var result = _hashingService.VerifyPassword(plainPassword, hashedPassword);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_ShouldReturnFalse_ForIncorrectPassword()
    {
        // Arrange
        var correctPassword = "CorrectPassword123!";
        var incorrectPassword = "WrongPassword456!";
        var hashedPassword = _hashingService.HashPassword(correctPassword);

        // Act
        var result = _hashingService.VerifyPassword(incorrectPassword, hashedPassword);

        // Assert
        result.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void HashPassword_ShouldHandleEmptyPassword(string emptyPassword)
    {
        // Act
        var hashedPassword = _hashingService.HashPassword(emptyPassword);

        // Assert
        hashedPassword.Should().NotBeNullOrEmpty();
        hashedPassword.Should().StartWith("$2a$");
    }

    [Theory]
    [InlineData("a")]
    [InlineData("ab")]
    [InlineData("abc")]
    public void HashPassword_ShouldHandleShortPasswords(string shortPassword)
    {
        // Act
        var hashedPassword = _hashingService.HashPassword(shortPassword);

        // Assert
        hashedPassword.Should().NotBeNullOrEmpty();
        hashedPassword.Should().StartWith("$2a$");
    }

    [Fact]
    public void HashPassword_ShouldHandleLongPasswords()
    {
        // Arrange
        var longPassword = new string('a', 1000); // 1000 characters

        // Act
        var hashedPassword = _hashingService.HashPassword(longPassword);

        // Assert
        hashedPassword.Should().NotBeNullOrEmpty();
        hashedPassword.Should().StartWith("$2a$");
    }

    [Theory]
    [InlineData("Password123!")]
    [InlineData("P@ssw0rd")]
    [InlineData("MySecure!Pass123")]
    [InlineData("Admin#2024$Pass")]
    public void HashAndVerify_ShouldWorkForVariousPasswords(string password)
    {
        // Act
        var hashedPassword = _hashingService.HashPassword(password);
        var isValid = _hashingService.VerifyPassword(password, hashedPassword);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_ShouldBeCaseSensitive()
    {
        // Arrange
        var originalPassword = "Password123";
        var hashedPassword = _hashingService.HashPassword(originalPassword);

        // Act
        var resultLower = _hashingService.VerifyPassword("password123", hashedPassword);
        var resultUpper = _hashingService.VerifyPassword("PASSWORD123", hashedPassword);
        var resultCorrect = _hashingService.VerifyPassword("Password123", hashedPassword);

        // Assert
        resultLower.Should().BeFalse();
        resultUpper.Should().BeFalse();
        resultCorrect.Should().BeTrue();
    }

    [Fact]
    public void HashPassword_ShouldHandleSpecialCharacters()
    {
        // Arrange
        var passwordWithSpecialChars = "P@$$w0rd!#%&*()";

        // Act
        var hashedPassword = _hashingService.HashPassword(passwordWithSpecialChars);
        var isValid = _hashingService.VerifyPassword(passwordWithSpecialChars, hashedPassword);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void HashPassword_ShouldHandleUnicodeCharacters()
    {
        // Arrange
        var passwordWithUnicode = "Contraseña123!";

        // Act
        var hashedPassword = _hashingService.HashPassword(passwordWithUnicode);
        var isValid = _hashingService.VerifyPassword(passwordWithUnicode, hashedPassword);

        // Assert
        isValid.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_ShouldReturnFalse_ForInvalidHash()
    {
        // Arrange
        var password = "TestPassword123";
        var invalidHash = "not_a_valid_bcrypt_hash";

        // Act & Assert
        Assert.Throws<BCrypt.Net.SaltParseException>(() =>
        {
            _hashingService.VerifyPassword(password, invalidHash);
        });
    }
}
