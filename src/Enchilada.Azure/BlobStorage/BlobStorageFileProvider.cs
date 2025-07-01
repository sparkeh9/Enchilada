namespace Enchilada.Azure.BlobStorage
{
    using System;
    using System.IO;
    using System.Linq;
    using Infrastructure.Extensions;
    using Infrastructure.Interface;
    using global::Azure.Storage.Blobs;
    using global::Azure.Storage.Blobs.Models;

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

            var blobServiceClient = new BlobServiceClient( configuration.ConnectionString );
            var container = blobServiceClient.GetBlobContainerClient( configuration.ContainerReference );

            if ( configuration.CreateContainer )
            {
                container.CreateIfNotExists();

                var accessType = configuration.IsPublicAccess ? PublicAccessType.BlobContainer : PublicAccessType.None;
                container.SetAccessPolicy( accessType );
            }

            if ( isRootContainer )
            {
                RootDirectory = new BlobStorageDirectory( container, "" );
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