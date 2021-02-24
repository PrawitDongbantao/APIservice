using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using APIservice.Entity;


namespace APIservice.Installers
{
   
    public class DBInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CMPOSContext>(options =>
             options.UseSqlServer(configuration.GetConnectionString("ConnectionSQLServer")));
        }
    }
}