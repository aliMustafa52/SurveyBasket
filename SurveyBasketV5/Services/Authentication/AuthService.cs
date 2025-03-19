using Hangfire;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using SurveyBasketV5.Authentication;
using SurveyBasketV5.Helpers;
using System.Security.Cryptography;
using System.Text;

namespace SurveyBasketV5.Services.Authentication
{
    public class AuthService(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtProvider jwtProvider,
        IEmailSender emailSender,
        IHttpContextAccessor httpContextAccessor,
        ApplicationDbContext dbContext) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly IJwtProvider _jwtProvider = jwtProvider;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly int _refreshTokenExpiryDays = 100;

        public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            if (await _userManager.FindByEmailAsync(email) is not { } user)
                return Result.Failure<AuthResponse>(UserErrors.UserInvalidCredentials);

            if (user.IsDisabled)
                return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

            var result = await _signInManager.PasswordSignInAsync(user, password, false, true);
            if (!result.Succeeded)
            {
                var error = result.IsNotAllowed
                    ? UserErrors.UserNotConfirmedEmail
                    : result.IsLockedOut
                    ? UserErrors.LockedOutUser
                    : UserErrors.UserInvalidCredentials;

                return Result.Failure<AuthResponse>(error);
            }

            // get roles and permissions
            var (roles, permissions) = await GetRolesAndPermissionsAsync(user, cancellationToken);

            // generate Token
            var (token, expiresIn) = _jwtProvider.GenerateToken(user, roles, permissions);

            // Generate Refresh Token
            var refreshTokne = GenerateRefreshToken();

            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshTokne,
                ExpiresOn = refreshTokenExpiration
            });

            await _userManager.UpdateAsync(user);

            return Result.Success(new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn * 60, refreshTokne, refreshTokenExpiration));
        }



        public async Task<Result<AuthResponse>> GetRefreshAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            var userId = _jwtProvider.ValidateToken(token);
            if (userId is null)
                return Result.Failure<AuthResponse>(UserErrors.UserInvalidAccessToken);

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Result.Failure<AuthResponse>(UserErrors.UserNotFound);

            if (user.IsDisabled)
                return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

            if (user.LockoutEnd > DateTime.UtcNow)
                return Result.Failure<AuthResponse>(UserErrors.LockedOutUser);

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);
            if (userRefreshToken is null)
                return Result.Failure<AuthResponse>(UserErrors.UserInvalidResreshToken);

            userRefreshToken.RevokedOn = DateTime.UtcNow;


            // get roles and permissions
            var (roles, permissions) = await GetRolesAndPermissionsAsync(user, cancellationToken);
            // generate new access Token
            var (newToken, expiresIn) = _jwtProvider.GenerateToken(user, roles, permissions);

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

        public async Task<Result> RegisterAsync(RegisterRequest registerRequest, CancellationToken cancellationToken = default)
        {
            //check if email exists 
            var isEmailExists = await _userManager.Users.AnyAsync(u => u.Email == registerRequest.Email, cancellationToken);
            if (isEmailExists)
                return Result.Failure(UserErrors.UserDublicatedEmail);

            var user = registerRequest.Adapt<ApplicationUser>();

            var result = await _userManager.CreateAsync(user, registerRequest.Password);
            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure(
                    new Error(error.Code,
                        error.Description
                        , StatusCodes.Status400BadRequest
                    )
                );
            }



            //generate verfication code
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            //send email
            // make this background job using hangfire

            await SendConfirmationCodeAsync(user, code);

            return Result.Success();

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
        public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
        {
            if (await _userManager.FindByIdAsync(request.UserId) is not { } user)
                return Result.Failure(UserErrors.UserNotFound);

            if (user.EmailConfirmed)
                return Result.Failure(UserErrors.DublicatedConfirmationCode);

            string token = request.Code;
            try
            {
                token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
            }
            catch (FormatException)
            {
                return Result.Failure(UserErrors.InvalidCode);
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure(
                    new Error(error.Code,
                        error.Description
                        , StatusCodes.Status400BadRequest
                    )
                );
            }

            await _userManager.AddToRoleAsync(user, DefaultRoles.Member.Name);
            return Result.Success();

        }
        public async Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request)
        {
            if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
                return Result.Success();

            if (user.EmailConfirmed)
                return Result.Failure(UserErrors.DublicatedConfirmationCode);

            //generate verfication code
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            //send email
            await SendConfirmationCodeAsync(user, code);

            return Result.Success();

        }

        public async Task<Result> SendResetPasswordCodeAsync(ResendConfirmationEmailRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return Result.Success();

            if (!user.EmailConfirmed)
                return Result.Failure(UserErrors.UserNotConfirmedEmail);

            //generate Password Reset Token
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            //send email
            await SendResetPasswordEmailAsync(user, code);

            return Result.Success();

        }

        public async Task<Result> ResetPasswordCodeAsync(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null || !user.EmailConfirmed)
                return Result.Failure(UserErrors.InvalidCode);

            //TODO
            // check if new password is the same as current password

            IdentityResult result;
            try
            {
                var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Code));
                result = await _userManager.ResetPasswordAsync(user, code, request.NewPassword);
            }
            catch (FormatException)
            {
                result = IdentityResult.Failed(_userManager.ErrorDescriber.InvalidToken());
            }

            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure(
                    new Error(error.Code,
                        error.Description
                        , StatusCodes.Status401Unauthorized
                    )
                );
            }

            return Result.Success();
        }

        private static string GenerateRefreshToken()
        {
            var number = RandomNumberGenerator.GetBytes(64);
            var token = Convert.ToBase64String(number);
            return token;
        }

        private async Task SendConfirmationCodeAsync(ApplicationUser user, string code)
        {
            var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

            var placeHolder = new Dictionary<string, string>
            {
                { "{{name}}", $"{user.FirstName} {user.LastName}" },
                { "{{action_url}}", $"{origin}/auth/confirm-email?UserId={user.Id}&Code={code}" }
            };

            var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation", placeHolder);

            BackgroundJob.Enqueue(() => _emailSender.SendEmailAsync(user.Email!, "Survey Basket: Email Confirmation", emailBody));
            await Task.CompletedTask;
        }
        private async Task SendResetPasswordEmailAsync(ApplicationUser user, string code)
        {
            var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

            var placeHolder = new Dictionary<string, string>
            {
                { "{{name}}", $"{user.FirstName} {user.LastName}" },
                { "{{action_url}}", $"{origin}/auth/forget-password?email={user.Email}&Code={code}" }
            };

            var emailBody = EmailBodyBuilder.GenerateEmailBody("ForgetPassword", placeHolder);
            await _emailSender.SendEmailAsync(user.Email!, "Survey Basket: Reset Password", emailBody);
        }

        private async Task<(IEnumerable<string> roles, IEnumerable<string> permissions)> GetRolesAndPermissionsAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            //get user Roles
            var userRoles = await _userManager.GetRolesAsync(user);

            //get user Permissions using Fluent Syntax (Method-Based)
            var userPermissions = await _dbContext.Roles
                .Join(_dbContext.RoleClaims,
                    role => role.Id,
                    roleClaim => roleClaim.RoleId,
                    (role, roleClaim) => new { role, roleClaim }
                )
                .Where(x => userRoles.Contains(x.role.Name!))
                .Select(x => x.roleClaim.ClaimValue)
                .Distinct()
                .ToListAsync(cancellationToken);

            //another way for join using Query Syntax (SQL-Like)
            var userPermissions2 = await (from r in _dbContext.Roles
                                          join rc in _dbContext.RoleClaims
                                          on r.Id equals rc.RoleId
                                          where userRoles.Contains(r.Name!)
                                          select rc.ClaimValue)
                                    .Distinct()
                                    .ToListAsync(cancellationToken);

            return (userRoles, userPermissions2!);
        }


    }
}
