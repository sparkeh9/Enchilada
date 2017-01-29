namespace Enchilada.Azure.BlobStorage
{
    using System;
    using System.IO;
    using System.Linq;
    using Infrastructure.Extensions;
    using Infrastructure.Interface;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    public class BlobStorageFileProvider : IFileProvider
    {
        protected DirectoryInfo BackingRootDirectory;

        public IDirectory RootDirectory { get; protected set; }
        public IFile File { get; protected set; }

        public bool IsDirectory => !IsFile;
        public bool IsFile => File != null;

        public BlobStorageFileProvider( BlobStorageAdapterConfiguration configuration, string filePath )
        {
            if ( configuration.ContainerReference.Any( char.IsUpper ) )
            {
                throw new InvalidOperationException( "Blob container names can contain only lowercase letters, numbers, and hyphens, and must begin" +
                                                     " and end with a letter or a number. The name can't contain two consecutive hyphens." );
            }

            bool isDirectory = filePath.EndsWith( "/" );
            string blobPath = filePath.StripLeadingSlash();
            bool isRootContainer = blobPath.IsNullOrWhiteSpace();

            var account = CloudStorageAccount.Parse( configuration.ConnectionString );
            var blobClient = account.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference( configuration.ContainerReference );

            if ( configuration.CreateContainer )
            {
                container.CreateIfNotExistsAsync().Wait();

                container.SetPermissionsAsync( new BlobContainerPermissions
                {
                    PublicAccess = configuration.IsPublicAccess ? BlobContainerPublicAccessType.Container : BlobContainerPublicAccessType.Off,
                } ).Wait();
            }

            if ( isRootContainer )
            {
                RootDirectory = new BlobStorageDirectory( container, "/" );
                return;
            }

            if ( isDirectory )
            {
                RootDirectory = new BlobStorageDirectory( container, blobPath );
                return;
            }

            RootDirectory = new BlobStorageDirectory( container, blobPath.RemoveFilename() );
            File = RootDirectory.GetFile( blobPath );
        }

        public void Dispose() {}
    }
}