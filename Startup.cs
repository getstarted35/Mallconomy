using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using LiderTablosuAPI.Controllers;

namespace LiderTablosuAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add MongoDBConnection service
            services.AddScoped<MongoDBConnection>(mongoDBConnection => new MongoDBConnection(
                Configuration.GetValue<string>("MongoDb:ConnectionString"),
                Configuration.GetValue<string>("MongoDb:veritabani")));

            // Add OdulHesaplayici service
            services.AddScoped<OdulHesaplayici>();
            services.AddScoped<LiderTablosuController>();

            // Add MVC service
            services.AddControllers();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Use MVC
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
