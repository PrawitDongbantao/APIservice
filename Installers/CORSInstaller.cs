using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace APIservice.Installers
{
    public class CORSInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
          {
              options.AddPolicy("AllowSpecificOrigins", builder => 
              {
                  builder.WithOrigins(
                      "http://localhost:80",
                      "http://example.com",
                      "http://localhost:4200",
                      "http://localhost:1152",
                      "http://192.168.99.100:1152",
                      "https://localhost:5001/"
                      )
                  //.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
                  //.WithMethods("GET", "POST", "HEAD");
              });
          });
        }
    }
}