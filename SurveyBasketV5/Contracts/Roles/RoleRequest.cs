namespace SurveyBasketV5.Contracts.Roles
{
    public record RoleRequest
    (
        string Name,
        IEnumerable<string> Permissions
    );
}
