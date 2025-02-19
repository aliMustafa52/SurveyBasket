using SurveyBasketV5.Contracts.Results.PollsRawData;
using SurveyBasketV5.Contracts.Results.VotesForAnswersPerQuestion;
using SurveyBasketV5.Contracts.Results.VotesPerDay;

namespace SurveyBasketV5.Services.Results
{
    public class ResultService(ApplicationDbContext dbContext) : IResultService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<Result<PollVotesResponse>> GetAllVotesForPollAsync(int pollId, CancellationToken cancellationToken = default)
        {
            var pollVotesResponse = await _dbContext.Polls
                                .Where(p => p.Id == pollId && p.IsActive)
                                .Select(p => new PollVotesResponse(
                                            p.Title,
                                            p.Votes.Select(v => new VoteResponse(
                                                $"{v.User.FirstName} {v.User.LastName}",
                                                v.SubmittedOn,
                                                v.VoteAnswers.Select(va => new QuestionAnswerResponse(
                                                  va.Question.Content,
                                                  va.Answer.Content
                                                ))
                                            ))
                                ))
                                .SingleOrDefaultAsync(cancellationToken);

            if (pollVotesResponse is null)
                return Result.Failure<PollVotesResponse>(PollErrors.PollNotFound);

            return Result.Success(pollVotesResponse);
        }

        public async Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesPerDayForPollAsync(int pollId, CancellationToken cancellationToken = default)
        {
            var isPollExists = await _dbContext.Polls
                                .AnyAsync(p => p.Id == pollId && p.IsActive, cancellationToken);
            if(!isPollExists)
                return Result.Failure<IEnumerable<VotesPerDayResponse>>(PollErrors.PollNotFound);

            var votesPerDay = await _dbContext.Votes
                                .Where(v => v.PollId == pollId)
                                .GroupBy(v => new {Date = DateOnly.FromDateTime(v.SubmittedOn) })
                                .Select(g => new VotesPerDayResponse(g.Key.Date,g.Count()))
                                .ToListAsync(cancellationToken);

            return Result.Success<IEnumerable<VotesPerDayResponse>>(votesPerDay);
        }

        public async Task<Result<IEnumerable<VotesPerQuestionResponse>>> GetVotesPerQuestionForPollAsync(int pollId, CancellationToken cancellationToken = default)
        {
            var isPollExists = await _dbContext.Polls
                                 .AnyAsync(p => p.Id == pollId && p.IsActive, cancellationToken);
            if (!isPollExists)
                return Result.Failure<IEnumerable<VotesPerQuestionResponse>>(PollErrors.PollNotFound);

            var votesPerQuestion = _dbContext.VoteAnswers
                    .Where(v => v.Vote.PollId == pollId)
                    .Select(va => new VotesPerQuestionResponse(
                        va.Question.Content,
                        va.Question.VoteAnswers
                            .GroupBy(x => new { AnswerId = x.Answer.Id, AnswerContent = x.Answer.Content })
                            .Select(g => new VotesPerAnswer(
                                    g.Key.AnswerContent,
                                    g.Count()
                            ))
                    ));

            return Result.Success<IEnumerable<VotesPerQuestionResponse>>(votesPerQuestion);
        }
    }
}
