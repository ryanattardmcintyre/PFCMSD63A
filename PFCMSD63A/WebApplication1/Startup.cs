using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApplication1.DataAccess.Interfaces;
using WebApplication1.DataAccess.Repositories;
using Google.Cloud.Diagnostics.AspNetCore;
using Google.Cloud.SecretManager.V1;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @"C:\Users\Ryan\Downloads\pfc2021-420505dc0fd3.json");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGoogleExceptionLogging(options =>
            {
                options.ProjectId = "pfc2021";
                options.ServiceName = "Pfc2021msd63a"; //random name where the logs will be categorized
                options.Version = "0.01";
            });

            var passwordForPosgres = GetPostgresPassword();
            var connectionstringwithoutpassword = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    connectionstringwithoutpassword+passwordForPosgres+";"
                   ));


            //these 3 lines here, they basically specify to asp.net core what the dependency injector should follow
            //in order to create the required instances
            services.AddScoped<IBlogsRepository, BlogsFirestoreRepository>();
            services.AddScoped<ICacheRepository, CacheRepository>();
            services.AddScoped<IPubSubRepository, PubSubRepository>();
            services.AddScoped<ILogRepository, LogRepository>();


            //how you can register classes + interfaces which you have created with the IOC container


            // services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //     .AddEntityFrameworkStores<ApplicationDbContext>();


            services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddDefaultUI()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();




            services.AddControllersWithViews();
            services.AddRazorPages();

            var clientId = GetClientIdOrSecret("Authentication:Google:ClientId");
            var clientSecret = GetClientIdOrSecret("Authentication:Google:ClientSecret");
            services.AddAuthentication()
                  .AddGoogle(options =>
                  {
                      ////IConfigurationSection googleAuthNSection =
                      ////    Configuration.GetSection("Authentication:Google");

                      //options.ClientId = googleAuthNSection["ClientId"];
                      //options.ClientSecret = googleAuthNSection["ClientSecret"];

                      options.ClientId = clientId;
                      options.ClientSecret = clientSecret;
                  });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); //this pages shows too much technical stuff which can help fix the errors
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error"); //error page showing less technical stuff to the end user

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            app.UseGoogleExceptionLogging();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }


        public string GetClientIdOrSecret(string key)
        {
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Build the resource name.
            SecretVersionName secretVersionName = new SecretVersionName("pfc2021", "ApiClientId", "7");

            // Call the API.
            AccessSecretVersionResponse result = client.AccessSecretVersion(secretVersionName);

            // Convert the payload to a string. Payloads are bytes by default.
            String payload = result.Payload.Data.ToStringUtf8();

         //   dynamic deserializedObject = JsonConvert.DeserializeObject(payload);
            
            JObject jObject = JObject.Parse(payload);
            JToken jKey = jObject[key];
            return jKey.ToString();
        }
        public string GetPostgresPassword()
        {
            SecretManagerServiceClient client = SecretManagerServiceClient.Create();

            // Build the resource name.
            SecretVersionName secretVersionName = new SecretVersionName("pfc2021", "PosgtresPassword", "2");

            // Call the API.
            AccessSecretVersionResponse result = client.AccessSecretVersion(secretVersionName);

            // Convert the payload to a string. Payloads are bytes by default.
            String payload = result.Payload.Data.ToStringUtf8();

            return payload;
        }
    }
}
