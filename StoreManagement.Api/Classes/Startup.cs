using StoreManagement.Api;
using StoreManagement.Application.Interfaces;
using StoreManagement.Application.Services;
using StoreManagement.Infrastructure;

namespace StoreManagement.API;


public static class StartupBuilder
{
    public static void ConfigureServiceTypes(IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ISubCategoryService, SubCategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IReportService, ReportService>();
    }

    public static void ConfigureRepositoriesTypes(IServiceCollection services)
    {
        #region Generic
        services.AddScoped(typeof(UnitOfWork<>), typeof(UnitOfWork<>));
        services.AddScoped<IUnitOfWork, UnitOfWork<StoreDbContext>>();
        services.AddScoped<IUnitOfWork<StoreDbContext>, UnitOfWork<StoreDbContext>>();
        #endregion


    }
    public static void ConfigurePackages(IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;
        });
    }
    public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>()
            .ConfigureLogging(loggingBuilder => loggingBuilder.ClearProviders());
        });
}

