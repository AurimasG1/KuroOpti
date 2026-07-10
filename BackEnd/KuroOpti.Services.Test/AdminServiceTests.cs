using KuroOpti.Common.DTO;
using KuroOpti.Entities;
using KuroOpti.Repositories.Interfaces;
using KuroOpti.Services.Implementations;
using Moq;

namespace KuroOpti.Services.Test;

public class AdminServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly AdminService _service;

    public AdminServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();

        _service = new AdminService(_userRepositoryMock.Object);
    }

    [Fact]
    public async Task GetAllUsersAsync_WhenUsersExist_ReturnsMappedUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new()
            {
                Id = 1,
                Email = "first@test.com",
                Role = "user",
            },
            new()
            {
                Id = 2,
                Email = "admin@test.com",
                Role = "admin",
            },
        };

        _userRepositoryMock.Setup(repository => repository.GetAllAsync()).ReturnsAsync(users);

        // Act
        var result = (await _service.GetAllUsersAsync()).ToList();

        // Assert
        Assert.Equal(2, result.Count);

        Assert.Equal(1, result[0].Id);
        Assert.Equal("first@test.com", result[0].Email);
        Assert.Equal("user", result[0].Role);

        Assert.Equal(2, result[1].Id);
        Assert.Equal("admin@test.com", result[1].Email);
        Assert.Equal("admin", result[1].Role);

        _userRepositoryMock.Verify(repository => repository.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetTotalUsersAsync_ReturnsRepositoryCount()
    {
        // Arrange
        _userRepositoryMock.Setup(repository => repository.CountAsync()).ReturnsAsync(12);

        // Act
        var result = await _service.GetTotalUsersAsync();

        // Assert
        Assert.Equal(12, result);

        _userRepositoryMock.Verify(repository => repository.CountAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_WhenRequestIsValid_UpdatesNormalizedUser()
    {
        // Arrange
        const int userId = 10;

        var existingUser = new User
        {
            Id = userId,
            Email = "old@test.com",
            Role = "user",
        };

        var request = new UserDto { Email = "  NEW@TEST.COM  ", Role = "  ADMIN  " };

        _userRepositoryMock
            .Setup(repository => repository.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _userRepositoryMock
            .Setup(repository => repository.UpdateAsync(existingUser))
            .ReturnsAsync(true);

        // Act
        var result = await _service.UpdateUserAsync(userId, request);

        // Assert
        Assert.True(result);
        Assert.Equal("new@test.com", existingUser.Email);
        Assert.Equal("admin", existingUser.Role);

        _userRepositoryMock.Verify(
            repository =>
                repository.UpdateAsync(
                    It.Is<User>(user =>
                        user.Id == userId && user.Email == "new@test.com" && user.Role == "admin"
                    )
                ),
            Times.Once
        );
    }

    [Fact]
    public async Task UpdateUserAsync_WhenUserDoesNotExist_ReturnsFalse()
    {
        // Arrange
        const int userId = 404;

        _userRepositoryMock
            .Setup(repository => repository.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        var request = new UserDto { Email = "user@test.com", Role = "user" };

        // Act
        var result = await _service.UpdateUserAsync(userId, request);

        // Assert
        Assert.False(result);

        _userRepositoryMock.Verify(
            repository => repository.UpdateAsync(It.IsAny<User>()),
            Times.Never
        );
    }

    [Theory]
    [InlineData(null, "user")]
    [InlineData("", "user")]
    [InlineData(" ", "user")]
    [InlineData("user@test.com", null)]
    [InlineData("user@test.com", "")]
    [InlineData("user@test.com", "manager")]
    public async Task UpdateUserAsync_WhenInputIsInvalid_ReturnsFalse(string? email, string? role)
    {
        // Arrange
        const int userId = 10;

        var existingUser = new User
        {
            Id = userId,
            Email = "old@test.com",
            Role = "user",
        };

        _userRepositoryMock
            .Setup(repository => repository.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        var request = new UserDto { Email = email, Role = role };

        // Act
        var result = await _service.UpdateUserAsync(userId, request);

        // Assert
        Assert.False(result);

        Assert.Equal("old@test.com", existingUser.Email);
        Assert.Equal("user", existingUser.Role);

        _userRepositoryMock.Verify(
            repository => repository.UpdateAsync(It.IsAny<User>()),
            Times.Never
        );
    }

    [Fact]
    public async Task DeleteUserAsync_WhenUserExists_DeletesUser()
    {
        // Arrange
        const int userId = 10;

        var user = new User
        {
            Id = userId,
            Email = "user@test.com",
            Role = "user",
        };

        _userRepositoryMock.Setup(repository => repository.GetByIdAsync(userId)).ReturnsAsync(user);

        _userRepositoryMock.Setup(repository => repository.DeleteAsync(user)).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteUserAsync(userId);

        // Assert
        Assert.True(result);

        _userRepositoryMock.Verify(repository => repository.DeleteAsync(user), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_WhenUserDoesNotExist_ReturnsFalse()
    {
        // Arrange
        const int userId = 404;

        _userRepositoryMock
            .Setup(repository => repository.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _service.DeleteUserAsync(userId);

        // Assert
        Assert.False(result);

        _userRepositoryMock.Verify(
            repository => repository.DeleteAsync(It.IsAny<User>()),
            Times.Never
        );
    }
}
