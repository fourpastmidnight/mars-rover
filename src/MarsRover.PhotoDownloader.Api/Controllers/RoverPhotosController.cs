using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Mime;
using Ardalis.SmartEnum;
using MarsRover.PhotoDownload.Api.Extensions;
using MarsRover.PhotoDownload.Api.Models;
using MarsRover.PhotoDownloader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MarsRover.PhotoDownload.Api.Controllers
{
    [ApiController]
    [Route("api/rover-photos")]
    [Produces("application/json")]
    public class RoverPhotosController : Controller
    {
        public RoverPhotosController(IOptions<MarsRoverPhotoDownloadApiOptions> roverApiOptions) =>
            ImagesPath = roverApiOptions.Value.ImagesPath;

        private string ImagesPath { get; }

        public IActionResult GetRoverPhotos(
            [FromQuery, Range(0, int.MaxValue)] int skip = 0,
            [FromQuery, Range(0, int.MaxValue)] int take = 10)
        {
            var photoUrls = (
                    from rover in SmartEnum<Rover>.List
                    from dir in Directory.GetDirectories(Path.Combine(ImagesPath, rover.Name))
                    from filePath in Directory.EnumerateFiles(dir)
                    where !filePath.EndsWith("NoPhotos.txt")
                    let canonicalizedFilePath = filePath.Replace('\\', '/')
                    let relativeUrl = Url.Action(
                        nameof(DownloadRoverPhotos),
                        new
                        {
                            roverName = rover.Name,
                            date = canonicalizedFilePath.Substring(
                                canonicalizedFilePath.IndexOf(rover.Name, StringComparison.OrdinalIgnoreCase) + rover.Name.Length + 1,
                                10),
                            filename = canonicalizedFilePath.Substring(canonicalizedFilePath.LastIndexOf('/') + 1)
                        })
                    select $"{Request.Scheme}://{Request.Host.ToUriComponent()}{relativeUrl}"
                )
                .Skip(skip)
                .Take(take)
                .ToList();

            return photoUrls.Count > 0
                ? (IActionResult) Json(new PhotoUrls {Photos = photoUrls})
                : NoContent();
        }

        [Route("{roverName}/{date:datetime}")]
        public IActionResult GetRoverPhotosByDate([FromRoute] string roverName, [FromRoute] DateTime date)
        {
            if (!SmartEnum<Rover>.TryFromName(roverName, true, out _)) return NoContent();

            var roverDir = Path.Combine(ImagesPath, roverName);
            var roverDateDir = Path.Combine(roverDir, date.ToString("yyyy-MM-dd"));

            if (System.IO.File.Exists(Path.Combine(roverDateDir, "NoPhotos.txt"))) return NoContent();

            return Json(new PhotoUrls
            {
                Photos = Directory.EnumerateFiles(roverDateDir)
                    .Select(f => f.Replace('\\', '/'))
                    .Select(f =>
                        Url.Action(nameof(DownloadRoverPhotos), new
                        {
                            roverName,
                            date = date.ToString("yyyy-MM-dd"),
                            filename = f.Substring(f.LastIndexOf('/') + 1)
                        }))
                    .Select(ru => $"{Request.Scheme}://{Request.Host.ToUriComponent()}{ru}")
                    .ToList()
            });
        }

        [Route("{date:datetime}")]
        public IActionResult GetPhotosByDate(DateTime date)
        {
            var photoUrls = (
                    from rover in SmartEnum<Rover>.List
                    from dir in Directory.GetDirectories(Path.Combine(ImagesPath, rover.Name))
                    where dir.Split('\\', '/').Last() == date.ToString("yyyy-MM-dd")
                    from filePath in Directory.EnumerateFiles(dir)
                    where !filePath.EndsWith("NoPhotos.txt")
                    let canonicalizedFilePath = filePath.Replace('\\', '/')
                    let relativeUrl = Url.Action(
                        nameof(DownloadRoverPhotos),
                        new
                        {
                            roverName = rover.Name,
                            date = date.ToString("yyyy-MM-dd"),
                            filename = canonicalizedFilePath.Substring(canonicalizedFilePath.LastIndexOf('/') + 1)
                        })
                    select $"{Request.Scheme}://{Request.Host.ToUriComponent()}{relativeUrl}"
                )
                .ToList();

            return photoUrls.Count > 0
                ? (IActionResult) Json(new PhotoUrls {Photos = photoUrls})
                : NoContent();
        }

        [Route("{roverName}")]
        public IActionResult GetPhotosByRover(string roverName)
        {
            if (!SmartEnum<Rover>.TryFromName(roverName, true, out _)) return NoContent();

            var photoUrls = (
                    from dir in Directory.GetDirectories(Path.Combine(ImagesPath, roverName))
                    from filePath in Directory.EnumerateFiles(dir)
                    where !filePath.EndsWith("NoPhotos.txt")
                    let canonicalizedFilePath = filePath.Replace('\\', '/')
                    let relativeUrl = Url.Action(
                        nameof(DownloadRoverPhotos),
                        new
                        {
                            roverName,
                            date = dir.Split('\\', '/').Last(),
                            filename = canonicalizedFilePath.Substring(canonicalizedFilePath.LastIndexOf('/') + 1)
                        })
                    select $"{Request.Scheme}://{Request.Host.ToUriComponent()}{relativeUrl}"
                )
                .ToList();

            return photoUrls.Count > 0
                ? (IActionResult) Json(new PhotoUrls {Photos = photoUrls})
                : NoContent();
        }

        [Route("download/{roverName}/{date:datetime}/{filename}")]
        public ActionResult DownloadRoverPhotos(string roverName, DateTime date, string filename)
        {
            var relativePath = Path.Combine(ImagesPath, roverName, date.ToString("yyyy-MM-dd"), filename);
            var contentType = Path.GetExtension(filename) switch
            {
                ".jpg" => MediaTypeNames.Image.Jpeg,
                ".jpeg" => MediaTypeNames.Image.Jpeg,
                ".gif" => MediaTypeNames.Image.Gif,
                ".tiff" => MediaTypeNames.Image.Tiff,
                _ => MediaTypeNames.Application.Octet
            };

            if (!System.IO.File.Exists(relativePath)) return NoContent();

            return PhysicalFile(Path.GetFullPath(relativePath), contentType);
        }
    }
}
