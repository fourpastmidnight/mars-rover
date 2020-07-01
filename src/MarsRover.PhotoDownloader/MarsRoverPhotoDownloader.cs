using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MarsRover.PhotoDownloader.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace MarsRover.PhotoDownloader
{
    public sealed class MarsRoverPhotoDownloader : IDisposable
    {
        private const string DemoKey = "DEMO_KEY";

        private readonly string _apiKey;
        private readonly string _imageCacheLocation;
        private HttpClient _httpClient;

        public MarsRoverPhotoDownloader(IOptions<MarsRoverPhotoDownloaderOptions> options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrWhiteSpace(options.Value.ImageCacheLocation))
            {
                throw new ArgumentException("ImageCacheLocation must have a value.");
            }

            _apiKey = options.Value.ApiKey ?? DemoKey;
            _imageCacheLocation = Path.GetFullPath(options.Value.ImageCacheLocation);

            if (!Directory.Exists(_imageCacheLocation))
            {
                Directory.CreateDirectory(_imageCacheLocation);
            }

            _httpClient = new HttpClient();
        }

        public async Task DownloadPhotos(Rover rover, IEnumerable<DateTime> dates, CancellationToken token = default)
        {
            var roverDir = Path.Combine(_imageCacheLocation, rover.Name);

            foreach (var date in dates)
            {
                var outputDir = Path.Combine(roverDir, date.ToString("yyyy-MM-dd"));
                if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);

                await (
                        from photoUrl in (await GetPhotoUrls(rover, date, token).ConfigureAwait(false))
                        let path = Path.Combine(outputDir, photoUrl.Substring(photoUrl.LastIndexOf('/') + 1))
                        where !File.Exists(path)
                        select new { DownloadUrl = photoUrl, Path = path }
                    )
                    .ForEachAsync(Environment.ProcessorCount, async pd =>
                    {
                        await _httpClient.GetByteArrayAsync(pd.DownloadUrl)
                            .ContinueWith(async bytes => await File.WriteAllBytesAsync(pd.Path, bytes.Result, token), token)
                            .Unwrap().ConfigureAwait(false);
                    });

                if (!Directory.EnumerateFileSystemEntries(outputDir).Any()) File.Create(Path.Combine(outputDir, "NoPhotos.txt"));
            }
        }

        public async Task DownloadPhotos(IEnumerable<Rover> rovers, IEnumerable<DateTime> dates, CancellationToken token = default)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            // ! dates is not enumerated over twice...
            foreach (var rover in rovers) await DownloadPhotos(rover, dates, token);
        }

        public async Task<List<string>> GetPhotoUrls(Rover rover, DateTime date, CancellationToken token = default)
        {
            var url = $"https://api.nasa.gov/mars-photos/api/v1/rovers/{rover.Name}/photos?earth_date={date:yyyy-MM-dd}&api_key={_apiKey}";

            var response = await _httpClient.GetAsync(url, token).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode) return Enumerable.Empty<string>().ToList();

            token.ThrowIfCancellationRequested();

            var photos = JObject.Parse(await response.Content.ReadAsStringAsync())["photos"];

            token.ThrowIfCancellationRequested();

            return (from p in photos select p["img_src"]?.ToString() ?? "").Distinct().ToList();
        }

        public void Dispose()
        {
            if (_httpClient == null) return;

            _httpClient.Dispose();
            _httpClient = null!;

        }
    }
}
