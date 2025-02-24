using SurveyBasketV5.Contracts.Users;

namespace SurveyBasketV5.Services.Users
{
    public interface IUserService
    {
        Task<Result<UserProfileResponse>> GetUserProfileAsync(string userId, CancellationToken cancellationToken = default);
        Task<Result> UpdateUserProfileAsync(string userId, UpdateProfileRequest request);
        Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request);
    }
}
