using SurveyBasketV5.Contracts.Votes;

namespace SurveyBasketV5.Services.Votes
{
    public interface IVoteService
    {
        Task<Result> AddAsync(int pollId, string userId, VoteRequest vote, CancellationToken cancellationToken = default);
    }
}
