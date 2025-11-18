using FluentAssertions;
using Moq;
using plantita.Shared.Domain.Repositories;
using plantita.User.Application.Internal.CommandServices;
using plantita.User.Application.Internal.OutboundServices;
using plantita.User.Domain.Model.Aggregates;
using plantita.User.Domain.Model.Commands;
using plantita.User.Domain.Repositories;
using Xunit;

namespace plantita.Tests.Unit.User;

public class AuthUserCommandServiceTests
{
    private readonly Mock<IAuthUserRepository> _mockUserRepository;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<IHashingService> _mockHashingService;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly AuthUserCommandService _service;

    public AuthUserCommandServiceTests()
    {
        _mockUserRepository = new Mock<IAuthUserRepository>();
        _mockTokenService = new Mock<ITokenService>();
        _mockHashingService = new Mock<IHashingService>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();

        _service = new AuthUserCommandService(
            _mockUserRepository.Object,
            _mockTokenService.Object,
            _mockHashingService.Object,
            _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task Handle_SignIn_WithValidCredentials_ReturnsUserAndToken()
    {
        // Arrange
        var command = new SignInCommand("test@example.com", "password123");
        var passwordHash = "hashed_password";
        var token = "jwt_token_here";

        var user = new AuthUser(
            "test@example.com",
            passwordHash,
            "John",
            "Doe",
            "UTC",
            "en",
            DateTime.UtcNow,
            "User");

        _mockUserRepository
            .Setup(r => r.FindByEmailAsync(command.Email))
            .ReturnsAsync(user);

        _mockHashingService
            .Setup(h => h.VerifyPassword(command.Password, passwordHash))
            .Returns(true);

        _mockTokenService
            .Setup(t => t.GenerateToken(user))
            .Returns(token);

        // Act
        var result = await _service.Handle(command);

        // Assert
        result.authUser.Should().Be(user);
        result.token.Should().Be(token);

        _mockUserRepository.Verify(r => r.FindByEmailAsync(command.Email), Times.Once);
        _mockHashingService.Verify(h => h.VerifyPassword(command.Password, passwordHash), Times.Once);
        _mockTokenService.Verify(t => t.GenerateToken(user), Times.Once);
    }

    [Fact]
    public async Task Handle_SignIn_WithInvalidEmail_ThrowsException()
    {
        // Arrange
        var command = new SignInCommand("nonexistent@example.com", "password123");

        _mockUserRepository
            .Setup(r => r.FindByEmailAsync(command.Email))
            .ReturnsAsync((AuthUser?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => _service.Handle(command));

        exception.Message.Should().Be("Invalid username or password");
    }

    [Fact]
    public async Task Handle_SignIn_WithInvalidPassword_ThrowsException()
    {
        // Arrange
        var command = new SignInCommand("test@example.com", "wrongpassword");
        var passwordHash = "hashed_password";

        var user = new AuthUser(
            "test@example.com",
            passwordHash,
            "John",
            "Doe",
            "UTC",
            "en",
            DateTime.UtcNow,
            "User");

        _mockUserRepository
            .Setup(r => r.FindByEmailAsync(command.Email))
            .ReturnsAsync(user);

        _mockHashingService
            .Setup(h => h.VerifyPassword(command.Password, passwordHash))
            .Returns(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => _service.Handle(command));

        exception.Message.Should().Be("Invalid username or password");
        _mockTokenService.Verify(t => t.GenerateToken(It.IsAny<AuthUser>()), Times.Never);
    }

    [Fact]
    public async Task Handle_SignUp_WithValidData_CreatesUser()
    {
        // Arrange
        var command = new SignUpCommand(
            "newuser@example.com",
            "SecurePassword123",
            "Jane",
            "Smith",
            "America/New_York",
            "en",
            DateTime.UtcNow,
            "User");

        var hashedPassword = "hashed_SecurePassword123";

        _mockUserRepository
            .Setup(r => r.ExistsByEmail(command.Email))
            .Returns(false);

        _mockHashingService
            .Setup(h => h.HashPassword(command.Password))
            .Returns(hashedPassword);

        _mockUserRepository
            .Setup(r => r.AddAsync(It.IsAny<AuthUser>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(u => u.CompleteAsync())
            .Returns(Task.CompletedTask);

        // Act
        await _service.Handle(command);

        // Assert
        _mockUserRepository.Verify(r => r.ExistsByEmail(command.Email), Times.Once);
        _mockHashingService.Verify(h => h.HashPassword(command.Password), Times.Once);
        _mockUserRepository.Verify(r => r.AddAsync(It.Is<AuthUser>(
            u => u.Email == command.Email && u.Name == "Jane" && u.LastName == "Smith")), Times.Once);
        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_SignUp_WithExistingEmail_ThrowsException()
    {
        // Arrange
        var command = new SignUpCommand(
            "existing@example.com",
            "password",
            "Test",
            "User",
            "UTC",
            "en",
            DateTime.UtcNow,
            "User");

        _mockUserRepository
            .Setup(r => r.ExistsByEmail(command.Email))
            .Returns(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => _service.Handle(command));

        exception.Message.Should().Be("Email existing@example.com is already taken");

        _mockHashingService.Verify(h => h.HashPassword(It.IsAny<string>()), Times.Never);
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<AuthUser>()), Times.Never);
    }

    [Fact]
    public async Task Handle_SignUp_WithDatabaseError_ThrowsException()
    {
        // Arrange
        var command = new SignUpCommand(
            "test@example.com",
            "password",
            "Test",
            "User",
            "UTC",
            "en",
            DateTime.UtcNow,
            "User");

        var hashedPassword = "hashed_password";

        _mockUserRepository
            .Setup(r => r.ExistsByEmail(command.Email))
            .Returns(false);

        _mockHashingService
            .Setup(h => h.HashPassword(command.Password))
            .Returns(hashedPassword);

        _mockUserRepository
            .Setup(r => r.AddAsync(It.IsAny<AuthUser>()))
            .ThrowsAsync(new Exception("Database connection error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(
            () => _service.Handle(command));

        exception.Message.Should().Contain("An error occurred while creating user");
        exception.Message.Should().Contain("Database connection error");

        _mockUnitOfWork.Verify(u => u.CompleteAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_SignUp_WithDefaultRole_CreatesUserWithUserRole()
    {
        // Arrange
        var command = new SignUpCommand(
            "test@example.com",
            "password",
            "Test",
            "User",
            "UTC",
            "en",
            DateTime.UtcNow); // No role specified, should default to "User"

        _mockUserRepository
            .Setup(r => r.ExistsByEmail(command.Email))
            .Returns(false);

        _mockHashingService
            .Setup(h => h.HashPassword(command.Password))
            .Returns("hashed");

        _mockUserRepository
            .Setup(r => r.AddAsync(It.IsAny<AuthUser>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(u => u.CompleteAsync())
            .Returns(Task.CompletedTask);

        // Act
        await _service.Handle(command);

        // Assert
        _mockUserRepository.Verify(r => r.AddAsync(It.Is<AuthUser>(
            u => u.Role == "User")), Times.Once);
    }

    [Fact]
    public async Task Handle_SignUp_VerifiesPasswordIsHashed()
    {
        // Arrange
        var plainPassword = "MySecretPassword123";
        var command = new SignUpCommand(
            "test@example.com",
            plainPassword,
            "Test",
            "User",
            "UTC",
            "en",
            DateTime.UtcNow,
            "User");

        var hashedPassword = "hashed_MySecretPassword123";

        _mockUserRepository
            .Setup(r => r.ExistsByEmail(command.Email))
            .Returns(false);

        _mockHashingService
            .Setup(h => h.HashPassword(plainPassword))
            .Returns(hashedPassword);

        _mockUserRepository
            .Setup(r => r.AddAsync(It.IsAny<AuthUser>()))
            .Returns(Task.CompletedTask);

        _mockUnitOfWork
            .Setup(u => u.CompleteAsync())
            .Returns(Task.CompletedTask);

        // Act
        await _service.Handle(command);

        // Assert
        _mockHashingService.Verify(h => h.HashPassword(plainPassword), Times.Once);

        // Verify that the stored password is the hashed version, not the plain password
        _mockUserRepository.Verify(r => r.AddAsync(It.Is<AuthUser>(
            u => u.PasswordHash == hashedPassword)), Times.Once);
    }
}
