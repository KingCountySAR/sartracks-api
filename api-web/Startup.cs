using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SarData.Api.Data;
using System.IO;

namespace SarData.Api
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    private bool isSqlite = false;

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      string dbConnectionString = Configuration.GetValue<string>("Store:ConnectionString");
      services.AddDbContext<StoreContext>(options =>
      {
        if (dbConnectionString.ToLowerInvariant().StartsWith("filename="))
        {
          isSqlite = true;
          options.UseSqlite(dbConnectionString);
        }
        else
        {
          options.UseSqlServer(dbConnectionString);
        }
      });

      services.AddSingleton<DataSeeder>();

      services.AddMvc();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, DataSeeder seeder)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      
      using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
      {
        var context = serviceScope.ServiceProvider.GetRequiredService<StoreContext>();
        if (isSqlite)
        {
          context.Database.EnsureCreated();
        }

        seeder.Seed(context, env.ContentRootFileProvider.GetFileInfo("seed.json"));
      }

      app.UseMvc();
    }
  }
}
