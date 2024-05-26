using Certitrack.Data;
using Certitrack.Models;
using jsreport.AspNetCore;
using jsreport.Local;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System;
using System.Runtime.InteropServices;

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
                // target platform binary
               .UseBinary(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                    jsreport.Binary.JsReportBinary.GetBinary() :
                    jsreport.Binary.Linux.JsReportBinary.GetBinary())
               .Configure((cfg) => {
                   // explicitly set port, because azure web app sets environment variable PORT
                   // which is used also by jsreport
                   cfg.HttpPort = 1000;
                   return cfg;
               }).AsUtility().Create());

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

            services.AddRazorPages();

            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
            services.AddSession();

            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if(env == null)
            {
                throw new ArgumentNullException("env", "Environment not found");
            }

            if (env.IsDevelopment())
            {
                SetConnectionStringWithContentRootPath(env);

                //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                //loggerFactory.AddDebug();

                app.UseDeveloperExceptionPage();
            }
            else
            {
                SetConnectionStringWithContentRootPath(env); // 2020-10-10

                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            //app.UseCookiePolicy();
            app.UseSession();
            app.UseAuthentication();

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=Login}/{id?}");
            });
        }

        private void SetConnectionStringWithContentRootPath(IWebHostEnvironment env)
        {
            string ContentRootPath = env.ContentRootPath;

            if (ConnectionString.Contains("%CONTENTROOTPATH%", StringComparison.CurrentCulture))
            {
                try
                {
                    ConnectionString = ConnectionString.Replace("%CONTENTROOTPATH%", ContentRootPath, StringComparison.CurrentCulture);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
                ConnectionString = Configuration.GetConnectionString("Certitrack");
        }
    }
}
