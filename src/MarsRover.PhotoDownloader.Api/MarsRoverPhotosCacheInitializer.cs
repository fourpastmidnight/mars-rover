using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Extensions.Hosting.AsyncInitialization;
using MarsRover.PhotoDownload.Api.Extensions;
using MarsRover.PhotoDownloader;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MarsRover.PhotoDownload.Api
{
    public sealed class MarsRoverPhotosCacheInitializer : IAsyncInitializer
    {
        private readonly ILogger _logger;
        private readonly MarsRoverPhotoDownloader _downloader;
        private readonly MarsRoverPhotoDownloadApiOptions _settings;

        public MarsRoverPhotosCacheInitializer(MarsRoverPhotoDownloader downloader, IOptions<MarsRoverPhotoDownloadApiOptions> apiSettings, ILogger<MarsRoverPhotosCacheInitializer> logger)
        {
            _downloader = downloader;
            _settings = apiSettings.Value;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                _logger.LogInformation("Initializing MarsRover.PhotoDownload.Api ...");

                var datesFileAbsolutePath = Path.GetFullPath(_settings.InitialDatesFilepath);

                _logger.LogInformation($"Retrieving dates from {datesFileAbsolutePath} for which to cache NASA Mars rover photos...");

                using var sr = new StreamReader(datesFileAbsolutePath);
                var dates = sr.ReadAllLines().ParseDates();

                _logger.LogInformation("Loading cache, this may take a few minutes ...");
                await _downloader.DownloadPhotos(new[] {Rover.Curiosity, Rover.Opportunity, Rover.Spirit}, dates);
                _logger.LogInformation("Cache loaded.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Initialization failed!");
                throw;
            }
        }
    }

    public static class RoverPhotoCacheInitializerHelpers
    {
        /// <summary>
        /// Given a sequence of strings, parses dates from the given strings. Strings in the
        /// sequence which cannot be parsed as a date are simply ignored.
        /// </summary>
        /// <param name="dateStrings">A sequence of strings from which to parse dates</param>
        /// <returns>
        /// A <see cref="List{DateTime}"/> for those strings which could be parsed as a date. If
        /// no strings could be parsed as a <see cref="DateTime"/>, an empty list is returned.
        /// </returns>
        public static List<DateTime> ParseDates(this IEnumerable<string> dateStrings)
        {
            return dateStrings
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Aggregate(
                    new List<DateTime>(),
                    (ds, d) =>
                    {
                        if (DateTime.TryParse(d, out var date)) ds.Add(date);
                        return ds;
                    });
        }

        public static IEnumerable<string> ReadAllLines(this StreamReader reader)
        {
            var lines = new List<string>();
            while (!reader.EndOfStream) lines.Add(reader.ReadLine());

            return lines;
        }
    }
}
