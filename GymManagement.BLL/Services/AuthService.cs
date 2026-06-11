
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GymManagement.BLL.DTOs.Auth;
using GymManagement.BLL.Exceptions;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.DAL.Entities;
using GymManagement.DAL.Services;
using GymManagement.DAL.UnitOfWork;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GymManagement.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }


        public async Task<LoginResponseDto> Login(LoginRequestDto dto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Get user by username
                var user = await _unitOfWork.Users.GetByUsernameAsync(dto.Username);

                // Validate user exists and password is correct
                if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                    throw new UnauthorizedException("اسم المستخدم أو كلمة المرور غير صحيحة");

                // Generate tokens
                var jwtToken = GenerateJwtToken(user);
                var refreshToken = new RefreshToken
                {
                    Token = GenerateRefreshToken(),
                    ExpiresAt = DateTime.UtcNow.AddDays(
                        int.Parse(_configuration["JwtSettings:RefreshTokenExpiryDays"]!)),
                    UserId = user.Id
                };

                // Save refresh token
                _unitOfWork.RefreshTokens.Add(refreshToken);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                // Return response
                return user.ToDto(jwtToken, refreshToken.Token);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<LoginResponseDto> Refresh(RefreshTokenRequestDto dto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Get refresh token from DB
                var refreshToken = await _unitOfWork.RefreshTokens
                    .GetByTokenAsync(dto.Token);

                // Validate refresh token
                if (refreshToken == null ||
                    refreshToken.ExpiresAt < DateTime.UtcNow ||
                    refreshToken.RevokedAt != null)
                    throw new UnauthorizedException("التوكن غير صحيح أو منتهي أو ملغى");

                // Get user
                var user = await _unitOfWork.Users.GetByIdAsync(refreshToken.UserId);
                if (user == null)
                    throw new NotFoundException("المستخدم غير موجود");

                // Generate new tokens
                var newJwtToken = GenerateJwtToken(user);
                var newRefreshToken = new RefreshToken
                {
                    Token = GenerateRefreshToken(),
                    ExpiresAt = DateTime.UtcNow.AddDays(
                        int.Parse(_configuration["JwtSettings:RefreshTokenExpiryDays"]!)),
                    UserId = user.Id
                };

                // Revoke old token and save new one
                refreshToken.RevokedAt = DateTime.UtcNow;
                _unitOfWork.RefreshTokens.Update(refreshToken);
                _unitOfWork.RefreshTokens.Add(newRefreshToken);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                // Return response
                return user.ToDto(newJwtToken, newRefreshToken.Token);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<LoginResponseDto> Register(RegisterRequestDto dto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Check if username already exists
                var existingUser = await _unitOfWork.Users.GetByUsernameAsync(dto.Username);
                if (existingUser != null)
                    throw new ValidationException("اسم المستخدم موجود مسبقاً");

                // Create new user
                var user = new User
                {
                    Username = dto.Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    Role = dto.Role
                };

                // Save user first to get Id
                _unitOfWork.Users.Add(user);
                await _unitOfWork.SaveChangesAsync();

                // Generate tokens
                var jwtToken = GenerateJwtToken(user);
                var refreshToken = new RefreshToken
                {
                    Token = GenerateRefreshToken(),
                    ExpiresAt = DateTime.UtcNow.AddDays(
                        int.Parse(_configuration["JwtSettings:RefreshTokenExpiryDays"]!)),
                    UserId = user.Id
                };

                // Save refresh token
                _unitOfWork.RefreshTokens.Add(refreshToken);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                // Return response
                return user.ToDto(jwtToken, refreshToken.Token);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task Revoke(RevokeTokenRequestDto dto)
        {
            // Get refresh token from DB
            var refreshToken = await _unitOfWork.RefreshTokens
                .GetByTokenAsync(dto.RefreshToken);

            // Validate refresh token
            if (refreshToken == null)
                throw new UnauthorizedException("التوكن غير صحيح");

            if (refreshToken.RevokedAt != null)
                throw new ValidationException("التوكن ملغى مسبقاً");

            // Revoke token
            refreshToken.RevokedAt = DateTime.UtcNow;
            _unitOfWork.RefreshTokens.Update(refreshToken);
            await _unitOfWork.SaveChangesAsync();
        }

        private string GenerateJwtToken(User user)
        {
            // Read JWT settings from appsettings
            var secretKey = _configuration["JwtSettings:SecretKey"]!;
            var issuer = _configuration["JwtSettings:Issuer"]!;
            var audience = _configuration["JwtSettings:Audience"]!;
            var expiryMinutes = int.Parse(_configuration["JwtSettings:ExpiryMinutes"]!);

            // Build claims
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.Username),
                new Claim(ClaimTypes.Role,user.Role.ToString()),
            };

            // Create signing key
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(secretKey));

            // Create credentials
            var credentials = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256);

            // Create token
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            // Generate cryptographically secure random token
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}