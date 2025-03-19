using SurveyBasketV5.Contracts.Votes;

namespace SurveyBasketV5.Services.Votes
{
    public class VoteService(ApplicationDbContext dbContext) : IVoteService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<Result> AddAsync(int pollId, string userId, VoteRequest request, CancellationToken cancellationToken = default)
        {
            var isVotedBefore = await _dbContext.Votes
                            .AnyAsync(x => x.PollId == pollId
                                && x.UserId == userId, cancellationToken);
            if (isVotedBefore)
                return Result.Failure(VoteErrors.DuplicatedVote);

            var isPollExists = await _dbContext.Polls
                    .AnyAsync(x => x.Id == pollId
                            && x.IsActive
                            && x.IsPublished
                            && DateOnly.FromDateTime(DateTime.UtcNow) >= x.StartsAt
                            && DateOnly.FromDateTime(DateTime.UtcNow) <= x.EndsAt,
                            cancellationToken
                    );
            if (!isPollExists)
                return Result.Failure(PollErrors.PollNotFound);

            var questionIds = await _dbContext.Questions
                                .Where(q => q.IsActive && q.PollId == pollId)
                                .Select(x => x.Id)
                                .ToListAsync(cancellationToken);

            if (!request.VoteAnswers.All(x => questionIds.Contains(x.QuestionId)))
                return Result.Failure(VoteErrors.InvalidQuestions);

            var vote = new Vote
            {
                PollId = pollId,
                UserId = userId,
                VoteAnswers = request.VoteAnswers
                        .Adapt<IEnumerable<VoteAnswer>>()
                        .ToList()
            };

            await _dbContext.Votes.AddAsync(vote, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
