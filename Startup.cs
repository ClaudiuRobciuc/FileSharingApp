using ConsoleApplication.Models;
using ConsoleApplication.Models.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ConsoleApplication.Models.ViewModels;
using System.Diagnostics;
using ConsoleApplication.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApplication
{
    public class Startup
    {

        public void ConfigureServices(IServiceCollection services)
        { 
            services.AddDbContext<MyDbContext>();
            services.AddMemoryCache();
            services.AddSession();
            services.AddMvc();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IItemsRepository, ItemsRepository>();
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequiredLength = 4;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
            }).AddDefaultTokenProviders()
            .AddEntityFrameworkStores<MyDbContext>();
        }

       public void Configure(IApplicationBuilder app, ILoggerFactory logger, MyDbContext context) 
       {
            
            app.UseStaticFiles();
            app.UseIdentity();
            app.UseSession();
            logger.AddConsole();
            //app.UseMvcWithDefaultRoute();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                name: "default",
                template: "{controller=Home}/{action=Index}");
            });
            
            DbInitializer.Initialize(context);
            
            DbInitializer.SeedRolesUsers(app.ApplicationServices);
       }
    }
}