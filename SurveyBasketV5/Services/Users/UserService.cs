using SurveyBasketV5.Contracts.Users;
using SurveyBasketV5.Services.Roles;

namespace SurveyBasketV5.Services.Users
{
    public class UserService(UserManager<ApplicationUser> userManager,
                ApplicationDbContext dbContext,
                IRoleService roleService) : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly IRoleService _roleService = roleService;

        public async Task<IEnumerable<UserResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await (from u in _dbContext.Users
                         join ur in _dbContext.UserRoles
                         on u.Id equals ur.UserId
                         join r in _dbContext.Roles
                         on ur.RoleId equals r.Id into roles
                         where !roles.Any(r => r.Name == DefaultRoles.MemberRoleName) 
                         select new
                         {
                             u.Id,
                             u.FirstName,
                             u.LastName,
                             u.Email,
                             u.IsDisabled,
                             Roles = roles.Select(x => x.Name!)
                         })
                         .GroupBy(u => new {u.Id,u.FirstName,u.LastName,u.Email,u.IsDisabled})
                         .Select(u => new UserResponse(
                            u.Key.Id,
                            u.Key.FirstName,
                            u.Key.LastName,
                            u.Key.Email,
                            u.Key.IsDisabled,
                            u.SelectMany(x => x.Roles)
                         ))
                         .ToListAsync(cancellationToken);
        }

        public async Task<Result<UserResponse>> GetAsync(string id)
        {
            //**can use the way used in GetAllAsync in one DB Hit**

            if (await _userManager.FindByIdAsync(id) is not { } user)
                return Result.Failure<UserResponse>(UserErrors.UserNotFound);

            var roles = await _userManager.GetRolesAsync(user);

            //mapping from two sources but you need to configure it in mapping configuration
            var response = (user,roles).Adapt<UserResponse>();

            return Result.Success(response);
        }

        public async Task<Result<UserResponse>> AddAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
        {
            var isEmailExists = await _userManager.Users
                        .AnyAsync(x => x.Email == request.Email, cancellationToken);
            if(isEmailExists)
                return Result.Failure<UserResponse>(UserErrors.UserDublicatedEmail);

            //check roles
            var allowedRoles = await _roleService.GetAllAsync(false,cancellationToken);

            if(request.Roles.Except(allowedRoles.Select(x => x.Name)).Any())
                return Result.Failure<UserResponse>(UserErrors.InvalidRoles);

            var user = request.Adapt<ApplicationUser>();
            var result = await _userManager.CreateAsync(user,request.Password);
            if(!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure<UserResponse>(new Error(
                    error.Code, error.Description, StatusCodes.Status400BadRequest    
                ));
            }

            await _userManager.AddToRolesAsync(user, request.Roles);

            var response = (user,request.Roles).Adapt<UserResponse>();
            return Result.Success(response);
        }

        public async Task<Result> UpdateAsync(string id, UpdateUserRequest request, CancellationToken cancellationToken = default)
        {
            if (await _userManager.FindByIdAsync(id) is not { } user)
                return Result.Failure(UserErrors.UserNotFound);

            var isEmailExists = await _userManager.Users
                        .AnyAsync(x => x.Id != id && x.Email == request.Email, cancellationToken);
            if (isEmailExists)
                return Result.Failure(UserErrors.UserDublicatedEmail);

            //check roles
            var allowedRoles = await _roleService.GetAllAsync(false, cancellationToken);
            if (request.Roles.Except(allowedRoles.Select(x => x.Name)).Any())
                return Result.Failure(UserErrors.InvalidRoles);

            user = request.Adapt(user);
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure(new Error(
                    error.Code, error.Description, StatusCodes.Status400BadRequest
                ));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var newRoles = request.Roles.Except(roles);
            await _userManager.AddToRolesAsync(user, newRoles);

            var removedRoles = roles.Except(request.Roles);
            await _userManager.RemoveFromRolesAsync(user, removedRoles);

            return Result.Success();
        }

        public async Task<Result> ToggleStatusAsync(string id)
        {
            if (await _userManager.FindByIdAsync(id) is not { } user)
                return Result.Failure(UserErrors.UserNotFound);

            user.IsDisabled = !user.IsDisabled;
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure(new Error(
                    error.Code, error.Description, StatusCodes.Status400BadRequest
                ));
            }

            return Result.Success();
        }

        public async Task<Result> UnlockAsync(string id)
        {
            if (await _userManager.FindByIdAsync(id) is not { } user)
                return Result.Failure(UserErrors.UserNotFound);

            var result = await _userManager.SetLockoutEndDateAsync(user,null);
            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure(new Error(
                    error.Code, error.Description, StatusCodes.Status400BadRequest
                ));
            }

            return Result.Success();
        }

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
