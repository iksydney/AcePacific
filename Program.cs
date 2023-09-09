using AcePacific.API.ExtensionServices;

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
            services.AddSwaggerGen();
            services.SwaggerExtension();








            var app = builder.Build();

            if(app.Environment.IsDevelopment())
            {
                app.SwaggerDocumentation();
            }
            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseAuthorization();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}