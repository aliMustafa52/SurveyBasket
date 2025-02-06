namespace SurveyBasketV5.Services.Polls
{
    public class PollService(ApplicationDbContext dbContext) : IPollService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<IEnumerable<PollResponse>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var pollResponses = await _dbContext.Polls
                    .Where(x => x.IsActive)
                    .AsNoTracking()
                    .ProjectToType<PollResponse>()
                    .ToListAsync(cancellationToken);

            return pollResponses;
        }

        public async Task<Result<PollResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            var poll = await _dbContext.Polls
                    .SingleOrDefaultAsync(x => x.Id == id && x.IsActive, cancellationToken);

            if (poll is null)
                return Result.Failure<PollResponse>(PollErrors.PollNotFound);

            return Result.Success(poll.Adapt<PollResponse>());
        }

        public async Task<Result<PollResponse>> AddAsync(PollRequest request, CancellationToken cancellationToken = default)
        {
            var isExistingTitle = await _dbContext.Polls
                .AnyAsync(x => x.Title == request.Title && x.IsActive, cancellationToken);
            if(isExistingTitle)
                return Result.Failure<PollResponse>(PollErrors.DuplicatedPollTitle);

            var poll = request.Adapt<Poll>();

            await _dbContext.Polls.AddAsync(poll, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success(poll.Adapt<PollResponse>());
        }

        public async Task<Result> UpdateAsync(int id, PollRequest request, CancellationToken cancellationToken = default)
        {
            var polls = await _dbContext.Polls
                .Where(x => x.IsActive && (x.Id == id || x.Title == request.Title))
                .ToListAsync(cancellationToken);

            var existingPoll = polls.SingleOrDefault(x => x.Id ==id);
            if (existingPoll is null)
                return Result.Failure<PollResponse>(PollErrors.PollNotFound);

            var isExistingTitle = polls
                        .Any(x => x.Title == request.Title && x.Id != id);
            if (isExistingTitle)
                return Result.Failure(PollErrors.DuplicatedPollTitle);

            existingPoll.Title = request.Title;
            existingPoll.Summary = request.Summary;
            existingPoll.StartsAt = request.StartsAt;
            existingPoll.EndsAt = request.EndsAt;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var poll = await _dbContext.Polls
                .SingleOrDefaultAsync(x => x.Id == id && x.IsActive, cancellationToken);
            if (poll is null)
                return Result.Failure<PollResponse>(PollErrors.PollNotFound);

            poll.IsActive = false;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> TogglePublishStatusAsync(int id, CancellationToken cancellationToken = default)
        {
            var poll = await _dbContext.Polls
               .SingleOrDefaultAsync(x => x.Id == id && x.IsActive, cancellationToken);
            if (poll is null)
                return Result.Failure<PollResponse>(PollErrors.PollNotFound);

            poll.IsPublished = !poll.IsPublished;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
