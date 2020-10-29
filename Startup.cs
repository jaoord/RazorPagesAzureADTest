using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Graph;
using System.Net;
using System.Net.Http.Headers;
using RazorPagesAzureADTest.Graph;


namespace RazorPagesAzureADTest
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
            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
               .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"))
               .EnableTokenAcquisitionToCallDownstreamApi(options => {
                   Configuration.Bind("AzureAd", options);
               }, GraphConstants.Scopes)
                // Use in-memory token cache
                // See https://github.com/AzureAD/microsoft-identity-web/wiki/token-cache-serialization
                
               .AddMicrosoftGraph(options =>
                {
                    options.Scopes = string.Join(' ', GraphConstants.Scopes);
                })
                .AddInMemoryTokenCaches(); 

            var adminPolicy = new AuthorizationPolicyBuilder()
                                    .RequireAuthenticatedUser()
                                    .RequireRole("Admin")
                                    .Build();
            
            services.AddAuthorization(options =>
            {
                options.AddPolicy("adminPolicy", adminPolicy);
            });

            services.AddRazorPages()
                .AddRazorPagesOptions(opt =>
                {
                    opt.Conventions.AuthorizeFolder("/");
                    opt.Conventions.AuthorizeAreaFolder("Admin", "/", "adminPolicy");
                })
                .AddMicrosoftIdentityUI();
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
