using StoreManagement.API;
using StoreManagement.Application.Interfaces;
using StoreManagement.Application.Services;
using StoreManagement.Domain.Entities;
using StoreManagement.Infrastructure;

namespace StoreManagement.Api
{

  public partial class Startup(IConfiguration configuration)
  {
        public IConfiguration Configuration { get; } = configuration;
    public void ConfigureServices(IServiceCollection services)
    {
      StoreManagement.Bases.Startup.ConfigureServices<StoreDbContext, ApplicationUser, ApplicationRole>(services, Configuration);
      StartupBuilder.ConfigurePackages(services);
      StartupBuilder.ConfigureServiceTypes(services);
      StartupBuilder.ConfigureRepositoriesTypes(services);
    }
    public async Task Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      StoreManagement.Bases.Startup.Configure(app, env);

      await SeedDatabase.Initialize(app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider);
    }
  }
}


