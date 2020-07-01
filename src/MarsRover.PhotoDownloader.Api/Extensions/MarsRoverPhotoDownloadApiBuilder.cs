using Microsoft.Extensions.DependencyInjection;

namespace MarsRover.PhotoDownload.Api.Extensions
{
    internal sealed class MarsRoverPhotoDownloadApiBuilder : IMarsRoverPhotoDownloadApiBuilder
    {
        public MarsRoverPhotoDownloadApiBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}