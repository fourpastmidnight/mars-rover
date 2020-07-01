using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace MarsRover.PhotoDownloader.Extensions
{
    public sealed class MarsRoverPhotoDownloaderOptions
    {
        /// <summary>
        /// Don't set this in a configuration file. Use ASP.NET Core User Secrets
        /// management or an environment variable.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// The location where downloaded images should be stored.
        /// </summary>
        [Required]
        public string ImageCacheLocation { get; set; }
    }

    internal sealed class DefaultMarsRoverPhotoDownloaderOptions : ConfigureOptions<MarsRoverPhotoDownloaderOptions>
    {
        public DefaultMarsRoverPhotoDownloaderOptions() : base(options => options.ImageCacheLocation = null!)
        {
        }
    }
}