namespace SurveyBasketV5.Authentication
{
    public interface IJwtProvider
    {
        (string token,int expiresIn) GenerateToken(ApplicationUser user);
    }
}
