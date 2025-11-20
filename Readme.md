# Enchilada
[![Build Status](https://github.com/sparkeh9/Enchilada/actions/workflows/build-test.yml/badge.svg)](https://github.com/sparkeh9/Enchilada/actions/workflows/build-test.yml)

## What is it?
Enchilada is a filesystem abstraction layer written in C#, the aim is to enable the seamless use of file operations over and between different providers.

Implemented:
- Local Filesystem - (local)
- Azure Blob Storage - (azure-blob)
- FTP/S - (ftp)

Planned:
- SCP (Secure Copy)
- AWS S3


## How to contribute
1. Fork
1. Hack!
1. Pull Request

### Nuget Packages
- https://www.nuget.org/packages/Enchilada/ - The main package, provides local filesystem support
- https://www.nuget.org/packages/Enchilada.Azure/ - Provides Azure blob storage
- https://www.nuget.org/packages/Enchilada.AspNetCore/ - Provides ASP.NET Core configuration support
- https://www.nuget.org/packages/Enchilada.Ftp/ - Provides FTP/S


## Usage

To reference a file, simply inject the filesystem resolver (`IEnchiladaFilesystemResolver`) into your code, which will normally be a single instance of `Enchilada.Infrastructure.EnchiladaFileProviderResolver`.
Once injected, simply pass in a URI (see below) to `IEnchiladaFilesystemResolver.OpenFileReference`, which will produce an instance of `IFile`, which represents the file on whatever platform your configuration specifies, regardless of whether it exists yet or not.
```C#
fileSystemResolver.OpenFileReference( "enchilada://blob_storage/image.jpg" );
```
The URI is made up of three parts:
- The scheme: Simply by convention this is normally `enchilada://`, but any such scheme can be specified
- The provider name: This mirrors the configurations you have specified in the appsettings file. It can be anything which looks like a valid URI hostname, however it must have a corresponding configuration.
- The path: as you might imagine, this is the path to the file.

### Saving a file from a stream

```C#
// Injected filesystem resolver
IEnchiladaFilesystemResolver enchilada;

var tempFile = new FileInfo( "C:\\test.png" );
using ( var filestream = tempFile.OpenReadStream() )
{
	using ( var fileReference = enchilada.OpenFileReference( filepath ) )
	{
	    await fileReference.CopyFromAsync( filestream );
	}
}
```

## Local
The local adapter allows the resolution of files on the local filesystem or UNC path. 
It does not currently handle connecting to resources which require authentication.
```json
"your_configuration_name": {
	"adapter": "local",
	"directory": "C:\\my-folder"
}
```

## Azure Blob
The Azure Blob (Binary Large Object) adapter allows the resolution of files on the azure service.
Authentication is handled via the connection string.
```json
"your_configuration_name": {
	"adapter": "azure-blob",
	"connectionString": "UseDevelopmentStorage=true;",
	"containerReference": "test",
	"createContainer": true,
	"isPublicAccess": true
}
```

## FTP
The FTP adapter enables non-encrypted file transfer to a passive mode FTP server.
```json
"your_configuration_name": {
	"adapter": "ftp",
	"host": "ftp.github.com",
	"port": "21",
	"directory": "/sub/folder",
	"username": "user@ftpserver.com",
	"password": "$up3r.$3cur3.p4$$w0rd"
}
```

## AspNetCore configuration
**Enchilada.AspNetCore** comes with functionality to plumb your app settings configuration, straight
into the AspNetCore Dependency Injection framework. To configure, simply amend the `ConfigureServices` method 
in `Startup.cs` as follows.

```C#
services.AddEnchilada( new EnchiladaBuilderOptions
                        {
                            Adapters = Configuration.GetSection( "Enchilada:Adapters" )
                        } );
```

And provide configuration in the appsettings file, e.g.
```json
{
  "Enchilada": {
    "Adapters": {
      "local_filesystem": {
        "adapter": "local",
        "directory": "C:\\my-folder"
      },
      "blob_storage": {
        "adapter": "azure-blob",
        "connectionString": "UseDevelopmentStorage=true;",
        "containerReference": "test",
        "createContainer": true,
        "isPublicAccess": true
      }
    }
  }
}
```

## Development and Releases

### Building and Testing
The project uses GitHub Actions for continuous integration. Every push to the `master` branch triggers a build and test workflow that includes:
- Building all projects
- Running integration tests with Docker containers for FTP and Azure Blob Storage

### Creating a Release
To create a new release and publish NuGet packages:

1. Create and push a tag with the format `release/v{version}`:
   ```bash
   git tag release/v1.1.4
   git push origin release/v1.1.4
   ```

2. The release workflow will automatically:
   - Extract the version from the tag name
   - Build all projects with the extracted version
   - Create NuGet packages
   - Publish packages to NuGet.org
   - Create a GitHub release with the packages attached

The version number from the tag is embedded into the assembly and NuGet packages.
