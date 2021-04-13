using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using P9_Backend.Models;
using P9_Backend.DAL;
using Microsoft.EntityFrameworkCore;
using P9_Backend.Services;
using Microsoft.EntityFrameworkCore.Diagnostics;
using P9_Backend.HubConfig;

namespace P9_Backend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            DatabaseSettings dbSettings = Configuration.GetSection(nameof(DatabaseSettings)).Get<DatabaseSettings>();

            services.AddDbContext<DatabaseContext>(options => options.UseMySQL(dbSettings.ConnectionString));

            services.AddSignalR();

            services.AddSingleton<IDroneService, DroneService>();
            services.AddSingleton<IBoatService, BoatService>();
            services.AddSingleton<ISocketService, SocketService>();
            services.AddSingleton<IFeedService, FeedService>();
            services.AddSingleton<IDemoService, DemoService>();
            services.AddSingleton<ILogsService, LogsService>();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.WithOrigins("http://localhost:4200", "http://ronsholt.me", "https://ronsholt.me")
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                    );
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthorization();

            app.ApplicationServices.GetService<ISocketService>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<DemoHub>("api/DemoStatus");
            });
        }
    }
}
