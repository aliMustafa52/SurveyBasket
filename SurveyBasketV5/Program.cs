using Scalar.AspNetCore;
using SurveyBasketV5.Middleware;

namespace SurveyBasketV5
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDependencies(builder.Configuration);

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

            app.UseHttpsRedirection();

            app.UseCors();

            app.UseAuthorization();

            //app.MapIdentityApi<ApplicationUser>();

            app.MapControllers();

            //use Exception Handling Middleware after .net 8
            app.UseExceptionHandler();

            //use Exception Handling Middleware before .net 8
            //app.UseMiddleware<ExceptionHandlingMiddleware>();

            app.Run();
        }
    }
}
