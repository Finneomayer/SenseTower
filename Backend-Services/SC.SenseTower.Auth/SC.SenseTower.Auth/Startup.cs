using IdentityServer4.ResponseHandling;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using MongoDbGenericRepository;
using Polly;
using Polly.Extensions.Http;
using SC.SenseTower.Auth.Data;
using SC.SenseTower.Auth.Generators;
using SC.SenseTower.Auth.Models;
using SC.SenseTower.Auth.Services;
using SC.SenseTower.Auth.Services.EmailSender;
using SC.SenseTower.Auth.Services.RabbitMQ;
using SC.SenseTower.Auth.Settings;
using SC.SenseTower.Auth.Validators;
using System.Text.Json;

namespace SC.SenseTower.Auth
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
            services.Configure<MongoDbConfig>(Configuration.GetSection(nameof(MongoDbConfig)));
            services.Configure<ServiceEndpointsSettings>(Configuration.GetSection(nameof(ServiceEndpointsSettings)));
            services.Configure<MailerSettings>(Configuration.GetSection(nameof(MailerSettings)));
            services.Configure<RabbitMQSettings>(Configuration.GetSection(nameof(RabbitMQSettings)));

            services.AddScoped<InitialSeedService>();
            services.AddSingleton<RabbitMQService>();

            var mongoDbSettings = Configuration.GetSection(nameof(MongoDbConfig)).Get<MongoDbConfig>();
            var settings = MongoClientSettings.FromConnectionString(mongoDbSettings.ConnectionString);
            var client = new MongoClient(settings);
            var mongoDbContext = new MongoDbContext(client, mongoDbSettings.DatabaseName);

            services.AddIdentity<ApplicationUser, ApplicationRole>(o =>
            {
                o.User.RequireUniqueEmail = true;
                o.Password.RequireDigit = true;
                o.Password.RequiredLength = 12;
                o.Password.RequireLowercase = true;
                o.Password.RequireNonAlphanumeric = true;
                o.Password.RequireUppercase = true;
            })
                .AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>
                (
                    mongoDbContext
                )
                .AddDefaultTokenProviders();

            var identityServerSettings = Configuration.GetSection(nameof(IdentityServerSettings)).Get<IdentityServerSettings>();
            services.AddIdentityServer(options =>
            {
                options.IssuerUri = identityServerSettings.UssuerUri;

                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
                .AddAspNetIdentity<ApplicationUser>()
                .AddInMemoryApiScopes(identityServerSettings.ApiScopes)
                .AddInMemoryApiResources(identityServerSettings.ApiResources)
                .AddInMemoryClients(identityServerSettings.Clients)
                .AddInMemoryIdentityResources(identityServerSettings.IdentityResources)
                .AddCustomTokenRequestValidator<AccessGrantedToTokenValidator>()
                .AddCustomAuthorizeRequestValidator<AccessGrantedToAuthorizeValidator>()
                .AddJwtBearerClientAuthentication()
                .AddDeveloperSigningCredential();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o =>
                {
                    o.Authority = identityServerSettings.UssuerUri;
                    o.Audience = identityServerSettings.ApiResources.First().Name;
                    o.RequireHttpsMetadata = false;
                    o.TokenValidationParameters.ValidIssuer = identityServerSettings.UssuerUri;
                    o.TokenValidationParameters.ValidAudience = identityServerSettings.ApiResources.First().Name;
                    o.TokenValidationParameters.ValidateAudience = true;
                    o.TokenValidationParameters.ValidateIssuer = true;
                    o.TokenValidationParameters.ValidateActor = false;
                    o.TokenValidationParameters.ValidateIssuerSigningKey = false;
                    o.TokenValidationParameters.ValidateLifetime = true;
                    o.TokenValidationParameters.ValidateTokenReplay = false;
                    o.TokenValidationParameters.RequireSignedTokens = false;

                    o.Events = new JwtBearerEvents();
                    o.Events.OnAuthenticationFailed += context =>
                    {
                        using var scope = context.HttpContext.RequestServices.CreateScope();
                        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Startup>>();
                        var message = $"AuthenticationFailed(): Exception = {context.Exception?.Message}, " +
                            $"InnerException = {context.Exception?.InnerException?.Message}, " +
                            $"Scheme = {context.Scheme}, " +
                            $"Path = {JsonSerializer.Serialize(context.Request.Path)}, " +
                            $"Result = {JsonSerializer.Serialize(context.Result?.Succeeded)}, " +
                            $"Failure = {JsonSerializer.Serialize(context.Result?.Failure)}"
                            ;
                        logger.LogDebug(message);
                        return Task.CompletedTask;
                    };
                    o.Events.OnChallenge += (context) =>
                    {
                        using var scope = context.HttpContext.RequestServices.CreateScope();
                        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Startup>>();
                        var message = $"Challenge(): Exception = {context.AuthenticateFailure?.Message}, " +
                            $"InnerException = {context.AuthenticateFailure?.InnerException?.Message}, " +
                            $"Scheme = {context.Scheme}, " +
                            $"Path = {JsonSerializer.Serialize(context.Request?.Path)}, " +
                            $"Error = {JsonSerializer.Serialize(context.Error)}"
                            ;
                        logger.LogDebug(message);
                        return Task.CompletedTask;
                    };
                    o.Events.OnForbidden += (context) =>
                    {
                        using var scope = context.HttpContext.RequestServices.CreateScope();
                        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Startup>>();
                        var message = $"Forbidden(): Scheme = {context.Scheme}, " +
                            $"Path = {JsonSerializer.Serialize(context.Request.Path)}, " +
                            $"Result = {JsonSerializer.Serialize(context.Result?.Succeeded)}, " +
                            $"Failure = {JsonSerializer.Serialize(context.Result?.Failure)}"
                            ;
                        logger.LogDebug(message);
                        return Task.CompletedTask;
                    };
                    o.Events.OnMessageReceived += (context) =>
                    {
                        using var scope = context.HttpContext.RequestServices.CreateScope();
                        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Startup>>();
                        var message = $"MessageReceived(): Token = {context.Token}, " +
                            $"Scheme = {context.Scheme}, " +
                            $"Path = {JsonSerializer.Serialize(context.Request.Path)}, " +
                            $"Result = {JsonSerializer.Serialize(context.Result?.Succeeded)}, " +
                            $"Failure = {JsonSerializer.Serialize(context.Result?.Failure)}"
                            ;
                        logger.LogDebug(message);
                        return Task.CompletedTask;
                    };
                    o.Events.OnTokenValidated += (context) =>
                    {
                        using var scope = context.HttpContext.RequestServices.CreateScope();
                        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Startup>>();
                        var message = $"TokenValidated(): SecurityToken = {context.SecurityToken}, " +
                            $"Scheme = {context.Scheme}, " +
                            $"Path = {JsonSerializer.Serialize(context.Request.Path)}, " +
                            $"Result = {JsonSerializer.Serialize(context.Result?.Succeeded)}, " +
                            $"Failure = {JsonSerializer.Serialize(context.Result?.Failure)}"
                            ;
                        logger.LogDebug(message);
                        return Task.CompletedTask;
                    };
                });

            services.AddAutoMapper(typeof(MappingProfile));
            services.AddScoped<AuthDbContext>();
            services.AddScoped<EmailSenderService>();
            var mailerSettings = Configuration.GetSection(nameof(MailerSettings)).Get<MailerSettings>();
            services.AddHttpClient<EmailSenderService>(client =>
            {
                client.BaseAddress = new Uri(mailerSettings.RootUrl);
            })
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .WaitAndRetryAsync(mailerSettings.MaxAttempts, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
                .AddPolicyHandler(_ => HttpPolicyExtensions
                    .HandleTransientHttpError()
                    .CircuitBreakerAsync(mailerSettings.BreakAfter, TimeSpan.FromSeconds(mailerSettings.BreakForSeconds)));
            services.AddScoped<UsersService>();
            services.AddControllersWithViews();

            services.Replace(ServiceDescriptor.Transient<ITokenResponseGenerator, LoggingTokenResponseGenerator>());
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

            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
