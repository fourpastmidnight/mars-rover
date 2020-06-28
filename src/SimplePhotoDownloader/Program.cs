using System;
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
        private static readonly HttpClient s_httpClient = new HttpClient();
        private static readonly string DemoApiKey = "DEMO_KEY";
        private static readonly CancellationTokenSource cts = new CancellationTokenSource();

        static async Task<int> Main(string[] args)
        {
            Console.CancelKeyPress += (s, e) =>
            {
                e.Cancel = false;
                cts.Cancel();
            };

            var argsList = args.ToList();

            if (argsList.HelpRequested())
            {
                ShowHelp();
                return 1;
            }

            List<DateTime> dates;
            var apiKey = DemoApiKey;
            var output = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "MarsRoverPhotos");

            // ! NOTE: I'm using an ArgumentException for simplicity in this application.
            // !       HOWEVER, this is BAD because it's using exceptions for control flow.
            // !       Given this is a small "demo app", I'm OK with it--it's throw away code.
            try
            {
                apiKey = argsList.GetApiKey(defaultValue: DemoApiKey);
                dates = argsList.GetDates();
                output = argsList.GetOutputPath(defaultValue: output);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("       Use --help to get help with using this program.");
                return 1;
            }

            if (!Directory.Exists(output))
            {
                try
                {
                    Directory.CreateDirectory(output);
                }
                catch
                {
                    Console.WriteLine("ERROR: An error occurred trying to create the directory:");
                    Console.WriteLine($"  '{output}'");
                    Console.WriteLine("\nPlease ensure the path is valid and that you have permission");
                    Console.WriteLine("to write to the specified location.\n");
                    return 1;
                }
            }

            try
            {
                await DownloadPhotos("Curiosity", dates, apiKey, output, cts.Token);
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

            Console.WriteLine($"\nFinished downloading photos to '{output}'.\n");
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

        public static async Task DownloadPhotos(string rover, List<DateTime> dates, string apiKey, string destinationFolder, CancellationToken token = default)
        {
            foreach (var date in dates)
            {
                var outputDir = Path.Combine(destinationFolder, rover, date.ToString("yyyy-MM-dd"));
                if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);

                await (
                        from photoUrl in (await GetPhotoUrls(rover, date, apiKey, token))
                        let path = Path.Combine(outputDir, photoUrl.Substring(photoUrl.LastIndexOf('/') + 1))
                        where !File.Exists(path)
                        select new {DownloadUrl = photoUrl, Path = path}
                    )
                    .ForEachAsync(Environment.ProcessorCount, async pd =>
                    {
                        await s_httpClient.GetByteArrayAsync(pd.DownloadUrl)
                            .ContinueWith(async bytes => await File.WriteAllBytesAsync(pd.Path, bytes.Result, token), token)
                            .Unwrap();
                    });
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine(@"
SimplePhotoDownloader v0.4.0

  Downloads NASA Mars Curiosity rover photos for the specified date. Dates
  which cannot be parsed are simply ignored.

  usage SimplePhotoDownloader.exe (--date <date> | --dates <path>)
                                  [--api-key <api-key>] [--output <path>]

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

    --dates <path>           Downloads photos for the NASA Mars rovers for the
                             dates listed in the file specified by <path>. The
                             file should consist of one date per line.

    --output <path>          A path to where Mars rover photos should be
                             downloaded. Photos will be stored in sub-folders
                             by rover by date.

                             Default: ~/Pictures/MarsRoverPhotos

    --help                   Shows this help.

");
        }
    }
}
