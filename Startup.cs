using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using UrlService.Infrastructure;
using UrlService.Models;
using UrlService.Services;

namespace UrlService
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ITokenFactory, JwtTokenFactory>();
            services.AddSingleton<IConnectionMultiplexer>(x =>
                ConnectionMultiplexer.Connect(Configuration.GetConnectionString("RedisConnection"))
                );
            services.AddSingleton<ICacheService, RedisCacheService>();

            services.AddDbContext<UrlServiceDbContext>(options =>
            {

                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            }
                );
            // var t = Configuration.GetConnectionString("DefaultConnection");


            services.AddControllers();

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<UrlServiceDbContext>();

            services.AddScoped<IAnalyticsRecordSaver, AnalyticsRecordSaver>();

            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["AuthSettings:Audience"],
                    ValidIssuer = Configuration["AuthSettings:Issuer"],
                    RequireExpirationTime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["AuthSettings:Key"])),
                    ValidateIssuerSigningKey = true
                };
            });

            services.AddSwaggerGen();
            ConfigureHangFire(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            MigrationsConfiguration.ApplyMigrations(app);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "URL Shortener Service");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void ConfigureHangFire(IServiceCollection services)
        {
            // using (var sp = services.BuildServiceProvider())
            // {
            //     using (var scop = sp.CreateScope())
            //     {
            //         var unitOfWork = scop.ServiceProvider.GetService<UrlServiceDbContext>();
            //         unitOfWork.Database.GetAppliedMigrations();
            //         unitOfWork.Database.Migrate();
            //     }
            // };

            var connection = string.Format(Configuration.GetConnectionString("HangfireConnection"),
                  Configuration["ApiUrls:Sql-Data"]);
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                // .UseSerilogLogProvider()
                .UseSqlServerStorage(connection, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));
            JobStorage.Current = new SqlServerStorage(connection);
            services.AddHangfireServer();
            services.AddTransient<Services.BackgroundService>();
            // Add framework services.
            services.AddMvc();


            var nextFixedHour = ((DateTime.Now.Hour) % 24) + 1;
            RecurringJob.AddOrUpdate<Services.BackgroundService>((data) =>
                data.CollectAndClean(),
                // $"0 0 0/{Configuration.GetValue<int>("BackgroundServiceTimeSpanInHour")} * * *");
            "*/" + Configuration.GetValue<int>("BackgroundServiceTimeSpanInHour") + " * * * *");

            RecurringJob.AddOrUpdate<Services.BackgroundService>((data) =>
                data.FlushOldData(), Cron.Daily);
        }
    }
}
