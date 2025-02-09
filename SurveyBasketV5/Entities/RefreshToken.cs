namespace SurveyBasketV5.Entities
{
    [Owned]
    public class RefreshToken
    {
        public string Token { get; set; } = string.Empty;

        public DateTime ExpiresOn { get; set; }
        public DateTime CreatedOn { get; set; }

        public DateTime? RevokedOn { get; set; }

        public bool IsExpired => ExpiresOn <= DateTime.UtcNow;
        public bool IsActive => !IsExpired && RevokedOn is null;
    }
}
