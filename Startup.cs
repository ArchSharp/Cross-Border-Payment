using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Identity.Data;
using Microsoft.EntityFrameworkCore;
using Identity.Interfaces;
using Identity.Services;
using System.Reflection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.Threading.Tasks;
using Identity.Interfaces.MessageBroker;
using Identity.Services.MessageBroker;
using Identity.HostedService;
using Identity.MessageBroker;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Identity.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using Identity.Policies.Handlers;
using Identity.Data.Models;

namespace Identity
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
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")), ServiceLifetime.Transient);
            services.Configure<TwilioFnParameters>(Configuration.GetSection("TwilioFnParameters"));

            services.AddHttpContextAccessor();
            services.AddSingleton<IUriService>(o =>
            {
                var accessor = o.GetRequiredService<IHttpContextAccessor>();
                var request = accessor.HttpContext.Request;
                var uri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());
                return new UriService(uri);
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
                        ClockSkew = TimeSpan.Zero
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("IS-TOKEN-EXPIRED", "true");
                                context.Response.StatusCode = 401;
                            }
                            return Task.CompletedTask;
                        },
                        OnChallenge = context =>
                        {
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";
                            return Task.CompletedTask;
                        },
                    };
                });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                  {
                    {
                      new OpenApiSecurityScheme
                      {
                        Reference = new OpenApiReference
                          {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                          },
                          Scheme = "oauth2",
                          Name = "Bearer",
                          In = ParameterLocation.Header,

                        },
                        new List<string>()
                      }
                    });
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Identity", Version = "v1" });
            });

            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddHttpClient()
                .AddSingleton<IRabbitMQConfig, RabbitMQConfig>()
                .AddTransient<IProducerService, ProducerService>()
                .AddTransient<IConsumerService, ConsumerService>()
                .AddTransient<ITokenService, TokenService>()
                .AddTransient<INotificationService, NotificationService>()
                .AddTransient<IAuthService, AuthService>()
                .AddTransient<IUserService, UserService>()
                .AddTransient<IHttpDataClient, HttpDataClient>()
                .AddTransient<ILocationService, LocationService>()
                .AddTransient<IRoleService, RoleService>()
                .AddTransient<IAccountService, AccountService>()
                .AddTransient<IZaiService, ZaiService>()
                .AddTransient<ICustomerService, CustomerService>()
                .AddTransient<IStaffService, StaffService>()
                .AddTransient<ITwilioService, TwilioService>()
                .AddTransient<ITransactionService, TransactionsService>()
                .AddHostedService<ConsumerHostedService>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.Requirements.Add(new RoleRequirement()));
            });
            services.AddSingleton<IAuthorizationHandler, RoleHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity v1"));
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
