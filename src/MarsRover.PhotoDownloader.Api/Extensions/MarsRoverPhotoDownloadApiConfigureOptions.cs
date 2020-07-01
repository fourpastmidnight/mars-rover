using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace MarsRover.PhotoDownload.Api.Extensions
{
    internal sealed class MarsRoverPhotoDownloadApiConfigureOptions : IConfigureOptions<MarsRoverPhotoDownloadApiOptions>
    {
        private const string InitialDatesFilePathKey = "InitialDatesFilePath";
        private const string ImagesPath = "ImagesPath";

        private readonly IConfiguration _configuration;

        public MarsRoverPhotoDownloadApiConfigureOptions(IConfiguration configuration) => _configuration = configuration;

        public void Configure(MarsRoverPhotoDownloadApiOptions options)
        {
            LoadDefaultConfigValues(options);
        }

        private void LoadDefaultConfigValues(MarsRoverPhotoDownloadApiOptions options)
        {
            if (_configuration == null) return;

            options.InitialDatesFilepath = _configuration.GetValue<string>(InitialDatesFilePathKey, null);
            options.ImagesPath = _configuration.GetValue(ImagesPath, "./RoverPhotos");
        }
    }
}