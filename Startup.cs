using Certitrack.Data;
using Certitrack.Models;
using jsreport.AspNetCore;
using jsreport.Binary;
using jsreport.Local;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Certitrack
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetConnectionString("Certitrack");
        }

        public IConfiguration Configuration { get; }
        public static string ConnectionString { get; set; }

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddJsReport(new LocalReporting()
               .UseBinary(JsReportBinary.GetBinary())
               .AsUtility()
               .Create());

            services.AddIdentity<Staff, Role>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            }).AddEntityFrameworkStores<CertitrackContext>().AddDefaultTokenProviders();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<CertitrackContext>(options =>
            {
                options.UseSqlServer(ConnectionString, o => o.EnableRetryOnFailure());
            });

            services.AddAntiforgery(options =>
            {
                options.Cookie.Name = "X-CSRF-TOKEN-CERTITRACK";
                options.FormFieldName = "CSRF-TOKEN-CERTITRACK-FORM";
            });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddSessionStateTempDataProvider();

            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
            services.AddSession();

            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                //2019-09-10 enable portable db
                string ContentRootPath = env.ContentRootPath;

                //2019-09-10 enable portable db
                if (ConnectionString.Contains("%CONTENTROOTPATH%"))
                {
                    try
                    {
                        ConnectionString = ConnectionString.Replace("%CONTENTROOTPATH%", ContentRootPath);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                loggerFactory.AddDebug();

                app.UseDeveloperExceptionPage();
            }
            else
            {
                ConnectionString = Configuration.GetConnectionString("Certitrack");
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            //app.UseCookiePolicy();
            app.UseSession();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Account}/{action=Login}/{id?}");
            });
        }
    }
}
