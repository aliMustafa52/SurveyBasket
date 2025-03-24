using SurveyBasketV5.Services.Authentication;

namespace SurveyBasketV5.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("")]
        public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);
            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.RegisterAsync(request, cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ToProblem();
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest request)
        {
            var result = await _authService.ConfirmEmailAsync(request);
            return result.IsSuccess
                ? Ok()
                : result.ToProblem();
        }

        [HttpPost("resend-confirmation-email")]
        public async Task<IActionResult> ResendConfirmationEmail(ResendConfirmationEmailRequest request)
        {
            var result = await _authService.ResendConfirmationEmailAsync(request);
            return result.IsSuccess
                ? Ok()
                : result.ToProblem();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.GetRefreshAsync(request.Token, request.RefreshToken, cancellationToken);
            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpPost("revoke-refresh-token")]
        public async Task<IActionResult> Revoke(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.RevokeRefreshAsync(request.Token, request.RefreshToken, cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ToProblem();
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ResendConfirmationEmailRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.SendResetPasswordCodeAsync(request);
            return result.IsSuccess
                ? Ok()
                : result.ToProblem();
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _authService.ResetPasswordCodeAsync(request);

            return result.IsSuccess
                    ? NoContent()
                    : result.ToProblem();
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            List<Course> COURSES = new List<Course>
            {
                new Course { Id = 1, Description = "angular core deep dive", IconUrl = "https://s3-us-west-1.amazonaws.com/angular-university/course-images/angular-core-in-depth-small.png", LongDescription = "A detailed walk-through of the most important part of Angular - the Core and Common modules", Category = "INTERMEDIATE", LessonsCount = 10 },
                new Course { Id = 2, Description = "RxJs In Practice Course", IconUrl = "https://s3-us-west-1.amazonaws.com/angular-university/course-images/rxjs-in-practice-course.png", LongDescription = "Understand the RxJs Observable pattern, learn the RxJs Operators via practical examples", Category = "BEGINNER", LessonsCount = 10 },
                new Course { Id = 3, Description = "NgRx In Depth", IconUrl = "https://s3-us-west-1.amazonaws.com/angular-university/course-images/angular-ngrx-course.png", LongDescription = "Learn the modern Ngrx Ecosystem, including Store, Effects, Router Store, Ngrx Entity, Dev Tools and Schematics.", Category = "ADVANCED" },
                new Course { Id = 4, Description = "Angular for Beginners", IconUrl = "https://angular-academy.s3.amazonaws.com/thumbnails/angular2-for-beginners-small-v2.png", LongDescription = "Establish a solid layer of fundamentals, learn what's under the hood of Angular", Category = "BEGINNER", LessonsCount = 10 },
                new Course { Id = 5, Description = "Angular Security Course", IconUrl = "https://s3-us-west-1.amazonaws.com/angular-university/course-images/security-cover-small-v2.png", LongDescription = "Learn Web Security Fundamentals and apply them to defend an Angular / Node Application from multiple types of attacks.", Category = "ADVANCED", LessonsCount = 11 },
                new Course { Id = 6, Description = "Angular PWA Course", IconUrl = "https://s3-us-west-1.amazonaws.com/angular-university/course-images/angular-pwa-course.png", LongDescription = "Learn Angular Progressive Web Applications, build the future of the Web Today.", Category = "ADVANCED", LessonsCount = 8 },
                new Course { Id = 7, Description = "Angular Advanced Course", IconUrl = "https://angular-academy.s3.amazonaws.com/thumbnails/advanced_angular-small-v3.png", LongDescription = "Learn Advanced Angular functionality typically used in Library Development. Advanced Components, Directives, Testing, Npm", Category = "ADVANCED" },
                new Course { Id = 8, Description = "Complete Typescript Course", IconUrl = "https://angular-academy.s3.amazonaws.com/thumbnails/typescript-2-small.png", LongDescription = "Complete Guide to Typescript From Scratch: Learn the language in-depth and use it to build a Node REST API.", Category = "BEGINNER" },
                new Course { Id = 9, Description = "Angular Architecture Course", IconUrl = "https://s3-us-west-1.amazonaws.com/angular-academy/blog/images/rxjs-reactive-patterns-small.png", LongDescription = "Learn the core RxJs Observable Pattern as well and many other Design Patterns for building Reactive Angular Applications.", Category = "BEGINNER" },
                new Course { Id = 10, Description = "Angular Material Course", IconUrl = "https://s3-us-west-1.amazonaws.com/angular-university/course-images/material_design.png", LongDescription = "Build Applications with the official Angular Widget Library" }
            };

            return Ok(COURSES);
        }

        [HttpPut("test")]
        public IActionResult Update([FromBody] Course course)
        {
            List<Course> COURSES = new List<Course>
            {
                new() { Id = 1, Description = "angular core deep dive", IconUrl = "https://s3-us-west-1.amazonaws.com/angular-university/course-images/angular-core-in-depth-small.png", LongDescription = "A detailed walk-through of the most important part of Angular - the Core and Common modules", Category = "INTERMEDIATE", LessonsCount = 10 },
                new Course { Id = 2, Description = "RxJs In Practice Course", IconUrl = "https://s3-us-west-1.amazonaws.com/angular-university/course-images/rxjs-in-practice-course.png", LongDescription = "Understand the RxJs Observable pattern, learn the RxJs Operators via practical examples", Category = "BEGINNER", LessonsCount = 10 },
                new Course { Id = 3, Description = "NgRx In Depth", IconUrl = "https://s3-us-west-1.amazonaws.com/angular-university/course-images/angular-ngrx-course.png", LongDescription = "Learn the modern Ngrx Ecosystem, including Store, Effects, Router Store, Ngrx Entity, Dev Tools and Schematics.", Category = "ADVANCED" },
                new Course { Id = 4, Description = "Angular for Beginners", IconUrl = "https://angular-academy.s3.amazonaws.com/thumbnails/angular2-for-beginners-small-v2.png", LongDescription = "Establish a solid layer of fundamentals, learn what's under the hood of Angular", Category = "BEGINNER", LessonsCount = 10 },
                new Course { Id = 5, Description = "Angular Security Course", IconUrl = "https://s3-us-west-1.amazonaws.com/angular-university/course-images/security-cover-small-v2.png", LongDescription = "Learn Web Security Fundamentals and apply them to defend an Angular / Node Application from multiple types of attacks.", Category = "ADVANCED", LessonsCount = 11 },
                new Course { Id = 6, Description = "Angular PWA Course", IconUrl = "https://s3-us-west-1.amazonaws.com/angular-university/course-images/angular-pwa-course.png", LongDescription = "Learn Angular Progressive Web Applications, build the future of the Web Today.", Category = "ADVANCED", LessonsCount = 8 },
                new Course { Id = 7, Description = "Angular Advanced Course", IconUrl = "https://angular-academy.s3.amazonaws.com/thumbnails/advanced_angular-small-v3.png", LongDescription = "Learn Advanced Angular functionality typically used in Library Development. Advanced Components, Directives, Testing, Npm", Category = "ADVANCED" },
                new Course { Id = 8, Description = "Complete Typescript Course", IconUrl = "https://angular-academy.s3.amazonaws.com/thumbnails/typescript-2-small.png", LongDescription = "Complete Guide to Typescript From Scratch: Learn the language in-depth and use it to build a Node REST API.", Category = "BEGINNER" },
                new Course { Id = 9, Description = "Angular Architecture Course", IconUrl = "https://s3-us-west-1.amazonaws.com/angular-academy/blog/images/rxjs-reactive-patterns-small.png", LongDescription = "Learn the core RxJs Observable Pattern as well and many other Design Patterns for building Reactive Angular Applications.", Category = "BEGINNER" },
                new Course { Id = 10, Description = "Angular Material Course", IconUrl = "https://s3-us-west-1.amazonaws.com/angular-university/course-images/material_design.png", LongDescription = "Build Applications with the official Angular Widget Library" }
            };

            COURSES[course.Id - 1] = course;

            return Ok(course);
        }

        public class Course
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public string IconUrl { get; set; }
            public string LongDescription { get; set; }
            public string Category { get; set; }
            public int? LessonsCount { get; set; } // Nullable to match missing values
        }
    }
}
