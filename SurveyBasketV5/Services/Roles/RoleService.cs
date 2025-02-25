using SurveyBasketV5.Abstractions.Consts;
using SurveyBasketV5.Contracts.Roles;

namespace SurveyBasketV5.Services.Roles
{
    public class RoleService(RoleManager<ApplicationRole> roleManager,ApplicationDbContext dbContext) : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<IEnumerable<RoleResponse>> GetAllAsync(bool? includeDisabled = false, CancellationToken cancellationToken = default)
        {
            var roleResponses = await  _roleManager.Roles
                    .Where(x => !x.IsDefault && (!x.IsDeleted || (includeDisabled.HasValue && includeDisabled.Value)))
                    .AsNoTracking()
                    .ProjectToType<RoleResponse>()
                    .ToListAsync(cancellationToken);

            return roleResponses;
        }

        public async Task<Result<RoleDetailResponse>> GetAsync(string roleId)
        {
            if(await _roleManager.FindByIdAsync(roleId) is not { } role)
                return Result.Failure<RoleDetailResponse>(RoleErrors.RoleNotFound);

            var permissions = await _roleManager.GetClaimsAsync(role);
            var response = new RoleDetailResponse(role.Id, role.Name,role.IsDeleted, permissions.Select(x => x.Value));

            return Result.Success(response);

        }

        public async Task<Result<RoleDetailResponse>> AddAsync(RoleRequest request)
        {
            var roleIsExists = await _roleManager.RoleExistsAsync(request.Name);
            if (roleIsExists)
                return Result.Failure<RoleDetailResponse>(RoleErrors.DublicatedRole);

            var allowedPermissions = Permissions.GetAllPermissions();
            if (request.Permissions.Except(allowedPermissions).Any())
                return Result.Failure<RoleDetailResponse>(RoleErrors.InvalidPermissions);

            var role = new ApplicationRole
            {
                Name = request.Name,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
            };

            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure<RoleDetailResponse>(new Error(
                    error.Code,
                    error.Description,
                    StatusCodes.Status400BadRequest
                ));
            }

            var permissions = request.Permissions
                .Select(x => new IdentityRoleClaim<string>
                {
                    ClaimValue = x,
                    ClaimType = Permissions.Type,
                    RoleId = role.Id
                });

            await _dbContext.RoleClaims.AddRangeAsync(permissions);
            await _dbContext.SaveChangesAsync();

            var roleDetailResponse = new RoleDetailResponse(role.Id,
                        role.Name,
                        role.IsDeleted,
                        request.Permissions);

            return Result.Success(roleDetailResponse);
        }

        public async Task<Result> UpdateAsync(string id, RoleRequest request)
        {
            if(await _roleManager.FindByIdAsync(id) is not { } role)
                return Result.Failure<RoleDetailResponse>(RoleErrors.RoleNotFound);

            var roleNameIsDuplicated = await _roleManager.Roles
                .AnyAsync(x => x.Id != id && x.Name == request.Name);
            if (roleNameIsDuplicated)
                return Result.Failure<RoleDetailResponse>(RoleErrors.DublicatedRole);

            var allowedPermissions = Permissions.GetAllPermissions();
            if (request.Permissions.Except(allowedPermissions).Any())
                return Result.Failure<RoleDetailResponse>(RoleErrors.InvalidPermissions);

            role.Name = request.Name;
            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure<RoleDetailResponse>(new Error(
                    error.Code,
                    error.Description,
                    StatusCodes.Status400BadRequest
                ));
            }

            var currentPermissions = await _dbContext.RoleClaims
                    .Where(rc => rc.RoleId == id && rc.ClaimType == Permissions.Type)
                    .Select(x => x.ClaimValue)
                    .ToListAsync();

            var newPermissions = request.Permissions
                .Except(currentPermissions)
                .Select(x => new IdentityRoleClaim<string>
                {
                    ClaimValue = x,
                    ClaimType = Permissions.Type,
                    RoleId = role.Id
                });
            
            var removedPermissions = currentPermissions
                    .Except(request.Permissions);

            await _dbContext.RoleClaims
                .Where(x => x.RoleId == id 
                    && removedPermissions.Contains(x.ClaimValue))
                .ExecuteDeleteAsync();

            await _dbContext.RoleClaims.AddRangeAsync(newPermissions);
            await _dbContext.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> ToggleStatusAsync(string id)
        {
            if (await _roleManager.FindByIdAsync(id) is not { } role)
                return Result.Failure<RoleDetailResponse>(RoleErrors.RoleNotFound);

            role.IsDeleted = !role.IsDeleted;
            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure<RoleDetailResponse>(new Error(
                    error.Code,
                    error.Description,
                    StatusCodes.Status400BadRequest
                ));
            }

            return Result.Success();
        }
    }
}
