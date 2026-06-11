using System.Threading.Tasks;
using GymManagement.BLL.DTOs.Auth;
using GymManagement.BLL.Exceptions;
using GymManagement.BLL.Services;
using GymManagement.DAL.Entities;
using GymManagement.DAL.Entities.Enums;
using GymManagement.DAL.Services;
using GymManagement.DAL.UnitOfWork;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace GymManagement.Tests.Services
{
    public class AuthServiceTests
    {
        // Mocks
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        // Services 
        private readonly AuthService _authService;

        // Constructor 
        public AuthServiceTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _configuration = Substitute.For<IConfiguration>();

            _authService = new AuthService(
                _unitOfWork,
                _configuration
            );
        }

        // Login
        [Fact]
        public async Task Login_WithValidCredentioals_ReturnsLoginResponse()
        {
            // Arrange:
            var user = new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = UserRole.Admin
            };

            _configuration["JwtSettings:SecretKey"]
                .Returns("your-super-secret-key-must-be-at-least-32-characters");
            _configuration["JwtSettings:Issuer"]
                .Returns("GymManagement");
            _configuration["JwtSettings:Audience"]
                .Returns("GymManagementUsers");
            _configuration["JwtSettings:ExpiryMinutes"]
                .Returns("60");
            _configuration["JwtSettings:RefreshTokenExpiryDays"]
                .Returns("7");

            _unitOfWork.Users
                .GetByUsernameAsync("admin")
                .Returns(user);

            _unitOfWork.SaveChangesAsync()
                .Returns(1);

            // Act:
            var dto = new LoginRequestDto
            {
                Username = "admin",
                Password = "Admin@123"
            };

            var result = await _authService.Login(dto);

            // Assert:
            Assert.NotNull(result);
            Assert.Equal("admin", result.Username);
            Assert.NotEmpty(result.Token);
            Assert.NotEmpty(result.RefreshToken);
        }

        [Fact]
        public async Task Login_WithInvalidUsername_ThrowsUnauthorizedException()
        {
            // Arrange 
            _unitOfWork.Users
                .GetByUsernameAsync(Arg.Any<string>())
                .Returns((User?)null);

            var dto = new LoginRequestDto
            {
                Username = "wronguser",
                Password = "Admin@123"
            };

            // Act + Assert 
            await Assert.ThrowsAsync<UnauthorizedException>(
                () => _authService.Login(dto));
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ThrowsUnauthorizedException()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = UserRole.Admin
            };

            _unitOfWork.Users
                .GetByUsernameAsync(Arg.Any<string>())
                .Returns(user);

            var dto = new LoginRequestDto
            {
                Username = "admin",
                Password = "WrongPassword"
            };

            // Act + Assert
            await Assert.ThrowsAsync<UnauthorizedException>(
                () => _authService.Login(dto));
        }


        // Register
        [Fact]
        public async Task Register_WithValidCredentioals_ReturnsLoginResponse()
        {
            // Arrange

            // Setup configuration
            _configuration["JwtSettings:SecretKey"]
                .Returns("your-super-secret-key-must-be-at-least-32-characters");
            _configuration["JwtSettings:Issuer"]
                .Returns("GymManagement");
            _configuration["JwtSettings:Audience"]
                .Returns("GymManagementUsers");
            _configuration["JwtSettings:ExpiryMinutes"]
                .Returns("60");
            _configuration["JwtSettings:RefreshTokenExpiryDays"]
                .Returns("7");

            // No existing user
            _unitOfWork.Users
                .GetByUsernameAsync(Arg.Any<string>())
                .Returns((User?)null);

            // When Add is called set Id = 1
            _unitOfWork.Users
                .When(x => x.Add(Arg.Any<User>()))
                .Do(x => x.Arg<User>().Id = 1);

            _unitOfWork.SaveChangesAsync()
                .Returns(1);

            var dto = new RegisterRequestDto
            {
                Username = "admin",
                Password = "Admin@123",
                Role = UserRole.Admin
            };


            // Act 
            var result = await _authService.Register(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.NotEmpty(result.Token);
            Assert.NotEmpty(result.RefreshToken);
        }

        [Fact]
        public async Task Register_WithExistingUser_ThrowsValidationException()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = UserRole.Admin
            };

            _unitOfWork.Users
                .GetByUsernameAsync("admin")
                .Returns(user);


            var dto = new RegisterRequestDto
            {
                Username = "admin",
                Password = "Admin@123",
                Role = UserRole.Admin
            };

            // Act + Assert
            await Assert.ThrowsAsync<ValidationException>(() => _authService.Register(dto));
        }


        // Refresh
        [Fact]
        public async Task Refresh_WithValidCredentioals_ReturnsLoginResponse()
        {
            // Arrange
            _configuration["JwtSettings:SecretKey"]
                .Returns("your-super-secret-key-must-be-at-least-32-characters");
            _configuration["JwtSettings:Issuer"]
                .Returns("GymManagement");
            _configuration["JwtSettings:Audience"]
                .Returns("GymManagementUsers");
            _configuration["JwtSettings:ExpiryMinutes"]
                .Returns("60");
            _configuration["JwtSettings:RefreshTokenExpiryDays"]
                .Returns("7");

            var user = new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = UserRole.Admin,
            };

            var refreshToken = new RefreshToken
            {
                Id = 1,
                Token = "valid-refresh-token",
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                RevokedAt = null,
                UserId = 1,
                User = user
            };

            _unitOfWork.RefreshTokens
                .GetByTokenAsync("valid-refresh-token")
                .Returns(refreshToken);

            _unitOfWork.Users
                .GetByIdAsync(1)
                .Returns(user);

            _unitOfWork.SaveChangesAsync()
                .Returns(1);

            var dto = new RefreshTokenRequestDto
            {
                Token = "valid-refresh-token"
            };

            // Act 
            var result = await _authService.Refresh(dto);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Token);
            Assert.NotEmpty(result.RefreshToken);
        }

        [Fact]
        public async Task Refresh_WithNonExistentToken_ThrowsUnauthorizedException()
        {
            // Arrange
            _unitOfWork.RefreshTokens
                .GetByTokenAsync(Arg.Any<string>())
                .Returns((RefreshToken?)null);

            var dto = new RefreshTokenRequestDto
            {
                Token = "valid-refresh-token"
            };

            await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.Refresh(dto));
        }

        [Fact]
        public async Task Refresh_WithExpiredToken_ThrowsUnauthorizedException()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = UserRole.Admin,
            };

            var refreshToken = new RefreshToken
            {
                Id = 1,
                Token = "valid-refresh-token",
                ExpiresAt = DateTime.UtcNow.AddDays(-7),
                RevokedAt = null,
                UserId = 1,
                User = user
            };

            _unitOfWork.RefreshTokens
                .GetByTokenAsync("valid-refresh-token")
                .Returns(refreshToken);

            var dto = new RefreshTokenRequestDto
            {
                Token = "valid-refresh-token"
            };
            // Act + Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.Refresh(dto));
        }

        [Fact]
        public async Task Refresh_WithRevokedToken_ThrowsUnauthorizedException()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = UserRole.Admin,
            };

            var refreshToken = new RefreshToken
            {
                Id = 1,
                Token = "valid-refresh-token",
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                RevokedAt = DateTime.UtcNow.AddDays(-1),
                UserId = 1,
                User = user
            };

            _unitOfWork.RefreshTokens
                .GetByTokenAsync("valid-refresh-token")
                .Returns(refreshToken);

            var dto = new RefreshTokenRequestDto
            {
                Token = "valid-refresh-token"
            };
            // Act + Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.Refresh(dto));
        }


        // Revoke
        [Fact]
        public async Task Revoke_WithValidToken_RevokesSuccessfully()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = UserRole.Admin
            };

            var refreshToken = new RefreshToken
            {
                Id = 1,
                Token = "valid-refresh-token",
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                RevokedAt = null,
                UserId = 1,
                User = user
            };

            _unitOfWork.RefreshTokens
                .GetByTokenAsync("valid-refresh-token")
                .Returns(refreshToken);

            _unitOfWork.SaveChangesAsync()
                .Returns(1);

            var dto = new RevokeTokenRequestDto
            {
                RefreshToken = "valid-refresh-token"
            };

            // Act
            await _authService.Revoke(dto);

            // Assert
            await _unitOfWork.Received(1).SaveChangesAsync();
        }

        [Fact]
        public async Task Revoke_WithNonExistentToken_ThrowsUnauthorizedException()
        {
            // Arrange
            _unitOfWork.RefreshTokens
                .GetByTokenAsync(Arg.Any<string>())
                .Returns((RefreshToken?)null);

            var dto = new RevokeTokenRequestDto
            {
                RefreshToken = "Non-Existent-Token"
            };

            // Act + Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.Revoke(dto));
        }

        [Fact]
        public async Task Revoke_WithAlreadyRevokedToken_ThrowsValidationException()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = UserRole.Admin
            };

            var refreshToken = new RefreshToken
            {
                Id = 1,
                Token = "valid-refresh-token",
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                RevokedAt = DateTime.UtcNow.AddDays(-1),
                UserId = 1,
                User = user
            };

            _unitOfWork.RefreshTokens
                .GetByTokenAsync("valid-refresh-token")
                .Returns(refreshToken);

            var dto = new RevokeTokenRequestDto
            {
                RefreshToken = "valid-refresh-token"
            };

            // Act + Assert
            await Assert.ThrowsAsync<ValidationException>(() => _authService.Revoke(dto));
        }

    }
}