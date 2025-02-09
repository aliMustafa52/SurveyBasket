using SurveyBasketV5.Authentication;
using SurveyBasketV5.Entities;
using System.Security.Cryptography;

namespace SurveyBasketV5.Services.Authentication
{
    public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IJwtProvider _jwtProvider = jwtProvider;

        private readonly int _refreshTokenExpiryDays = 100;

        public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return Result.Failure<AuthResponse>(UserErrors.UserInvalidCredentials);

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, password);
            if(!isPasswordCorrect)
                return Result.Failure<AuthResponse>(UserErrors.UserInvalidCredentials);

            // generate Token
            var (token, expiresIn) = _jwtProvider.GenerateToken(user);

            // Generate Refresh Token
            var refreshTokne = GenerateRefreshToken();

            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshTokne,
                ExpiresOn = refreshTokenExpiration
            });

            await _userManager.UpdateAsync(user);

            return Result.Success(new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn *60, refreshTokne, refreshTokenExpiration));
        }



        public async Task<Result<AuthResponse>> GetRefreshAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            var userId = _jwtProvider.ValidateToken(token);
            if(userId is null)
                return Result.Failure<AuthResponse>(UserErrors.UserInvalidAccessToken);

            var user = await _userManager.FindByIdAsync(userId);
            if(user is null)
                return Result.Failure<AuthResponse>(UserErrors.UserNotFound);

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);
            if(userRefreshToken is null)
                return Result.Failure<AuthResponse>(UserErrors.UserInvalidResreshToken);

            userRefreshToken.RevokedOn = DateTime.UtcNow;

            // generate new access Token
            var (newToken, expiresIn) = _jwtProvider.GenerateToken(user);

            // Generate new Refresh Token
            var newRefreshTokne = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshTokne,
                ExpiresOn = refreshTokenExpiration
            });

            await _userManager.UpdateAsync(user);

            return Result.Success(new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, newToken, expiresIn * 60, newRefreshTokne, refreshTokenExpiration));
        }

        private static string GenerateRefreshToken()
        {
            var number = RandomNumberGenerator.GetBytes(64);
            var token = Convert.ToBase64String(number);
            return token;
        }

        public async Task<Result> RevokeRefreshAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            var userId = _jwtProvider.ValidateToken(token);
            if (userId is null)
                return Result.Failure(UserErrors.UserInvalidAccessToken);

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Result.Failure(UserErrors.UserNotFound);

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);
            if (userRefreshToken is null)
                return Result.Failure(UserErrors.UserInvalidResreshToken);

            userRefreshToken.RevokedOn = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return Result.Success();
        }
    }
}
