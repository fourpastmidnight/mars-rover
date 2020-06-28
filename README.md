# NASA Mars Rover Photos

## Introduction

This is a simple application that allows you to download photos taken by the
NASA's Mars rovers Curiosity, Spirit and Opportunity.

## Prerequisites

To build and run this application, you must have .NET Core 3.1 installed.

## Building SimplePhotoDownloader

1. Clone this repository and open `mars-rover.sln` in Visual Studio.
2. Build the solution

## Running SimplePhotoDownloader

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

## Planned Additional Features

* Put this feature set behind a Web API, which will act as an "intermediate cache"
    * Upon API startup, if configured with a file with a list of dates, retrieve NASA Mars rover
      photos for the specified dates and cache them.
    * Provide API endpoint for retrieving photos by rover by date
    * Provide API endpoint for retrieving all photos by date
    * Provide API endpoint for retrieving all photos by rover 
* Create a ReactJS front-end application that communicates with the Web API to view the cached
  photos.