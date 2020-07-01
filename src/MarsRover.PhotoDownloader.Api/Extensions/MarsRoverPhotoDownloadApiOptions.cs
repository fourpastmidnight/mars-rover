using Microsoft.Extensions.Options;

namespace MarsRover.PhotoDownload.Api.Extensions
{
    public sealed class MarsRoverPhotoDownloadApiOptions
    {
        /// <summary>
        /// Gets or sets the location of a line-delimited text file containing dates
        /// to be used to setup an initial cache of photos for this API.
        /// </summary>
        public string InitialDatesFilepath { get; set; }

        /// <summary>
        /// Gets or sets the path from where NASA Mars Rover photos can be retrieved.
        /// </summary>
        public string ImagesPath { get; set; }
    }

    internal sealed class DefaultMarsRoverPhotoDownloadApiOptions : ConfigureOptions<MarsRoverPhotoDownloadApiOptions>
    {
        public DefaultMarsRoverPhotoDownloadApiOptions(string initialDatesFilepath)
            : base(options => options.InitialDatesFilepath = initialDatesFilepath)
        {
        }
    }
}
