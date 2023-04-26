# Introduction
A few years back, I created [FileSorter](https://github.com/jacobrobertjohnson/FileSorter) for the purpose of pulling years' worth of family photos out of old hard drive dumps. With that done, the next step was to build a system for keeping the photos and videos organized going forward. The main project goals were to:

- Provide a scheduled job that watches a drop folder for new photos and videos
- Isolate photos & videos between different family units
- Store photos & videos on the filesystem, but maintain a library that can be used for duplicate-checking and future access via a [web interface](https://github.com/jacobrobertjohnson/PhotoSite)

**PhotoImporter** is a console application written in C# on .NET 6. It is intended to run as a scheduled task (I'm using `cron` on Ubuntu Server 22.04). To allow the same executable to run for multiple family units, the application is called with the path to a config file detailing the drop folder and library locations.

# Usage
The executable is called with an argument providing the path to a JSON configuration file:

**Linux:**
```bash
./PhotoImporter --configFile /path/to/config.json
```

The configuration file is stored in the following JSON format:

```JSON
{
    "SourceDir": "/path/to/importer/folder",
    "SourceFilePattern": "jpg,jpeg,png,bmp",
    "DatabasePath": "/path/to/Photos.db",
    "StoragePath": "/path/to/storage/folder",
    "VerboseOutput": false
}
```

|Property|Description|Created Automatically?|
|---|---|---|
|`SourceDir`|Path to the directory in which the program should search for photos to import.|No|
|`SourceFilePattern`|Comma-delimited list of file extensions to include in the import.|N/A|
|`DatabasePath`|Path to a [Sqlite](https://sqlite.org/index.html) database file that will be used for storing the image library.|Yes|
|`StoragePath`|Path to the folder into which images & videos will be moved after importing.|Yes|
|`VerboseOutput`|If set to `true`, the result of every image & video file will be written to the console. This slows down processing, so it is recommended that this remain `false` unless you are troubleshooting.|N/A|

Once imported, a random GUID is generated for the file, and its "Date Taken" is read from its EXIF tags if possible. If not, the File Creation date is used instead. These values are used to create a destination path using the following format:

```
/{config.StoragePath}/{file.DateTaken:yyyy-MM}/{file.DateTaken:yyyy-MM-dd}_{randomGuid}.{ext}
```

The random GUID and Date Taken are also stored in the Sqlite database specified in the config's `DatabasePath` property.

# Sample Setup Scenario
Let's say an individual has ended up managing the photo collections for three different sets of relatives:

- The Brown family
- The Jones family
- The Smith family

They want a similar storage setup for each family, but they want their photos to be cleanly isolated from one another.

To make this happen, they can set up Importer & Library folders for each family:

```
ContentLibrary/
├─ Importers/
│  ├─ Brown_Photos/
│  ├─ Jones_Photos/
│  ├─ Smith_Photos/
├─ Libraries/
│  ├─ Brown/
│  │  ├─ Photos/ 
│  │  ├─ config.json 
│  │  ├─ Photos.db 
│  ├─ Jones/
│  │  ├─ Photos/ 
│  │  ├─ config.json 
│  │  ├─ Photos.db
│  ├─ Smith/
│  │  ├─ Photos/ 
│  │  ├─ config.json 
│  │  ├─ Photos.db
```

Three different scheduled tasks can then be set up, each pointing to a different family's `config.json` file:

```bash
./PhotoImporter --configFile /ContentLibrary/Libraries/Brown/config.json
./PhotoImporter --configFile /ContentLibrary/Libraries/Jones/config.json
./PhotoImporter --configFile /ContentLibrary/Libraries/Smith/config.json
```

This folder structure is entirely flexible. Each piece can sit anywhere, as long as it is configured in the `.json` file that is provided to the importer.

# Development Environment
This project was built & tested on Kubuntu 23.04, and runs in "Production" on Ubuntu Server 22.04. Since it's built in .NET 6, it should also run on Windows & macOS, but I have not tested it there.