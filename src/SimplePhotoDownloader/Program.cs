using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SimplePhotoDownloader
{
    class Program
    {
        private static HttpClient s_httpClient = new HttpClient();
        private static DateTime s_date;
        private static string s_apiKey = "DEMO_KEY";
        private static string s_output;
        private static CancellationTokenSource cts = new CancellationTokenSource();

        static async Task<int> Main(string[] args)
        {
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = false;
                cts.Cancel();
            };

            if (args.Length == 0 || (args.Length == 1 && args[0].Equals("--help", StringComparison.OrdinalIgnoreCase)))
            {
                ShowHelp();
                return 1;
            }

            var argsList = args.ToList();

            var dateArgIndex = argsList.FindIndex(a => a.Equals("--date", StringComparison.OrdinalIgnoreCase));
            if (dateArgIndex == -1)
            {
                ShowHelp();
                return 1;
            }

            if ((argsList.Count - 1) == dateArgIndex)
            {
                Console.WriteLine("ERROR: A valid date was not provided for --date.");
                ShowHelp();
                return 1;
            }

            if (!DateTime.TryParse(argsList[dateArgIndex + 1], out s_date))
            {
                Console.WriteLine("ERROR: Unable to parse the value provided for --date as a date.");
                ShowHelp();
                return 1;
            }

            var apiKeyIndex = argsList.FindIndex(a => a.Equals("--api-key", StringComparison.OrdinalIgnoreCase));
            if (apiKeyIndex > -1 && argsList.Count -1 != apiKeyIndex)
            {
                if (argsList[apiKeyIndex + 1].Equals("--date", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("ERROR: No value for --api-key was specified.");
                    ShowHelp();
                    return 1;
                }

                s_apiKey = argsList[apiKeyIndex + 1];
            }

            s_output = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), $"MarsRoverPhotos");
            var outputArgIndex = argsList.FindIndex(a => a.Equals("--output", StringComparison.OrdinalIgnoreCase));
            if (outputArgIndex > -1 && (argsList.Count > outputArgIndex + 1) && !argsList[outputArgIndex + 1].StartsWith("--"))
            {
                s_output = argsList[outputArgIndex + 1];
            }

            if (!Directory.Exists(s_output))
            {
                try
                {
                    Directory.CreateDirectory(s_output);
                }
                catch
                {
                    Console.WriteLine("ERROR: An error occurred trying to create the directory:");
                    Console.WriteLine($"  '{s_output}'");
                    Console.WriteLine("\nPlease ensure the path is valid and that you have permission");
                    Console.WriteLine("to write to the specified location.\n");
                    return 1;
                }
            }

            try
            {
                await DownloadPhotos("Curiosity", s_date, s_apiKey, s_output, cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Downloading of photos was cancelled.");
                Console.WriteLine("NOTE: Some photos may already have been downloaded.");
                Console.WriteLine();
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: An error occurred while downloading photos:");
                Console.WriteLine($"    \"{ex.Message}\"");
                Console.WriteLine();
                return 1;
            }

            Console.WriteLine($"\nFinished downloading photos to '{s_output}'.\n");
            return 0;
        }

        public static async Task<List<string>> GetPhotoUrls(string rover, DateTime date, string apiKey, CancellationToken token = default)
        {
            var response = await s_httpClient.GetAsync($"https://api.nasa.gov/mars-photos/api/v1/rovers/{rover}/photos?earth_date={date:yyyy-MM-dd}&api_key={apiKey}", token).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode) return Enumerable.Empty<string>().ToList();

            token.ThrowIfCancellationRequested();
            var photos = JObject.Parse(await response.Content.ReadAsStringAsync())["photos"];

            token.ThrowIfCancellationRequested();
            return (from p in photos select p["img_src"]?.ToString() ?? "").Distinct().ToList();
        }

        public static async Task DownloadPhotos(string rover, DateTime date, string apiKey, string destinationFolder, CancellationToken token = default)
        {
            string outputDir = Path.Combine(destinationFolder, rover, date.ToString("yyyy-MM-dd"));
            if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);

            await (
                from photoUrl in (await GetPhotoUrls(rover, date, apiKey, token))
                let path = Path.Combine(outputDir, photoUrl.Substring(photoUrl.LastIndexOf('/') + 1))
                where !File.Exists(path)
                select new { DownloadUrl = photoUrl, Path = path }
            )
            .ForEachAsync(Environment.ProcessorCount, async pd =>
            {
                await s_httpClient.GetByteArrayAsync(pd.DownloadUrl)
                    .ContinueWith(async bytes => await File.WriteAllBytesAsync(pd.Path, bytes.Result, token), token)
                    .Unwrap();
            });
        }

        public static void ShowHelp()
        {
            Console.WriteLine(@"
SimplePhotoDownloader v0.3.0

  Downloads NASA Mars Curiosity rover photos for the specified date. Dates
  which cannot be parsed are simply ignored.

  usage: SimplePhotoDownloader --date <date> [--output <path>] [--api-key <key>]

    --api-key <key>          The API key to use when retrieving photos.
                             You can use DEMO_KEY, but it's rate-limited.

                             See https://api.nasa.gov to obtain an API
                             key.

    --date <date>            A date for which to retrieve photos.
                             Any format which can be parsed as a DateTime
                             by .NET Core can be used.

                             Example valid formats:
                               yyyy-M-d; yyyy-MM-dd; M/d/yyyy; MM/dd/yyyy
                               M/d/yy; MM/dd/yy; MMMM d, yyyy; MMMM dd, yyyy
                               MMM-d-yy; MMM-dd-yy; MMM-d-yyyy, MMM-dd-yyyy

    --output <path>          A path to where Mars rover photos should be
                             downloaded. Photos will be stored in sub-folders
                             by rover by date.

                             Default: ~/Pictures/MarsRoverPhotos

    --help                   Shows this help.

");
        }
    }

    public static class Helpers
    {
        // Taken from 'Concurrency in .NET' by Ricardo Terrell, Manning, 2018, p. 304
        public static Task ForEachAsync<T>(this IEnumerable<T> source, int maxDegreeOfParallelism, Func<T, Task> body)
        {
            return Task.WhenAll(
                from partition in Partitioner.Create(source).GetPartitions(maxDegreeOfParallelism)
                select Task.Run(async () =>
                {
                    using (partition)
                        while (partition.MoveNext())
                            await body(partition.Current);
                }));
        }
    }
}
