using MarsRover.PhotoDownload.Api.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MarsRover.PhotoDownloader.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
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
