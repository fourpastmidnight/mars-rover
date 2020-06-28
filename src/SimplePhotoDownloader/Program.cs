using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SimplePhotoDownloader
{
    class Program
    {
        private static DateTime s_date;
        private static string s_apiKey = "DEMO_KEY";

        static async Task<int> Main(string[] args)
        {
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

            var photoUrls = await GetPhotoUrls("curiosity", s_date, s_apiKey);
            Console.WriteLine($"Found {photoUrls.Count} photos:");
            photoUrls.ForEach(u => Console.WriteLine($"  {u}"));
            Console.WriteLine();

            return 0;
        }

        public static async Task<List<string>> GetPhotoUrls(string rover, DateTime date, string apiKey)
        {
            HttpClient _httpClient = new HttpClient();

            var response = await _httpClient.GetAsync($"https://api.nasa.gov/mars-photos/api/v1/rovers/{rover}/photos?earth_date={date:yyyy-MM-dd}&api_key={apiKey}").ConfigureAwait(false);

            if (!response.IsSuccessStatusCode) return Enumerable.Empty<string>().ToList();

            var photos = JObject.Parse(await response.Content.ReadAsStringAsync())["photos"];
            return (from p in photos select p["img_src"]?.ToString() ?? "").Distinct().ToList();
        }

        public static void ShowHelp()
        {
            Console.WriteLine(@"
SimplePhotoDownloader v0.2.0

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

    --help                   Shows this help.

");
        }
    }
}
