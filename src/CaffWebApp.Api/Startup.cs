using CaffWebApp.Api.Authentication;
using CaffWebApp.Api.Identity;
using CaffWebApp.Api.ProblemDetails;
using CaffWebApp.Api.StaticFile;
using CaffWebApp.Api.Swagger;
using CaffWebApp.BLL;
using CaffWebApp.DAL;
using CaffWebApp.DAL.Entites;
using Hellang.Middleware.ProblemDetails;
using Microsoft.EntityFrameworkCore;
using NSwag.AspNetCore;
using NSwag.Generation.Processors.Security;
using Serilog;

namespace CaffWebApp.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CaffDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")!));

            services.AddCaffBll(Configuration);

            services.AddCaffWebAppIdentity();
            services.AddCaffWebAppIdentityServer();
            services.AddCaffWebAppAuthentication(Configuration);

            services.AddRazorPages();
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddCaffWebAppSwagger(Configuration);
            services.AddCaffWebAppProblemDetails();
            services.AddHttpContextAccessor();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSerilogRequestLogging();

            if (env.IsDevelopment())
            {
                app.UseCaffWebAppSwagger();
            }

            app.UseProblemDetails();
            app.UseHttpsRedirection();
            app.UseCaffStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
