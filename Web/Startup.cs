using Core.Services.Interfaces;
using Database;
using IOC;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Config.ConnectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? "Server=postgres;Port=5432;Database=fileshare;Userid=user;Password=password;";
            Config.DocumentsFolderPath = Environment.GetEnvironmentVariable("DOCUMENT_DIRECTORY") ?? "/DocumentsFileShare";
            Core.Config.DocumentsFolderPath = Environment.GetEnvironmentVariable("DOCUMENT_DIRECTORY") ?? "/DocumentsFileShare";
            Config.ServerUrl = Environment.GetEnvironmentVariable("SERVER_URL") ?? "https://localhost:8686";
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ArcheDbContext>(o => o.UseNpgsql(Config.ConnectionString, x => x.MigrationsHistoryTable("__EFMigrationsHistory", "public")), ServiceLifetime.Transient);

            IUserService userService;
            Container.ServiceCollection = services;
            Container.RegisterAllTypes(ServiceLifetime.Transient);

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
            });

            services.AddControllersWithViews();
            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseForwardedHeaders();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapHealthChecks("/health"));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Index}/{action=Index}/{id?}");
            });
        }
    }
}
