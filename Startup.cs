using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Tayko.co.Models;
using Tayko.co.Service;

namespace Tayko.co
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            HostingEnvironment = environment;
        }

        // Stores config options
        public IConfiguration Configuration { get; }

        // Stores hosting environment variables needed for Blogs
        public IWebHostEnvironment HostingEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Plan to remove cookies completely
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc(options => options.EnableEndpointRouting = false ).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(s => new Blogerator(HostingEnvironment));

            services.AddRazorPages()
                .AddRazorRuntimeCompilation();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
            
            Console.WriteLine($"Garsh look at what I found: {Environment.GetEnvironmentVariable("TESTING_123_123")}");

            var blogerator = app.ApplicationServices.GetService<Blogerator>();
            
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
                    name: "NotFound",
                    template:"{*url}",
                    defaults: new {controller = "Error", action = "HandleError", error = 404});
            });
        }
    }
}