using System;
using Epam.ASPCore.Northwind.Domain.Models;
using Epam.ASPCore.Northwind.Domain.Repositories;
using Epam.ASPCore.Northwind.WebUI.Filters;
using Epam.ASPCore.Northwind.WebUI.Infrastructure;
using Epam.ASPCore.Northwind.WebUI.Middleware;
using Epam.ASPCore.Northwind.WebUI.Middleware.Options;
using Epam.ASPCore.Northwind.WebUI.Services;
using Epam.ASPCore.Northwind.WebUI.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web.TokenCacheProviders.InMemory;
using Microsoft.Identity.Web;
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

            services.AddDbContext<UserIdentityContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddDefaultIdentity<IdentityUser>()
                .AddEntityFrameworkStores<UserIdentityContext>();

            services.AddMicrosoftIdentityPlatformAuthentication(Configuration)
                .AddMsal(Configuration, new string[] { Constants.ScopeUserRead })
                .AddInMemoryTokenCaches();

            services.AddGraphService(Configuration);

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Identity/Account/Login";
                options.SlidingExpiration = true;
            });

            services.AddSwaggerDocument(config =>
            {
                config.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "Northwind API";
                    document.Info.Description = "A simple ASP.NET Core web API";
                    document.Info.Contact = new NSwag.OpenApiContact
                    {
                        Name = "Pavel Veselov",
                        Email = string.Empty,
                        Url = "https://www.linkedin.com/in/pavel-veselov-287683167/"
                    };
                };
            });

            services.Configure<ProductsSettings>(Configuration.GetSection("ProductsSettings"));
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));

            services.AddTransient(typeof(INorthwindRepository<>), typeof(NorthwindRepository<>));
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddMvc(options =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                    options.Filters.Add(new LoggActionFilter(true));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
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

            var path = env.ContentRootPath[0] + ":" + Configuration.GetSection("CacheImagePath").Value;
            app.UseMiddleware<RequestResponseImagesMiddleware>(new ImageOptions
            {
                Path = path,
                MaxCountItem = 10,
                ExpirationMinutes = 30
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseOpenApi();
            app.UseSwaggerUi3();

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
