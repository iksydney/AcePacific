using Microsoft.OpenApi.Models;

namespace AcePacific.API.ExtensionServices
{
    public static class SwaggerServiceExtension
    {
        public static IServiceCollection SwaggerExtension(this IServiceCollection services)
        {
            services.AddSwaggerGen(
                opt =>
                {
                    opt.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Version = "v1",
                        Title = "Ace Pacific",
                        Description = "Endpoints that handles all the implementations of the mini bank"
                    });
                });
            return services;
        }
        public static IApplicationBuilder SwaggerDocumentation(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
            });
            return app;
        }
    }
}
