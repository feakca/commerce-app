using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using CommerceApp.BusinessLogicLayer.Abstract;
using CommerceApp.BusinessLogicLayer.Concrete;
using CommerceApp.DataAccessLayer.Abstract;
using CommerceApp.DataAccessLayer.Concrete.EFCore;
using CommerceApp.WebUI.EmailServices;
using CommerceApp.WebUI.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace CommerceApp.WebUI
{
    public class Startup
    {
        private IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CommerceApp;Integrated Security=True"));
            services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;

                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                options.Lockout.AllowedForNewUsers = true;

                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
                options.AccessDeniedPath = "/accessdenied";
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(365);
                options.Cookie = new CookieBuilder()
                {
                    HttpOnly = true,
                    Name = ".CommerceApp.Security.Cookie",
                    SameSite = SameSiteMode.Strict
                };
            });

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductService, ProductManager>();

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryService, CategoryManager>();

            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped<IBasketService, BasketManager>();

            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderService, OrderManager>();

            services.AddScoped<IEmailSender, SmtpEmailSender>(i => new SmtpEmailSender(
                _configuration["EmailSender:Host"],
                _configuration.GetValue<int>("EmailSender:Port"),
                _configuration.GetValue<bool>("EmailSender:EnableSsl"),
                _configuration["EmailSender:UserName"],
                _configuration["EmailSender:Password"]
                )
            );

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                SeedDatabase.Seed();
                app.UseDeveloperExceptionPage();
            }

            SeedIdentity.Seed(userManager, roleManager, configuration).Wait();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                #region admin
                endpoints.MapControllerRoute(
                    name: "adminOrderList",
                    pattern: "admin/orders",
                    defaults: new { controller = "Admin", action = "GetOrders" });
                endpoints.MapControllerRoute(
                    name: "adminProductList",
                    pattern: "admin/products",
                    defaults: new { controller = "Admin", action = "ProductList" });
                endpoints.MapControllerRoute(
                    name: "adminProductCreate",
                    pattern: "admin/products/create",
                    defaults: new { controller = "Admin", action = "ProductCreate" });
                endpoints.MapControllerRoute(
                    name: "adminProductEdit",
                    pattern: "admin/products/{id?}",
                    defaults: new { controller = "Admin", action = "ProductEdit" });
                endpoints.MapControllerRoute(
                    name: "adminCategoryList",
                    pattern: "admin/categories",
                    defaults: new { controller = "Admin", action = "CategoryList" });
                endpoints.MapControllerRoute(
                    name: "adminCategoryCreate",
                    pattern: "admin/categories/create",
                    defaults: new { controller = "Admin", action = "CategoryCreate" });
                endpoints.MapControllerRoute(
                    name: "adminCategoryEdit",
                    pattern: "admin/categories/{id?}",
                    defaults: new { controller = "Admin", action = "CategoryEdit" });
                endpoints.MapControllerRoute(
                    name: "adminRoleList",
                    pattern: "admin/roles",
                    defaults: new { controller = "Admin", action = "RoleList" });
                endpoints.MapControllerRoute(
                    name: "adminRoleCreate",
                    pattern: "admin/roles/create",
                    defaults: new { controller = "Admin", action = "RoleCreate" });
                endpoints.MapControllerRoute(
                    name: "adminRoleEdit",
                    pattern: "admin/roles/{id?}",
                    defaults: new { controller = "Admin", action = "RoleEdit" });
                endpoints.MapControllerRoute(
                    name: "adminUserList",
                    pattern: "admin/users",
                    defaults: new { controller = "Admin", action = "UserList" });
                endpoints.MapControllerRoute(
                    name: "adminUserList",
                    pattern: "admin/users/create",
                    defaults: new { controller = "Admin", action = "UserCreate" });
                endpoints.MapControllerRoute(
                    name: "adminUserList",
                    pattern: "admin/users/{id?}",
                    defaults: new { controller = "Admin", action = "UserEdit" });
                #endregion

                #region basket
                endpoints.MapControllerRoute(
                    name: "basket",
                    pattern: "basket",
                    defaults: new { controller = "Basket", action = "Index" });
                endpoints.MapControllerRoute(
                    name: "checkout",
                    pattern: "checkout",
                    defaults: new { controller = "Basket", action = "Checkout" });
                #endregion

                #region order
                endpoints.MapControllerRoute(
                    name: "orders",
                    pattern: "orders",
                    defaults: new { controller = "Order", action = "GetOrders" });
                #endregion

                #region shop
                endpoints.MapControllerRoute(
                    name: "productDetails",
                    pattern: "{productUrl}",
                    defaults: new { controller = "Shop", action = "Details" });
                endpoints.MapControllerRoute(
                    name: "products",
                    pattern: "products/{categoryUrl?}",
                    defaults: new { controller = "Shop", action = "List" });
                #endregion

                #region account
                endpoints.MapControllerRoute(
                    name: "accountAccessDenied",
                    pattern: "accessdenied",
                    defaults: new { controller = "Account", action = "AccessDenied" });
                endpoints.MapControllerRoute(
                    name: "accountRegister",
                    pattern: "register",
                    defaults: new { controller = "Account", action = "Register" });
                endpoints.MapControllerRoute(
                    name: "accountLogin",
                    pattern: "login",
                    defaults: new { controller = "Account", action = "Login" });
                endpoints.MapControllerRoute(
                    name: "accountForgotPassword",
                    pattern: "forgotpassword",
                    defaults: new { controller = "Account", action = "ForgotPassword" });
                endpoints.MapControllerRoute(
                    name: "accountResetPassword",
                    pattern: "resetpassword",
                    defaults: new { controller = "Account", action = "ResetPassword" });
                endpoints.MapControllerRoute(
                    name: "accountConfirmEmail",
                    pattern: "confirmemail",
                    defaults: new { controller = "Account", action = "ConfirmEmail" });
                #endregion

                #region default
                endpoints.MapControllerRoute(
                    name: "home",
                    pattern: "home",
                    defaults: new { controller = "Home", action = "Index" });
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                #endregion
            });
        }
    }
}
