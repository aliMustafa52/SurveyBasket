using SurveyBasketV5.Contracts.Users;
using SurveyBasketV5.Services.Users;
using System.Threading;

namespace SurveyBasketV5.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet("")]
        [HasPermission(Permissions.GetUsers)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var responses = await _userService.GetAllAsync(cancellationToken);

            return Ok(responses);
        }

        [HttpGet("{Id}")]
        [HasPermission(Permissions.GetUsers)]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            var result = await _userService.GetAsync(id);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpPost("")]
        [HasPermission(Permissions.AddUsers)]
        public async Task<IActionResult> Add([FromBody] CreateUserRequest request,CancellationToken cancellationToken)
        {
            var result = await _userService.AddAsync(request,cancellationToken);

            return result.IsSuccess
                ? CreatedAtAction(nameof(Get), new { id = result.Value.Id }, result.Value)
                : result.ToProblem();
        }

        [HttpPut("{Id}")]
        [HasPermission(Permissions.UpdateUsers)]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
        {
            var result = await _userService.UpdateAsync(id,request, cancellationToken);

            return result.IsSuccess
                ? NoContent()
                : result.ToProblem();
        }

        [HttpPut("{Id}/toggle-status")]
        [HasPermission(Permissions.UpdateUsers)]
        public async Task<IActionResult> ToggleStatus([FromRoute] string id)
        {
            var result = await _userService.ToggleStatusAsync(id);

            return result.IsSuccess
                ? NoContent()
                : result.ToProblem();
        }

        [HttpPut("{Id}/unlock")]
        [HasPermission(Permissions.UpdateUsers)]
        public async Task<IActionResult> Unlock([FromRoute] string id)
        {
            var result = await _userService.UnlockAsync(id);

            return result.IsSuccess
                ? NoContent()
                : result.ToProblem();
        }
    }
}
