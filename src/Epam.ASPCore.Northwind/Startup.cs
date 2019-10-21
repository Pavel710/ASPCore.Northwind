using System;
using Epam.ASPCore.Northwind.Domain.Models;
using Epam.ASPCore.Northwind.Domain.Repositories;
using Epam.ASPCore.Northwind.WebUI.Middleware;
using Epam.ASPCore.Northwind.WebUI.Middleware.Options;
using Epam.ASPCore.Northwind.WebUI.Services;
using Epam.ASPCore.Northwind.WebUI.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Epam.ASPCore.Northwind.WebUI
{
    public class Startup
    {
        private readonly string _contentRootPath;

        public Startup(IConfiguration configuration,
            IHostingEnvironment env)
        {
            Configuration = configuration;
            _contentRootPath = env.ContentRootPath;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            if (connectionString.Contains("%CONTENTROOTPATH%"))
            {
                connectionString = connectionString.Replace("%CONTENTROOTPATH%", _contentRootPath);
            }
            services.AddDbContext<NorthwindContext>(options => 
                options.UseSqlServer(connectionString));

            services.Configure<ProductsSettings>(Configuration.GetSection("ProductsSettings"));

            services.AddTransient(typeof(INorthwindRepository<>), typeof(NorthwindRepository<>));
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ISupplierService, SupplierService>();
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Log.Information("Start " + env.ApplicationName + " Application!" + Environment.NewLine +
                            "Application location - " + env.ContentRootPath);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseMiddleware<RequestResponseImagesMiddleware>(new ImageOptions
            {
                Path = "D:\\CacheImages",
                MaxCountItem = 10,
                ExpirationMinutes = 1
            });
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute("images", "CategoryImages/{id}",
                    defaults: new { controller = "Categories", action = "GetImage" });
            });
        }
    }
}
