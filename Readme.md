# Enchilada

Enchilada is a filesystem abstraction layer written in C#, the aim is to enable 
the seamless use of file operations over and between different providers.

Implemented:
- Local Filesystem
- Azure Blob Storage

Planned:
- FTP/S (FTP over SSL)
- SFTP (FTP over SSH)

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