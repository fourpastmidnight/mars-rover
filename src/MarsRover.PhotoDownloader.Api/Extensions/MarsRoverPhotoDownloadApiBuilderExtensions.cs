using MarsRover.PhotoDownload.Api.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace MarsRover.PhotoDownload.Api.Extensions
{
    public static class MarsRoverPhotoDownloadApiBuilderExtensions
    {
        public static IMarsRoverPhotoDownloadApiBuilder AddConfiguration(this IMarsRoverPhotoDownloadApiBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddSingleton<IConfigureOptions<MarsRoverPhotoDownloadApiOptions>>(new MarsRoverPhotoDownloadApiConfigureOptions(configuration));
            builder.Services.AddSingleton<IOptionsChangeTokenSource<MarsRoverPhotoDownloadApiOptions>>(new ConfigurationChangeTokenSource<MarsRoverPhotoDownloadApiOptions>(configuration));
            builder.Services.AddSingleton(new MarsRoverPhotoDownloadApiConfiguration(configuration));
            return builder;
        }
    }
}
