namespace SurveyBasketV5.Settings
{
    public class EmailSettings
    {
        public string SmtpServer { get; init; } = string.Empty;
        public int Port { get; set; }
        public string SenderEmail { get; init; } = string.Empty;
        public string SenderName { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }   
}
