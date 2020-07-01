using Microsoft.Extensions.DependencyInjection;

namespace MarsRover.PhotoDownload.Api.Extensions
{
    public interface IMarsRoverPhotoDownloadApiBuilder
    {
        public IServiceCollection Services { get; }
    }
}