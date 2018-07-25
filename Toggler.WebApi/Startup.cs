using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using Toggler.Common;
using Toggler.Common.Extensions;
using Toggler.Domain.SeedWork.Interfaces;
using Toggler.Infrastructure.Repositories;

namespace Toggler.WebApi
{
    public class Startup
    {
        private IHostingEnvironment _appHost;

        public Startup(IConfiguration configuration, IHostingEnvironment appHost)
        {
            Configuration = configuration;
            _appHost = appHost;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://localhost:5000"; //Identity Server URL
                    options.RequireHttpsMetadata = false;// make it false since we are not using https. Only for development
                    options.ApiName = "toggler_auth_api"; //api name which should be registered in IdentityServer
                });

            services.AddMvcCore()
                .AddApiExplorer()
                .AddJsonFormatters(
#if DEBUG
                    settings => settings.Formatting = Newtonsoft.Json.Formatting.Indented
#endif
                ).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddDbContext<TogglerContext>(options =>
            {
                var connectionString = $"Data Source={_appHost.ContentRootPath}/data.db"; //TODO: Decide to put in appsettings.json

                // SQLite kindly creates a DB-file if it doesn't exist, but it doesn't create a directory. 
                // We need to ensure that the directory exists. 

                var regex = new Regex("Data Source=([^;]+);?");
                var pathMatch = regex.Match(connectionString);
                var dbFilePath = pathMatch.Groups[1].Value;
                Directory.CreateDirectory(Path.GetDirectoryName(dbFilePath));

                options.UseSqlite(connectionString);
            });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "Toggler Web API",
                    Version = "v1",
                    Description = "Toggler ASP.NET Core Web API",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "Bishwaranjan Sandhu",
                        Email = string.Empty,
                        Url = "https://twitter.com/bishwaranjans"
                    },
                    License = new License
                    {
                        Name = "Use under LICX",
                        Url = "https://example.com/license"
                    }
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Toggler Web API V1");
            });

            app.UseMvc();

        }
    }
}
