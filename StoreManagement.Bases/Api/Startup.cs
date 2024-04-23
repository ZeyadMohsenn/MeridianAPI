using StoreManagement.Bases.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.Bases;

public class Startup
{
    public static void ConfigureServices<TDbContext, YAppUser, ZAppRole>(IServiceCollection services, IConfiguration Configuration) where TDbContext : DbContext where YAppUser : class where ZAppRole : class
    {
        services.AddControllers();

        services.AddOptions();

        services.Configure<TokenSettings>(Configuration.GetSection("TokenSettings"));


        services.AddCors(c =>
        {
            c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin());
        });

        services.AddHttpContextAccessor();

        //services.AddAutoMapper(System.Reflection.Assembly.Load("BusinessCard.Application"));

        services.Configure<RequestLocalizationOptions>(options =>
        {
            options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en");
            options.SupportedCultures = new CultureInfo[] {
                    new CultureInfo("ar") { DateTimeFormat = { Calendar = new GregorianCalendar() } },
                    new CultureInfo("en") { DateTimeFormat = { Calendar = new GregorianCalendar() } }
            };
        });


        services.AddMvc(option =>
        {
            option.Filters.Add(new CustomFilter());
            option.Filters.Add(new AuthorizeFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()));
            option.EnableEndpointRouting = false;
        }).AddNewtonsoftJson(opt => opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

        ConfigureDatabase<TDbContext, YAppUser, ZAppRole>(services, Configuration);
        ConfigureOAuthuntication(services);
    }

    public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        var locOptions = app.ApplicationServices.GetService<Microsoft.Extensions.Options.IOptions<RequestLocalizationOptions>>();

        app.UseRequestLocalization(locOptions.Value);

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseCors(x => x
           .AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader());

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseMiddleware<UserMiddleware>();

        app.UseStaticFiles();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.UseMvc();


    }

    static Func<RedirectContext<CookieAuthenticationOptions>, Task> ReplaceRedirector(HttpStatusCode statusCode, Func<RedirectContext<CookieAuthenticationOptions>, Task> existingRedirector) =>
    context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = (int)statusCode;
            return Task.CompletedTask;
        }
        return existingRedirector(context);
    };

    private static void ConfigureOAuthuntication(IServiceCollection services)
    {
        var _Configuration = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json").Build();
        var secOpts = _Configuration.GetSection("TokenSettings").Get<TokenSettings>();
        services.AddSingleton(typeof(TokenSettings), secOpts);
        var notificationsHubEvents = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                // If the request is for our hub...
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/ChatHub")))
                {
                    // Read the token out of the query string
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            },

        };

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer("Bearer", options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;

            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = false,
                ValidateLifetime = false,
                ValidIssuer = secOpts.Issuer,
                ValidAudience = secOpts.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secOpts.SecretKey))
            };
            options.Events = notificationsHubEvents;
        });
    }

    private static void ConfigureDatabase<TDbContext, YAppUser, ZAppRole>(IServiceCollection services, IConfiguration Configuration) where TDbContext : DbContext where YAppUser : class where ZAppRole : class
    {
        services.AddDbContext<TDbContext>(options =>
            //  options.UseLazyLoadingProxies()
            options.UseSqlServer(Configuration.GetConnectionString("StoreManagementConnectionString")));

        services.AddIdentity<YAppUser, ZAppRole>()
            .AddEntityFrameworkStores<TDbContext>()
            .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.Events.OnRedirectToAccessDenied = ReplaceRedirector(HttpStatusCode.Forbidden, options.Events.OnRedirectToAccessDenied);
            options.Events.OnRedirectToLogin = ReplaceRedirector(HttpStatusCode.Unauthorized, options.Events.OnRedirectToLogin);
        });

        services.Configure<IdentityOptions>(options =>
        {
            // Default Password settings.
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 5;
            options.Password.RequiredUniqueChars = 1;
        });
    }
}

