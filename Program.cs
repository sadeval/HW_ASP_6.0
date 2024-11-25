using Microsoft.EntityFrameworkCore;
using OnlineStore.Data;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Diagnostics;

namespace OnlineStore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            var logger = LoggerFactory.Create(loggingBuilder =>
            {
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            }).CreateLogger<Program>();

            logger.LogInformation("Строка подключения: {ConnectionString}", connectionString);

            builder.Services.AddControllersWithViews();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OnlineStore API", Version = "v1" });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "OnlineStore API V1");
                });
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // Регистрация маршрутов контроллеров
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // Глобальная обработка исключений
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (errorFeature != null)
                    {
                        var ex = errorFeature.Error;
                        var err = new { message = ex.Message };
                        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "Необработанное исключение.");

                        await context.Response.WriteAsJsonAsync(err);
                    }
                });
            });

            app.Run();
        }
    }
}
