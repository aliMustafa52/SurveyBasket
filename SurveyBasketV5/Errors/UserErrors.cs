namespace SurveyBasketV5.Errors
{
    public static class UserErrors
    {
        public static readonly Error UserNotFound =
            new("User.NotFound", "No User was found with the given ID", StatusCodes.Status404NotFound);

        public static readonly Error UserInvalidCredentials =
            new("User.InvalidCredentials", "Email or password is not correct", StatusCodes.Status409Conflict);

        public static readonly Error UserInvalidAccessToken =
            new("User.InvalidAccessToken", "Jwt Access Token is not valid", StatusCodes.Status400BadRequest);

        public static readonly Error UserInvalidResreshToken =
            new("User.InvalidResreshToken", "Resresh Token is not valid", StatusCodes.Status400BadRequest);
    }
}
