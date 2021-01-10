using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QualityShopping.Business.Abstract;
using QualityShopping.Business.Concrete;
using QualityShopping.DataAccess.Abstract;
using QualityShopping.DataAccess.Concrete.EntityFramework;
using QualityShopping.WebUI.EmailServices;
using QualityShopping.WebUI.Identity;
using QualityShopping.WebUI.Middlewares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QualityShopping.WebUI
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppIdentityDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();

            #region Configure
            services.Configure<IdentityOptions>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireUppercase = true;

                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                    options.Lockout.AllowedForNewUsers = true;

                    options.User.RequireUniqueEmail = true;
                    options.SignIn.RequireConfirmedEmail = true;
                    options.SignIn.RequireConfirmedPhoneNumber = false;
                });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/account/login";
                options.LogoutPath = "/account/logout";
                options.AccessDeniedPath = "/account/accessdenied";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
                options.Cookie = new CookieBuilder
                {
                    HttpOnly = true,
                    Name = "QualityShopping.Security.Cookie",
                    SameSite = SameSiteMode.Strict
                };
            });

            #endregion

            #region Services

            services.AddMvc(option => option.EnableEndpointRouting = false);

            services.AddScoped<ICategoryRepository, EfCategoryDal>();
            services.AddScoped<ICategoryService, CategoryManager>();

            services.AddScoped<ICartRepository, EfCartDal>();
            services.AddScoped<ICartService, CartManager>();

            services.AddScoped<IOrderRepository, EfOrderDal>();
            services.AddScoped<IOrderService, OrderManager>();

            services.AddScoped<IProductRepository, EfProductDal>();
            services.AddScoped<IProductService, ProductManager>();

            services.AddTransient<IEmailSender, SmtpEmailSender>(i =>
            new SmtpEmailSender(
                Configuration["EmailSender:Host"],
                Configuration.GetValue<int>("EmailSender:Port"),
                Configuration.GetValue<bool>("EmailSender:EnableSSL"),
                Configuration["EmailSender:Username"],
                Configuration["EmailSender:Password"]

                )
            );

            #endregion
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                SeedDatabase.Seed();
            }
            app.UseStaticFiles();
            app.UseAuthentication();
            //app.CustomStaticFiles();

            #region Routes

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                 name: "adminProducts",
                 template: "admin/products",
                 defaults: new { controller = "Admin", action = "ProductList" }
               );

                routes.MapRoute(
                    name: "adminProductsEdit",
                    template: "admin/products/{id?}",
                    defaults: new { controller = "Admin", action = "EditProduct" }
                );

                routes.MapRoute(
                name: "cart",
                template: "cart",
                defaults: new { controller = "Cart", action = "Index" }
              );
                routes.MapRoute(
               name: "checkout",
               template: "checkout",
               defaults: new { controller = "Cart", action = "Checkout" }
             );

                routes.MapRoute(
                  name: "products",
                  template: "products/{category?}",
                  defaults: new { controller = "Shop", action = "List" }
                );

                routes.MapRoute(
                  name: "orders",
                  template: "orders",
                  defaults: new { controller = "Cart", action = "GetOrders" }
                );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}"
                );

            });

            #endregion

            SeedIdentity.Seed(userManager, roleManager, Configuration).Wait();
             
        }
    }
}
