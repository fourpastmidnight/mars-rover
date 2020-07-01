# NASA Mars Rover Photos

## Mars Rover Photo Download API

This application creates a Web API that acts as an intermediary between the client using this API
and NASA's own provided Web API.

This application, if configured with a set of dates, will upon startup cache photos for the NASA
Mars Rovers on those selected dates. Clients using this API can then query for available photos
and optionally download them (e.g. for display in a client web application).

If no photos are cached for a specified rover and date, this API will query the NASA API for all
available photos for that rover on the specified date and return the list of URLs to the client,
while starting to download and cache those images in the background.

### Prerequisites

These instructions assume you are using Visual Studio 2019.

You must have Microsoft ASP.NET Core 3.1 installed in order to build and run this application.

You can use the default demonstration key that NASA provides, but this key is rate-limited. If
no key is provided, this demonstration key will be used by default. You can obtain your own key
for free by visiting https://api.nasa.gov.

If running this on your local machine in a development environment, use ASP.NET Core's
User Secrets management for managing the application's access to your API key:

```shell
dotnet user-secrets set "MarsRover.PhotoDownloader.Settings:ApiKey" "<YOUR-API-KEY>"
    --project src/MarsRover.PhotoDownload.Api
```

### Building MarsRover.PhotoDownload.Api

1. Clone this repository and open `mars-rover.sln` in Visual Studio.
2. Build the solution.

### Running MarsRover.PhotoDownload.Api

1. Ensure either the **MarsRover.PhotoDownload.Api** project OR **IIS Express**
   are selected as the launch configuration. (A Docker configuration exists but
   has not yet been tested or configured.)
2. Run the selected configuration.
3. Open your browser to https://localhost:5001/api/rover-photos?skip=0&take=10

### Notes about this Web API

Upon startup, the application will initialize itself by caching a number of
NASA Mars rover photos. To do this, it reads the `dates.txt` file and for each
of the NASA Mars rovers, queries for the photos on the dates. (Malformed dates
that may be present in the `dates.txt` file are simply ignored.) Once the
photo cache has been initialized, the Web API will begin accepting requests.

I have not yet implemented Swagger/OpenApi Spec. documentation.

## SimplePhotoDownloader

This is a simple application that allows you to download photos taken by the
NASA's Mars rovers Curiosity, Spirit and Opportunity.

### Prerequisites

To build and run this application, you must have .NET Core 3.1 installed.

### Building SimplePhotoDownloader

1. Clone this repository and open `mars-rover.sln` in Visual Studio.
2. Build the solution

### Running SimplePhotoDownloader

_SimplePhotoDownloader_ is a console application with a very minimal set of parameters:

```
SimplePhotoDownloader v0.5.0

  usage SimplePhotoDownloader.exe (--date <date> | --dates <path>)
                                  [--api-key <api-key>] [--output <path>]

  Downloads NASA Mars Curiosity, Spirit and Opportunity rover photos for the
  specified date. Dates which cannot be parsed are simply ignored.pecified date. Dates
  which cannot be parsed are simply ignored.

    --api-key <api-key>           Optional. Obtaining an API Key is free. If no
                                  <api-key> is specified, a demonstration key
                                  will be used. However, the demonstration key
                                  is rate-limited.

                                  See https://api.nasa.gov to obtain an API
                                  key.

    --date <date>                 Downloads photos for the NASA Mars rovers
                                  on the specified date. This argument is
                                  mutually exclusive with --dates

    --dates <path>                Downloads photos for the NASA Mars rovers
                                  for the dates listed in the file specified
                                  by <path>. The file should consist of one
                                  date per line.

    --output <path>               Specify the destination folder where photos
                                  will be stored. If this argument is not spec-
                                  ified, on Windows photos will be stored at:

                                    %USERPROFILE%\Pictures\MarsRoverPhotos

                                  Photos will be organized by rover by date.

    --help                        Shows this help.
```

### Planned Additional Features

* Generate Swagger/Swashbuckle/OpenApi Spec documentation for the Web API
* Complete the Dockerfile so that the WebAPI can be run in a Docker container
* Create a ReactJS front-end application that communicates with the Web API to view the cached
  photos.