using Microsoft.Extensions.Configuration;

namespace MarsRover.PhotoDownload.Api.Extensions.Configuration
{
    internal class MarsRoverPhotoDownloadApiConfiguration
    {
        public MarsRoverPhotoDownloadApiConfiguration(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }
    }
}