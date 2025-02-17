using Mapster;
using SurveyBasketV5.Contracts.Answers;
using SurveyBasketV5.Contracts.Questions;

namespace SurveyBasketV5.Services.Questions
{
    public class QuestionService(ApplicationDbContext dbContext) : IQuestionService
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task<Result<IEnumerable<QuestionResponse>>> GetAllAsync(int pollId, CancellationToken cancellationToken)
        {
            var isPollExists = await _dbContext.Polls
                                .AnyAsync(x => x.Id == pollId && x.IsActive, cancellationToken);
            if (!isPollExists)
                return Result.Failure<IEnumerable<QuestionResponse>>(PollErrors.PollNotFound);

            var questionResponses = await _dbContext.Questions
                    .Where(x => x.IsActive && x.PollId == pollId)
                    .Select(q => new QuestionResponse(
                        q.Id,
                        q.Content,
                        q.Answers
                            .Where(x => x.IsActive)
                            .Select(a=>new AnswerResponse(a.Id, a.Content))
                    ))
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

            return Result.Success<IEnumerable<QuestionResponse>>(questionResponses);
        }

        public async Task<Result<QuestionResponse>> GetAsync(int pollId,int id, CancellationToken cancellationToken)
        {
            var isPollExists = await _dbContext.Polls
                                .AnyAsync(x => x.Id == pollId && x.IsActive, cancellationToken);
            if (!isPollExists)
                return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

            var questionResponse = await _dbContext.Questions
                    .Where(x => x.Id == id && x.IsActive && x.PollId == pollId)
                    .Select(q => new QuestionResponse(
                        q.Id,
                        q.Content,
                        q.Answers
                            .Where(x => x.IsActive)
                            .Select(a => new AnswerResponse(a.Id, a.Content))
                    ))
                    .SingleOrDefaultAsync(cancellationToken);

            if (questionResponse is null)
                return Result.Failure<QuestionResponse>(QuestionErrors.QuestionNotFound);

            return Result.Success(questionResponse);
        }
        public async Task<Result<QuestionResponse>> AddAsync(int pollId, QuestionRequest request, CancellationToken cancellationToken = default)
        {
            var isPollExists = await _dbContext.Polls
                                .AnyAsync(x => x.Id == pollId && x.IsActive, cancellationToken);
            if(!isPollExists)
                return Result.Failure<QuestionResponse>(PollErrors.PollNotFound);

            var isQuestionExists = await _dbContext.Questions
                .AnyAsync(x => x.IsActive && x.Content == request.Content && x.PollId == pollId, cancellationToken);
            if (isQuestionExists)
                return Result.Failure<QuestionResponse>(QuestionErrors.DuplicatedQuestionContent);


            var question = request.Adapt<Question>();
            question.PollId = pollId;

            await _dbContext.Questions.AddAsync(question,cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success(question.Adapt<QuestionResponse>());
        }

        public async Task<Result> UpdateAsync(int pollId, int id, QuestionRequest request, CancellationToken cancellationToken = default)
        {
            var isPollExists = await _dbContext.Polls
                                .AnyAsync(x => x.Id == pollId && x.IsActive, cancellationToken);
            if (!isPollExists)
                return Result.Failure(PollErrors.PollNotFound);

            var question = await _dbContext.Questions
                                .Include(x => x.Answers.Where(x => x.IsActive))
                                .SingleOrDefaultAsync(x => x.Id == id && x.PollId == pollId && x.IsActive, cancellationToken);
            if (question is null)
                return Result.Failure(QuestionErrors.QuestionNotFound);

            var isQuestionContentExists = await _dbContext.Questions
                .AnyAsync(x => x.IsActive 
                        && x.Content == request.Content 
                        && x.PollId == pollId 
                        && x.Id != id
                        , cancellationToken
                );
            if (isQuestionContentExists)
                return Result.Failure(QuestionErrors.DuplicatedQuestionContent);

            question.Content = request.Content;

            //// get new answers and add them
            //var currentAnswers = question.Answers
            //    .Select(x => x.Content);
            //var newAnswers = request.Answers.Except(currentAnswers);
            //foreach (var newAnswer in newAnswers)
            //    question.Answers.Add(new Answer { Content = newAnswer });

            ////get answers that doen't exist in requst answers and delete them
            //foreach (var answer in question.Answers)
            //        answer.IsActive = request.Answers.Contains(answer.Content);


            // Convert answers to a dictionary for quick lookups
            var answerDict = question.Answers.ToDictionary(a => a.Content, a => a);

            foreach (var requestedAnswer in request.Answers)
            {
                if (answerDict.TryGetValue(requestedAnswer, out var existingAnswer))
                {
                    existingAnswer.IsActive = true; // Reactivate existing answer
                }
                else
                {
                    question.Answers.Add(new Answer { Content = requestedAnswer }); // Add new answer
                }
            }

            // Deactivate answers that are not in request
            foreach (var existingAnswer in question.Answers)
            {
                if (!request.Answers.Contains(existingAnswer.Content))
                    existingAnswer.IsActive = false;
            }


            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> ToggleStatusAsync(int pollId, int id, CancellationToken cancellationToken)
        {
            var isPollExists = await _dbContext.Polls
                                .AnyAsync(x => x.Id == pollId && x.IsActive, cancellationToken);
            if (!isPollExists)
                return Result.Failure(PollErrors.PollNotFound);

            var question = await _dbContext.Questions
                    .Where(x => x.Id == id && x.PollId == pollId)
                    .SingleOrDefaultAsync(cancellationToken);
            if (question is null)
                return Result.Failure(QuestionErrors.QuestionNotFound);

            question.IsActive = !question.IsActive;
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
