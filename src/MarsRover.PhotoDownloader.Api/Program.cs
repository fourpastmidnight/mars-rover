using System.Threading.Tasks;
using MarsRover.PhotoDownload.Api.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace MarsRover.PhotoDownloader.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.InitAsync();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureMarsRoverPhotoDownloadApi((hostingContext, photoDownloadApi) =>
                {
                    photoDownloadApi.AddConfiguration(hostingContext.Configuration.GetSection("MarsRover.PhotoDownload.Api.Settings"));
                });
    }
}
