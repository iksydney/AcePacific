using AcePacific.API.ExtensionServices;
using AcePacific.API.MappingConfigurations;
using AcePacific.Busines.Services;
using AcePacific.Data.DataAccess;
using AcePacific.Data.Repositories;
using AcePacific.Data.ViewModel;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

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
                opt.AddPolicy("CorsPolicy", opt =>
                {
                    opt.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
            });
            services.AddSingleton(config.GetSection("AppSettings").Get<AppSettings>());
            services.RegisterApplicationService<UserService>();
            services.RegisterLibraryServices<AppDbContext, UserRepository>();






            var app = builder.Build();

            if(app.Environment.IsDevelopment())
            {
                app.SwaggerDocumentation();
            }
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
    }
}