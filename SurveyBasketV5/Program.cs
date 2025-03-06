using Hangfire;
using HangfireBasicAuthenticationFilter;
using Scalar.AspNetCore;
using Serilog;

namespace SurveyBasketV5
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDependencies(builder.Configuration);

            // add Serilog but add configration in app setting
            builder.Host.UseSerilog((context, configration) =>
                configration.ReadFrom.Configuration(context.Configuration)
            );

            //builder.Services
            //    .AddIdentityApiEndpoints<ApplicationUser>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                //app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
                app.MapScalarApiReference();
            }

            //add http methods (requests)
            app.UseSerilogRequestLogging();

            app.UseHttpsRedirection();

            app.UseHangfireDashboard("/jobs", new DashboardOptions
            {
                Authorization =
                [
                    new HangfireCustomBasicAuthenticationFilter
                    {
                        User = app.Configuration.GetValue<string>("HangfireSettings:Username"),
                        Pass = app.Configuration.GetValue<string>("HangfireSettings:Password")
                    }
                ],
                DashboardTitle = "Survey Basket Dashboard"
            });

            app.UseCors();

            app.UseAuthorization();

            //app.MapIdentityApi<ApplicationUser>();

            app.MapControllers();

            //use Exception Handling Middleware after .net 8
            app.UseExceptionHandler();

            app.Run();
        }
    }
}
