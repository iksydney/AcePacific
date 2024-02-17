using AcePacific.API.ExtensionServices;
using AcePacific.API.MappingConfigurations;
using AcePacific.Busines.Services;
using AcePacific.Data.DataAccess;
using AcePacific.Data.Repositories;
using AcePacific.Data.ViewModel;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Email;
using System.Net;

namespace AcePacific
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var services = builder.Services;
            var config = builder.Configuration;

            // Add services to the container.
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("logs/log.txt",
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true)
                .WriteTo.Email(new EmailConnectionInfo
                {
                    FromEmail = "acepacific@digital.com",
                    ToEmail = "cindi4talk@gmail.com",
                    MailServer = "smtp.gmail.com",
                    NetworkCredentials = new NetworkCredential
                    {
                        UserName = "cindi4talk@gmail.com",
                        Password = "kyzixWF16$$"
                    },
                    EnableSsl = true,
                    Port = 465,
                    EmailSubject = "Error in app"
                }, restrictedToMinimumLevel: LogEventLevel.Error, batchPostingLimit: 1)
                .CreateLogger();

            Log.Logger.Information("Total services: {count}", builder.Services.Count);
            try
            {
                services.AddControllers();
                services.AddEndpointsApiExplorer();

                var mappingConfiguration = new MapperConfiguration(
                    opt =>
                    {
                        opt.AddProfile(new MappingConfigurations());
                    });
                var mapper = mappingConfiguration.CreateMapper();
                services.AddSingleton(mapper);
                services.AddIdentityServices(config);

                var connectionString = config.GetConnectionString("DefaultConnection");
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseSqlServer(connectionString);
                }, ServiceLifetime.Scoped);
                services.AddSwaggerGen();
                services.SwaggerExtension();
                services.AddCors(opt =>
                {
                    opt.AddPolicy("CorsPolicy", builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                        //.AllowCredentials()
                        //.WithOrigins("http://localhost:3000", "https://ace-pacific.vercel.app");
                    });
                });
                services.AddSingleton(config.GetSection("AppSettings").Get<AppSettings>());
                services.AddSingleton(config.GetSection("MailSettings").Get<MailSettings>());
                services.RegisterApplicationService<UserService>();
                services.RegisterLibraryServices<AppDbContext, UserRepository>();
                
                services.AddHttpClient("MailTrapApiClient", (services, client) =>
                {
                    var mailSettings = services.GetRequiredService<IOptions<MailSettings>>().Value;
                    client.BaseAddress = new Uri(mailSettings.ApiBaseUrl);
                    client.DefaultRequestHeaders.Add("Api-Token", mailSettings.ApiToken);
                });

                var app = builder.Build();

                /*if (app.Environment.IsDevelopment())
                {
                    app.SwaggerDocumentation();
                }*/

                app.SwaggerDocumentation();
                // Configure the HTTP request pipeline.

                app.UseHttpsRedirection();
                app.UseRouting();
                app.UseStaticFiles();
                app.UseSwagger();
                app.UseCors("CorsPolicy");
                app.UseAuthentication();
                app.UseAuthorization();


                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Logger.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}