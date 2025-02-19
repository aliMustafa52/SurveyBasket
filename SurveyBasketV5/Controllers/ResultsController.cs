namespace SurveyBasketV5.Controllers
{
    [Route("api/polls/{pollId}/[controller]")]
    [ApiController]
    [Authorize]
    public class ResultsController(IResultService resultService) : ControllerBase
    {
        private readonly IResultService _resultService = resultService;

        [HttpGet("row-data")]
        public async Task<IActionResult> GetAllVotesForPoll([FromRoute] int pollId, CancellationToken cancellationToken)
        {
            var result =await _resultService.GetAllVotesForPollAsync(pollId, cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem(); 
        }

        [HttpGet("votes-per-day")]
        public async Task<IActionResult> GetVotesPerDayForPoll([FromRoute] int pollId, CancellationToken cancellationToken)
        {
            var result = await _resultService.GetVotesPerDayForPollAsync(pollId, cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpGet("votes-per-question")]
        public async Task<IActionResult> GetVotesPerQuestionForPoll([FromRoute] int pollId, CancellationToken cancellationToken)
        {
            var result = await _resultService.GetVotesPerQuestionForPollAsync(pollId, cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }
    }
}
