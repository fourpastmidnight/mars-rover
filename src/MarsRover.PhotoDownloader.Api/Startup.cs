using MarsRover.PhotoDownload.Api;
using MarsRover.PhotoDownloader.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MarsRover.PhotoDownloader.Api
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
            // TODO: Move to an AddMarsRoverPhotoDownloader() extension method which registers the photo downloader alongith its configuration in the DI container.
            services.Configure<MarsRoverPhotoDownloaderOptions>(Configuration.GetSection("MarsRover.PhotoDownloader.Settings"));
            services.AddAsyncInitializer<MarsRoverPhotosCacheInitializer>();

            services.AddSingleton<MarsRoverPhotoDownloader>();
            services.AddControllers()
                           .AddNewtonsoftJson();


            services.AddLogging();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
