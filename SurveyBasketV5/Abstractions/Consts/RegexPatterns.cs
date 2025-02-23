namespace SurveyBasketV5.Abstractions.Consts
{
    public static class RegexPatterns
    {
        public const string Password = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{8,}$";
    }
}
