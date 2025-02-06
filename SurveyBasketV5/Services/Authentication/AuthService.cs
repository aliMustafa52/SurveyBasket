using SurveyBasketV5.Authentication;

namespace SurveyBasketV5.Services.Authentication
{
    public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IJwtProvider _jwtProvider = jwtProvider;

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


            return Result.Success(new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn *60));
        }
    }
}
