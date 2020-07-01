using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using MarsRover.PhotoDownload.Api.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MarsRover.PhotoDownload.Api.Controllers
{
    [Route("api/rover-photos")]
    [ApiController]
    public class RoverPhotosController : ControllerBase
    {
        IOptions<MarsRoverPhotoDownloadApiOptions> _roverApiOptions;

        public RoverPhotosController(IOptions<MarsRoverPhotoDownloadApiOptions> roverApiOptions)
        {
            _roverApiOptions = roverApiOptions;
        }

        public ActionResult GetRoverPhotos([FromQuery]uint skip = 0, [FromQuery]uint take = 10)
        {
            var skipping = skip * take;
            var first = (skip * take) + 1;
            var last = (skip * take) + take;
            return Content(@$"Initial date file path: {_roverApiOptions.Value.InitialDatesFilepath}
Skipping {skipping} photos and getting photos {first} through {last}...");
        }

        [Route("{rover}/{date:datetime}")]
        public ActionResult GetRoverPhotosByDate(Rover rover, DateTime date)
        {
            // TODO: Return JSONResult containing an array of available photo URLs for a given rover on a given date
            // If there are no photos stored locally:
            //   1. query the NASA API and obtain the list of URLS
            //   2. If there are any photos available for the selected date and rover:
            //      a. Transform the list of URLs to the list of local URLs where the photos can be obtained by this api
            //         - return this list
            //      b. Start a task that will begin retrieving the photos via the NASA API, storing them locally for retrieval
            //   3. If no photos are available for the selected date and rover, return a 204 No Content.
            return Content("No photos taken by this rover on that date.");
        }

        [Route("{date:datetime}")]
        public ActionResult GetPhotosByDate(DateTime date)
        {
            // TODO: Return JSONResult containing an array of available photo URLs for a given date
            // If there are no photos stored locally for any rover on the given date:
            //   1. query the NASA API and obtain the list of URLS
            //   2. If there are any photos available for the selected date and rover:
            //      a. Transform the list of URLs to the list of local URLs where the photos can be obtained by this api
            //         - return this list
            //      b. Start a task that will begin retrieving the photos via the NASA API, storing them locally for retrieval
            //   3. If no photos are available for the selected date and rover, return a 204 No Content.
            return Content("No photos were taken on this date.");
        }

        [Route("{rover}")]
        public ActionResult GetPhotosByRover(Rover rover)
        {
            // TODO: Return JSONResult containing an array of available photo URLs for a given date
            // If there are no photos stored locally for a given rover, return 204 No Content
            // Otherwise, return a list of available photo urls.
            // NOTE: This call does not attempt to go out and retrieve any photos from NASA's API, as their api
            // requires both a rover AND a date and there are thousands of photos available.
            // Also, this application does not attempt to query the rover's mission manifest to know which dates are valid for a given rover.
            return Content("Nothing to see here.");
        }

        [Route("download/{rover}/{date:datetime}/{filename}")]
        public ActionResult DownloadRoverPhotos(Rover rover, DateTime date, string filename)
        {
            // Retrieves a cached photo by rover, date, and filename.
            return Content("Not implemented yet.");
        }

        // TODO: Put all of this stuff in a testable, self-contained class...
        private readonly HttpClient _httpClient;


    }
}
