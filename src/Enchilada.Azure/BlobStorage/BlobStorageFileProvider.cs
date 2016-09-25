namespace Enchilada.Azure.BlobStorage
{
    using System.IO;
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

        public void Dispose()
        {
            
        }
    }
}