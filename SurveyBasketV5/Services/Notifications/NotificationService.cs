
using SurveyBasketV5.Helpers;

namespace SurveyBasketV5.Services.Notifications
{
    public class NotificationService(ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor) : INotificationService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task SendNewPollsNotification(int? pollId = null)
        {
            IEnumerable<Poll> polls = [];

            if (pollId.HasValue)
            {
                var poll = await _dbContext.Polls
                    .SingleOrDefaultAsync(x => x.Id == pollId && x.IsPublished && x.IsActive);

                polls = [poll!];
            }
            else
            {
                polls = await _dbContext.Polls
                        .Where(x => x.IsActive
                            && x.IsPublished
                            && x.StartsAt == DateOnly.FromDateTime(DateTime.UtcNow))
                        .AsNoTracking()
                        .ToListAsync();
            }

            //TODO: Select members only
            var users = await _userManager.GetUsersInRoleAsync(DefaultRoles.Member.Name);

            var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;
            foreach (var poll in polls)
            {
                foreach (var user in users)
                {
                    var placeHolders = new Dictionary<string, string>
                    {
                        {"{{name}}", $"{user.FirstName} {user.LastName}" },
                        {"{{pollTill}}" , poll.Title},
                        {"{{endDate}}" , poll.EndsAt.ToString("yyyy-MM-dd")},
                        {"{{url}}" , $"{origin}/api/polls/{poll.Id}/vote"},
                    };
                    var emailBody = EmailBodyBuilder.GenerateEmailBody("PollNotification", placeHolders);

                }
            }
        }
    }
}
