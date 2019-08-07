using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tayko.co.Data;

namespace Tayko.co
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            HostingEnvironment = environment;
        }

        // Stores config options
        public IConfiguration Configuration { get; }
        
        // Stores hosting environment variables needed for Blogs
        public IHostingEnvironment HostingEnvironment { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {
            // Plan to remove cookies completely
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // If development, use sqlite
            if (HostingEnvironment.IsDevelopment())
            {
                services.AddDbContext<CommentDbContext>(options =>
                    options.UseSqlite(
                        Configuration.GetConnectionString("DefaultConnection")));
            }
            else // implement postgresql
            {
                
            }

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Development Exception handling pages
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else // Production Exception handling pages
            {
                // Uses error controller and error code to handle errors
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            /* ROUTES
             * - default drops its name and uses just its actions,
             * - blog sends all requests to LoadBlog
             * - error sends all requests to HandleError
             * - notfound sends Error 404
             */
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{action=Index}/{id?}",
                    new { controller = "Home"});
                
                routes.MapRoute(
                    name: "blog",
                    template: "Blog/{*article}",
                    new {controller = "Blog", action="LoadBlog"});
                routes.MapRoute(
                    name: "error",
                    template: "Error/{error}",
                    new {controller = "Error", action="HandleError"});
                routes.MapRoute(
                    "NotFound",
                    "{*url}",
                    new { controller = "Error", action = "HandleError", error=404});
            });
        }
    }
}