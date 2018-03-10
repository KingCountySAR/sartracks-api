using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace SarData.Api
{
  public class Program
  {
    public static void Main(string[] args)
    {
      BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
          //.ConfigureLogging((context, logBuilder) => {
          //  logBuilder.AddConfiguration(context.Configuration.GetSection("Logging"));
          //  if (context.HostingEnvironment.IsDevelopment())
          //  {
          //    logBuilder.AddDebug();
          //    logBuilder.AddConsole();
          //  }
          //})
            .UseStartup<Startup>()
            .Build();
  }
}
