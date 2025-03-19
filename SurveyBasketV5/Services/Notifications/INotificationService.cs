namespace SurveyBasketV5.Services.Notifications
{
    public interface INotificationService
    {
        Task SendNewPollsNotification(int? pollId = null);
    }
}
