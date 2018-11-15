using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using WebApplicationCoreTest1.Configs;
using WebApplicationCoreTest1.Extensions;
using WebApplicationCoreTest1.Interfaces;
using WebApplicationCoreTest1.Models;

namespace WebApplicationCoreTest1
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory log)
        {
            log.AddLog4Net(Configuration.GetSection("Log4NetCore").Get<Log4NetProviderOptions>());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseStatusCodePages();
            app.UseSwagger();
            app.UseSwaggerUI(g =>
            {
                g.SwaggerEndpoint("/swagger/v1/swagger.json", "My Test API V1");
                g.RoutePrefix = "swagger";
                g.InjectStylesheet("/css/SwaggerWebAppCoreTest1.css");
            });

            app.UseCors(c => c.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseAuthentication();
            app.UseMvc();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(c =>
            {
                c.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                    .Build());
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddHsts(h =>
            {
                h.IncludeSubDomains = true;
            });

            services.AddSwaggerGen(generator =>
            {
                generator.SwaggerDoc("v1", new Info
                {
                    Title = "My Test API",
                    Version = "v1",
                    Description = "This is a test",
                    TermsOfService = "None",
                    Contact = new Contact()
                    {
                        Name = "Charmelio Salamander",
                        Email = string.Empty,
                        Url = "https://charmel.io"
                    }
                });

                generator.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
                generator.DocumentFilter<SwaggerEnumExt>();
            });

            // If models need to be updated run the following in Powershell:
            // Scaffold-DbContext "server=localhost;user id=root;password=e3zXNMIIrnWHGmPMO71M;database=testing" MySql.Data.EntityFrameworkCore -OutputDir Models
            services.AddDbContext<testingContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            //services.AddScoped<testingContext>(_ => new testingContext(Configuration.GetConnectionString("DefaultConnection"));

            services.Configure<TokenSettings>(Configuration.GetSection("JWT"));
            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetSection("JWT").Get<TokenSettings>().Secret)),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = Configuration["JWT:Issuer"],
                        ValidAudience = Configuration["JWT:Issuer"]
                    };
                });

            services.AddScoped<ITokenService, TokenService>();
        }
    }
}