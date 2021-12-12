using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BethanysPieShop.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BethanysPieShop
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IPieRepository, PieRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            // ensures when user comes to the site shopping cart will be associated with that request
            services.AddScoped<ShoppingCart>(sp => ShoppingCart.GetCart(sp));

            services.AddControllersWithViews();//services.AddMvc(); would also work still
            // allows us to access the session object
            services.AddHttpContextAccessor();
            // allows us to use sessions (session ID, cookies)
            services.AddSession();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // uses secure HTTPS protocol
            app.UseHttpsRedirection();
            // serves static files - default folder for static files is wwwroot
            app.UseStaticFiles();
            // support for sessions
            app.UseSession();
            // this routing middleware component enables routing system (engine)
            /* we are going to use convention based routing
             * second task of the routing engine is generating correct links (tag helpers)
             * tag helpers create link in our view code (href) based on given controller and action
             * tag helpers trigger server-side execution of code
             * */
            app.UseRouting();

            // this middleware component enables routing system
            /* most apps have multiple routes defined
             * matching system will sequentially run through all defined patterns
             * and the first match will be used
             * that means order is important - most specific routes must be at the top
             * Endpoints are things that we are going to be navigating to.
             * */
            app.UseEndpoints(endpoints =>
            {
                /* adds convention based route to our application
                 * maps URI to specific action within a controller
                 * This is default route.
                 * */ 
                endpoints.MapControllerRoute(
                    name: "default",
                    // defaults are Home/Index when nothing is specified
                    /* Through a process called model binding the value of id 
                     * in the URI will be passed to the action parameter 
                     * in the corresponding controller.
                     * ? means that this URI segment is optional
                     * part after the colon (int in this case) is called constraint
                     * that means the last segment (id) must be integer value to be a match
                     * */
                    pattern: "{controller=Home}/{action=Index}/{id:int?}");
            });
        }
    }
}
