# Enchilada

[![Build Status](https://ci.appveyor.com/api/projects/status/github/sparkeh9/coreftp?branch=master&svg=true)](https://ci.appveyor.com/api/projects/status/github/sparkeh9/enchilada?branch=master&svg=true)
Enchilada is a filesystem abstraction layer written in C#, the aim is to enable 
the seamless use of file operations over and between different providers.

Implemented:
- Local Filesystem - (local)
- Azure Blob Storage - (azure-blob)

Planned:
- FTP/S (FTP over SSL)
- SCP (Secure Copy)

##Usage##
To reference a file, simply inject the filesystem resolver (IEnchiladaFilesystemResolver) into your code, which will normally be a single instance of Enchilada.Infrastructure.EnchiladaFileProviderResolver.
Once injected, simply supply a URI (as below) to OpenFileReference, this will produce an instance of IFile, which represents the file on whatever platform your configuration specifies, regardless of whether it exists yet or not.

```
fileSystemResolver.OpenFileReference( "enchilada://blob_storage/image.jpg" );
```

The URI is made up of three parts:
- The scheme: Simply by convention this is normally enchilada://, but any such scheme can be specified
- The provider name: This mirrors the configurations you have specified in the appsettings file. It can be anything which looks like a valid URI hostname, however it must have a corresponding configuration.
- The path: as you might imagine, this is the path to the file.

##Local##
The local adapter allows the resolution of files on the local filesystem or UNC path. 
It does not currently handle connecting to resources which require authentication.
Configuration is as follows
```
"your_configuration_name": {
	"adapter": "local",
	"directory": "C:\\my-folder"
}
```

##Azure Blob##
The azure blob (Binary Large OBject) adapter allows the resolution of files on the azure service.
Authentication is handled via the connection string.
```
"your_configuration_name": {
	"adapter": "azure-blob",
	"connectionString": "UseDevelopmentStorage=true;",
	"containerReference": "test",
	"createContainer": true,
	"isPublicAccess": true
}
```

## AspNetCore configuration ##
Enchilada.AspNetCore comes with functionality to plumb your app settings configuration, straight
into the AspNetCore Dependency Injection framework. To configure, simply amend the `ConfigureServices` method 
in the startup as follows.

```
services.AddEnchilada( new EnchiladaBuilderOptions
                        {
                            Adapters = Configuration.GetSection( "Enchilada:Adapters" )
                        } );
```

And provide configuration in the appsettings file, e.g.
```
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