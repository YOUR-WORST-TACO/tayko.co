using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Npgsql.PostgresTypes;
//using Tayko.co.Data;
using Tayko.co.Models;

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

            /*services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.AccessDeniedPath = new PathString("/auth/denied");
                    });*/

            /*services.AddDbContext<CommentDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("PgSql")));


            services.AddDataProtection()
                .PersistKeysToDbContext<CommentDbContext>();*/

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //services.AddSingleton<BlogDataManager>();
            services.AddSingleton(s => new BlogDataManager(HostingEnvironment));
            services.AddSingleton(s => new Blogerator(HostingEnvironment));
            //services.AddSingleton(provider => new {Provider})
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Development Exception handling pages
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }
            else // Production Exception handling pages
            {
                // Uses error controller and error code to handle errors
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
                app.UseHsts();
            }

            var blogerator = app.ApplicationServices.GetService<Blogerator>();

            app.UseStatusCodePagesWithReExecute("/Error/{0}");
            app.UseHsts();
            app.UseAuthentication();

            app.UseCookiePolicy();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            blogerator.Posts
                .Where(p => p.PostResourceDirectory != null)
                .ToList()
                .ForEach(p =>
                {
                    app.UseStaticFiles(new StaticFileOptions
                    {
                        FileProvider = new PhysicalFileProvider(
                            p.PostResourceDirectory.FullName),
                        RequestPath = $"/Blog/{p.PostName}"
                    });
                });

            /* ROUTES
             * - default drops its name and uses just its actions,
             * - blog sends all requests to LoadBlog
             * - error sends all requests to HandleError
             * - notfound sends Error 404
             */
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "LyzeRoute",
                    "Lyze",
                    new {action= "Index", mode="lyze", controller = "Home"});
                routes.MapRoute(
                    name: "default",
                    template: "{action=Index}/{*mode}",
                    defaults: new {controller = "Home"});
                routes.MapRoute(
                    name: "blog",
                    template: "Blog/{*article}",
                    defaults: new {controller = "Blog", action = "LoadBlog"});
                routes.MapRoute(
                    name: "error",
                    template: "Error/{error}",
                    defaults: new {controller = "Error", action = "HandleError"});
                routes.MapRoute(
                    "authentication",
                    "{controller=Auth}/{action=Login}");
                routes.MapRoute(
                    "NotFound",
                    "{*url}",
                    new {controller = "Error", action = "HandleError", error = 404});
            });
        }
    }
}