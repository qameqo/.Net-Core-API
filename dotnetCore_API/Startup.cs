using dotnetCore_API.Center;
using dotnetCore_API.Center.Interfaces;
using dotnetCore_API.Models.Common;
using dotnetCore_API.Services;
using dotnetCore_API.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace dotnetCore_API
{
    public static class AppSetting
    {
        public static IConfiguration Configuration;
    }
    public class Startup
    {
        private readonly string _assEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        public IConfiguration Configuration { get; }

        private string assmName = Assembly.GetExecutingAssembly().GetName().Name;
        public Startup(IWebHostEnvironment env)
        {
            if (String.IsNullOrEmpty(this._assEnv) == true)
            {
                throw new Exception("Not found ASPNETCORE_ENVIRONMENT Variable");
            }

            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile($"appsettings.{this._assEnv}.json", optional: true, reloadOnChange: true)
            .AddJsonFile("message.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
            this.Configuration = builder.Build();
            AppSetting.Configuration = this.Configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddCors();
            services.AddMvc();
            services.AddCors(o =>
            {
                o.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("V1", new OpenApiInfo { Title = $"{assmName} - {_assEnv}", Version = "V1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = $@"Standard Authorization header using the Bearer scheme. Example: ""bearer [token]"" 
                        <br>Test Key (actor: Note, role: ADMIN) <br />
                        <br>Please copy text below and paste in value input. <br /> 
                        <textarea readonly style='height:150px;min-height:unset;'>
                            Bearer {_assEnv.GetBearerSchem()}
                        </textarea>"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer",
                            },
                        },
                        new string[] {}
                    }
                });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                    };
                });
            services.AddAuthorization(option =>
            {
                option.AddPolicy("JwtPolicy", builder =>
                {
                    builder.RequireAuthenticatedUser();
                    builder.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                });
            });

            services.Configure<Jwt>(options => Configuration.GetSection("Jwt").Bind(options));

            services.AddMvc(option =>
            {
                option.Conventions.Add(new JwtAuthorizationConvention("JwtPolicy", Convert.ToBoolean(Configuration["Jwt:Authen"]), Configuration["Jwt:ActionIgnore"]?.Split(',')));
            });

            services.AddScoped<ICustomerInfoServices, CustomerInfoServices>();
            services.AddScoped<IDBCenter, DBCenter>();
            services.AddAutoTransient();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsEnvironment("Localhost"))
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/V1/swagger.json", "1.0");
                });

            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseCors("CorsPolicy");
        }
    }
    public class JwtAuthorizationConvention : IApplicationModelConvention
    {
        private readonly string[] _actionIgnore;
        private readonly string _policy;
        private readonly bool _auth;

        public JwtAuthorizationConvention(string policy, bool auth, string[] actionIgnore)
        {
            _policy = policy;
            _auth = auth;
            _actionIgnore = actionIgnore;
        }

        public void Apply(ApplicationModel application)
        {
            if (_auth)
            {
                application.Controllers.ToList().ForEach(controller =>
                {
                    var isController = controller.Selectors.Any(x => x.AttributeRouteModel != null
                                                            && x.AttributeRouteModel.Template.ToLower().StartsWith("api"));
                    if (isController)
                    {
                        controller.Actions.ToList().ForEach(action =>
                        {
                            var isActionAuthen = _actionIgnore == null ? true : _actionIgnore?.Contains(action.ActionName.ToLower()) == false;
                            if (isActionAuthen)
                            {
                                action.Filters.Add(new AuthorizeFilter(_policy));
                            }
                        });
                    }
                });
            }
        }
    }

    public static class NswagExtensions
    {
        public static string GetBearerSchem(this string env)
        {
            string token = "";
            try
            {

                if (env.ToLower() == "localhost")
                {
                    token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA5LzA5L2lkZW50aXR5L2NsYWltcy9hY3RvciI6IkNhbGxzY3JlZW5VSSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6InVzZXIiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9oYXNoIjoiMjFkNDRlZmMtMGViZC00MjcxLTg0NmMtMzZmY2VmZTk1ODYyIiwiZXhwIjoxNzg4OTQ2ODYwLCJuYmYiOjE2MzExODA0ODEsImlzcyI6ImNhbGxzY3JlZW5zeXN0ZW0iLCJhdWQiOiJjYWxsc2NyZWVuc3lzdGVtIn0.5enokGoo6XnfL7Dff6c6ioBxFYTgbHoJQNN9MFwr7lQ";
                }
                else if (env.ToLower() == "development")
                {
                    token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA5LzA5L2lkZW50aXR5L2NsYWltcy9hY3RvciI6IkNhbGxzY3JlZW5VSSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6InVzZXIiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9oYXNoIjoiMjFkNDRlZmMtMGViZC00MjcxLTg0NmMtMzZmY2VmZTk1ODYyIiwiZXhwIjoxNzg4OTQ2ODYwLCJuYmYiOjE2MzExODA0ODEsImlzcyI6ImNhbGxzY3JlZW5zeXN0ZW0iLCJhdWQiOiJjYWxsc2NyZWVuc3lzdGVtIn0.5enokGoo6XnfL7Dff6c6ioBxFYTgbHoJQNN9MFwr7lQ";
                }
                else if (env.ToLower() == "uat")
                {
                    token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA5LzA5L2lkZW50aXR5L2NsYWltcy9hY3RvciI6IkNhbGxzY3JlZW5VSSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6InVzZXIiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9oYXNoIjoiNGZjMzRjMjgtMzM0OC00NTkyLWFkNDktNWFkOWZkODBmNmUxIiwiZXhwIjoxNzg4OTQ3MDYwLCJuYmYiOjE2MzExODA2NjMsImlzcyI6ImNhbGxzY3JlZW5zeXN0ZW0iLCJhdWQiOiJjYWxsc2NyZWVuc3lzdGVtIn0.GdNLdD5ck-yyNAQXsRWn1Fq2mMWoFhCX4RXHhCO2Mew";
                }
                else
                {
                    token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA5LzA5L2lkZW50aXR5L2NsYWltcy9hY3RvciI6IkNhbGxzY3JlZW5VSSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6InVzZXIiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9oYXNoIjoiZWMwOTM4OTAtM2M2YS00MWY5LTk3MjItMDM1YWZmMWI1MDIyIiwiZXhwIjoxNzg4OTQ3MjI2LCJuYmYiOjE2MzExODA4MjYsImlzcyI6ImNhbGxzY3JlZW5zeXN0ZW0iLCJhdWQiOiJjYWxsc2NyZWVuc3lzdGVtIn0.s1PBCg-fP1yygV78rIlPw-kfk8l_tNDGUtHGk1sEGBw";
                }

                return token;
            }
            catch
            {
                return token;
            }
        }
    }

    public static class SetupServiceLifeTime
    {
        public static void AddAutoTransient(this IServiceCollection services)
        {
            List<Type> allType = new List<Type>();
            List<string> nsRange = new List<string> { "dotnetCore_API.Services" };
            nsRange.ForEach(n =>
            {
                var tmp = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.Namespace != null && t.Namespace.StartsWith(n)).Where(t => t.IsClass && !t.IsAbstract && t.IsPublic).ToList();


                List<Type> srvTyp = Assembly.GetExecutingAssembly().GetTypes()
                                        .Where(t => t.Namespace != null && t.Namespace.StartsWith(n))
                                        //.Where(t => t.Namespace != null && !t.Namespace.EndsWith(".Common") && !t.Namespace.EndsWith(".MicroServices") && !t.Namespace.EndsWith(".Entities"))
                                        .Where(t =>
                                            t.Namespace != null &&
                                            !t.Namespace.EndsWith(".Common") &&
                                            //!t.Namespace.EndsWith(".MicroServices") &&
                                            !t.Namespace.EndsWith(".Entities")
                                        )
                                        .Where(t => t.IsClass && !t.IsAbstract && t.IsPublic).ToList();

                allType.AddRange(srvTyp);
            });

            if (allType != null && allType.Any())
            {
                foreach (var item in allType)
                {
                    var intrf = item.GetInterfaces().FirstOrDefault();
                    if (intrf != null)
                    {
                        services.AddTransient(intrf, item);
                    }
                    else
                    {
                        services.AddTransient(item);
                    }
                }
            }
        }
    }
}
