using SurveyBasketV5.Contracts.Results.PollsRawData;
using SurveyBasketV5.Contracts.Results.VotesForAnswersPerQuestion;
using SurveyBasketV5.Contracts.Results.VotesPerDay;

namespace SurveyBasketV5.Services.Results
{
    public interface IResultService
    {
        Task<Result<PollVotesResponse>> GetAllVotesForPollAsync(int pollId, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<VotesPerDayResponse>>> GetVotesPerDayForPollAsync(int pollId, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<VotesPerQuestionResponse>>> GetVotesPerQuestionForPollAsync(int pollId, CancellationToken cancellationToken = default);
    }
}
