using System.ComponentModel.DataAnnotations;

namespace SurveyBasketV5.Settings
{
    public class EmailSettings
    {
        [Required]
        public string SmtpServer { get; init; } = string.Empty;

        [Range(100, 999)]
        public int Port { get; set; }

        [Required, EmailAddress]
        public string SenderEmail { get; init; } = string.Empty;

        [Required]
        public string SenderName { get; init; } = string.Empty;

        [Required]
        public string Password { get; init; } = string.Empty;
    }
}
