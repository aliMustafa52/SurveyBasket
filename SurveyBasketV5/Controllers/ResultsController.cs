namespace SurveyBasketV5.Controllers
{
    [Route("api/polls/{pollId}/[controller]")]
    [ApiController]
    public class ResultsController(IResultService resultService) : ControllerBase
    {
        private readonly IResultService _resultService = resultService;

        [HttpGet("row-data")]
        [HasPermission(Permissions.GetResults)]
        public async Task<IActionResult> GetAllVotesForPoll([FromRoute] int pollId, CancellationToken cancellationToken)
        {
            var result = await _resultService.GetAllVotesForPollAsync(pollId, cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpGet("votes-per-day")]
        [HasPermission(Permissions.GetResults)]
        public async Task<IActionResult> GetVotesPerDayForPoll([FromRoute] int pollId, CancellationToken cancellationToken)
        {
            var result = await _resultService.GetVotesPerDayForPollAsync(pollId, cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpGet("votes-per-question")]
        [HasPermission(Permissions.GetResults)]
        public async Task<IActionResult> GetVotesPerQuestionForPoll([FromRoute] int pollId, CancellationToken cancellationToken)
        {
            var result = await _resultService.GetVotesPerQuestionForPollAsync(pollId, cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }
    }
}
