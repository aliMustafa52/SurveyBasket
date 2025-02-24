using SurveyBasketV5.Contracts.Users;

namespace SurveyBasketV5.Services.Users
{
    public class UserService(UserManager<ApplicationUser> userManager,
                ApplicationDbContext dbContext) : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<Result<UserProfileResponse>> GetUserProfileAsync(string userId, CancellationToken cancellationToken = default)
        {
            //first way >> so bad
            //if(await _userManager.FindByIdAsync(userId) is not { } user1)
            //    return Result.Failure<UserProfileResponse>(UserErrors.UserNotFound);

            //var userProfileResponse = user.Adapt<UserProfileResponse>();


            //second way >> good
            //var userProfileResponse = await _dbContext.Users
            //            .Where(x => x.Id == userId)
            //            .Select(x => new UserProfileResponse(x.Email!,
            //                    x.UserName!,
            //                    x.FirstName,
            //                    x.LastName)
            //            )
            //            .FirstOrDefaultAsync(cancellationToken);
            //if(userProfileResponse is null)
            //    return Result.Failure<UserProfileResponse>(UserErrors.UserNotFound);


            //third way >>> good but it uses userManager instead of dbcontext
            var userProfileResponse = await _userManager.Users
                        .Where(x => x.Id == userId)
                        .Select(x => new UserProfileResponse(x.Email!,
                                x.UserName!,
                                x.FirstName,
                                x.LastName)
                        )
                        .SingleOrDefaultAsync(cancellationToken);
            if (userProfileResponse is null)
                return Result.Failure<UserProfileResponse>(UserErrors.UserNotFound);



            return Result.Success(userProfileResponse);
        }

        public async Task<Result> UpdateUserProfileAsync(string userId, UpdateProfileRequest request)
        {
            //var user = await _userManager.FindByIdAsync(userId);

            // no need for this
            //if (user is null)
            //    return Result.Failure(UserErrors.UserNotFound);

            //user = request.Adapt(user);

            //user.FirstName = request.FirstName;
            //user.LastName = request.LastName;

            //await _userManager.UpdateAsync(user);

            //no need for this
            //if (!result.Succeeded)
            //{
            //    var error = result.Errors.First();
            //    return Result.Failure<UserProfileResponse>(new Error(error.Code,
            //        error.Description,
            //        StatusCodes.Status400BadRequest)
            //    );
            //}

            var rowsAffected = await _userManager.Users
                    .Where(x => x.Id == userId)
                    .ExecuteUpdateAsync(setters => 
                        setters
                            .SetProperty(u => u.FirstName,request.FirstName)
                            .SetProperty(u => u.LastName,request.LastName)
                    );

            if (rowsAffected == 0)
                return Result.Failure(UserErrors.UserNotFound);

            return Result.Success();
        }

        public async Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return Result.Failure(UserErrors.UserNotFound);

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure(new Error(error.Code
                        ,error.Description
                        ,StatusCodes.Status400BadRequest)
                );
            }

            return Result.Success();
        }
    }
}
