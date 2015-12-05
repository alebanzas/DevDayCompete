using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DevDayKeynote.Models;
using DevDayKeynote.Services;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using LogLevel = Microsoft.Framework.Logging.LogLevel;

namespace DevDayKeynote
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public static IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            // Add application services.
            services.AddSingleton<IVoteLog, CacheVoteGetter>();
            services.AddSingleton<IVoteGet, CacheVoteGetter>();

            //services.AddSingleton<IVoteLog, QueueVoteLog>();

            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // For more details on creating database during deployment see http://go.microsoft.com/fwlink/?LinkID=615859
                try
                {
                    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                        .CreateScope())
                    {
                        serviceScope.ServiceProvider.GetService<ApplicationDbContext>()
                             .Database.Migrate();
                    }
                }
                catch { }
            }

            app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

            app.UseStaticFiles();

            app.UseIdentity();

            
            var cloudStorageAccount = CloudStorageAccount.Parse(Configuration["MicrosoftAzureStorage:devdaydemo_AzureStorageConnectionString"]);
            var createCloudQueueClient = cloudStorageAccount.CreateCloudQueueClient();
            var queue = createCloudQueueClient.GetQueueReference(typeof(Voto).Name.ToLowerInvariant());
            queue.CreateIfNotExistsAsync();

            // To configure external authentication please see http://go.microsoft.com/fwlink/?LinkID=532715
            app.UseFacebookAuthentication(options =>
            {
                options.AppId = Configuration["AppSettings:Authentication:Facebook:AppId"];
                options.AppSecret = Configuration["AppSettings:Authentication:Facebook:AppSecret"];
                options.Scope.Add("email");
            });
            app.UseTwitterAuthentication(options =>
            {
                options.ConsumerKey = Configuration["AppSettings:Authentication:Twitter:ConsumerKey"];
                options.ConsumerSecret = Configuration["AppSettings:Authentication:Twitter:ConsumerSecret"];
            });
            // app.UseGoogleAuthentication();
            // app.UseMicrosoftAccountAuthentication();


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
