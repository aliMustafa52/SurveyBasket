using FluentValidation;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using SurveyBasketV5.Authentication;
using SurveyBasketV5.Services.Authentication;
using SurveyBasketV5.Services.Emails;
using SurveyBasketV5.Services.Polls;
using SurveyBasketV5.Services.Questions;
using SurveyBasketV5.Services.Results;
using SurveyBasketV5.Services.Roles;
using SurveyBasketV5.Services.Users;
using SurveyBasketV5.Services.Votes;
using SurveyBasketV5.Settings;
using System.Text;

namespace SurveyBasketV5
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            services.AddOpenApi();

            services
                .AddDbContextConfig(configuration)
                .AddMapsterConfig()
                .AddFluentValidationConfig()
                .AddAuthConfig(configuration)
                .AddCorsConfig();

            services.AddScoped<IPollService, PollService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IVoteService, VoteService>();
            services.AddScoped<IResultService, ResultService>();
            services.AddScoped<IEmailSender, EmailService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            services.AddHttpContextAccessor();

            services.Configure<EmailSettings>(configuration.GetSection(nameof(EmailSettings)));

            return services;
        }

        private static IServiceCollection AddDbContextConfig(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("connectionString name DefaultConnection is not found");

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            return services;
        }

        private static IServiceCollection AddMapsterConfig(this IServiceCollection services)
        {
            var mappingConfig = TypeAdapterConfig.GlobalSettings;
            mappingConfig.Scan(Assembly.GetExecutingAssembly());

            services.AddSingleton<IMapper>(new Mapper(mappingConfig));

            return services;
        }
        private static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
        {
            services
                .AddFluentValidationAutoValidation()
                .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }

        private static IServiceCollection AddCorsConfig(this IServiceCollection services)
        {
            services.AddCors(options =>
                options.AddDefaultPolicy(builder => builder
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .AllowAnyOrigin()
                )
            );

            return services;
        }

        private static IServiceCollection AddAuthConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //custom permission-based authorization
            services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

            services.AddSingleton<IJwtProvider, JwtProvider>();

            var jwtSetting = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting?.Key!)),
                        ValidIssuer = jwtSetting?.Issuer,
                        ValidAudience = jwtSetting?.Audience
                    };
                });

            //services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

            services.AddOptions<JwtOptions>()
                .BindConfiguration(JwtOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            });

            return services;
        }
    }
}
