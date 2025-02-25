namespace SurveyBasketV5.Errors
{
    public static class RoleErrors
    {
        public static readonly Error RoleNotFound =
            new("Role.NotFound", "No Role was found with the given ID", StatusCodes.Status404NotFound);
        public static readonly Error DublicatedRole =
            new("Role.DublicatedRole", "Another Role with the same name is already exists", StatusCodes.Status409Conflict);

        public static readonly Error InvalidPermissions =
            new("Role.InvalidPermissions", "Invalid Permissions", StatusCodes.Status400BadRequest);

        //public static readonly Error UserInvalidAccessToken =
        //    new("User.InvalidAccessToken", "Jwt Access Token is not valid", StatusCodes.Status401Unauthorized);

        //public static readonly Error UserInvalidResreshToken =
        //    new("User.InvalidResreshToken", "Resresh Token is not valid", StatusCodes.Status401Unauthorized);

        //public static readonly Error UserDublicatedEmail =
        //    new("User.DublicatedEmail", "This email is already exists", StatusCodes.Status409Conflict);

        //public static readonly Error UserNotConfirmedEmail =
        //    new("User.UserNotConfirmedEmail", "Email is not confirmed", StatusCodes.Status401Unauthorized);

        //public static readonly Error InvalidCode =
        //    new("User.InvalidCode", "Code is invalid", StatusCodes.Status401Unauthorized);



        //public static readonly Error UserInCorrectPassword =
        //    new("User.InCorrectPassword", "Current password is not correct", StatusCodes.Status400BadRequest);
    }
}
