# NASA Mars Rover Photos

## Introduction

This is a simple application that allows you to download photos taken by
NASA's Mars rover Curiosity. Currently, it's a mimimal console application.

## Prerequisites

To build and run this application, you must have Visual Studio 2019 and .NET Core 3.1.

## Building SimplePhotoDownloader

1. Clone this repository and open `mars-rover.sln` in Visual Studio.
2. Build the solution

## Running SimplePhotoDownloader

As mentioned, _SimplePhotoDownloader_ is a console application with a very
minimal set of parameters:

```
SimplePhotoDownloader v0.1.0

  usage SimplePhotoDownloader.exe --date <date> [--api-key <api-key>]
                                  [--output <path>]

  Downloads NASA Mars Curiosity rover photos for the specified date. Dates
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

    --output <path>               Specify the destination folder where photos
                                  will be stored. If this argument is not spec-
                                  ified, on Windows photos will be stored at:

                                    %USERPROFILE%\Pictures\MarsRoverPhotos

                                  Photos will be organized by rover by date.

    --help                        Shows this help.
```

Currently, the application only downloads photos for the NASA Mars rover Curiosity.

## Known Issues / Limitations

1. Only a single date can be specified
2. The NASA Mars rover is hard-coded to Curiosity.
3. The application only lists the URLs for the photos and does not download them

## Planned Additional Features

* Allow dates to be listed in a file and read by the application to download photos for mulitple dates
* Allow photos to be retrieved for NASA Mars rovers Spirit and Opportunity.
* Put this feature set behind a Web API, which will act as an "intermediate cache"
    * Upon API startup, if configured with a file with a list of dates, retrieve NASA Mars rover
      photos for the specified dates and cache them.
    * Provide API endpoint for retrieving photos by rover by date
    * Provide API endpoint for retrieving all photos by date
    * Provide API endpoint for retrieving all photos by rover 
* Create a ReactJS front-end application that communicates with the Web API to view the cached
  photos.