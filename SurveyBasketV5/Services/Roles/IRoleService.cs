using SurveyBasketV5.Contracts.Roles;

namespace SurveyBasketV5.Services.Roles
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleResponse>> GetAllAsync(bool? includeDisabled = false, CancellationToken cancellationToken = default);

        Task<Result<RoleDetailResponse>> GetAsync(string roleId);

        Task<Result<RoleDetailResponse>> AddAsync(RoleRequest request);
        Task<Result> UpdateAsync(string id, RoleRequest request);
        Task<Result> ToggleStatusAsync(string id);
    }
}
