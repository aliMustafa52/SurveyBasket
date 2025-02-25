namespace SurveyBasketV5.Authentication.Filters
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            //var user = context.User.Identity;

            //if (!user.IsAuthenticated)
            //    return;

            if (context.User.Identity is not { IsAuthenticated: true } user ||
                !context.User.Claims.Any(x => x.Value == requirement.Permission
                    && x.Type == Permissions.Type))
                return;


            context.Succeed(requirement);
            return;
        }
    }
}
