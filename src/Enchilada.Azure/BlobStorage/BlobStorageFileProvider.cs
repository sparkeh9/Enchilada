namespace Enchilada.Azure.BlobStorage
{
    using System.IO;
    using Infrastructure.Interface;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    public class BlobStorageFileProvider : IFileProvider
    {
        private readonly char[] DirectorySeparators = { '/', '\\' };
        protected DirectoryInfo BackingRootDirectory;

        public IDirectory RootDirectory { get; protected set; }
        public IFile File { get; protected set; }

        public bool IsDirectory => !IsFile;
        public bool IsFile => File != null;

        public BlobStorageFileProvider( BlobStorageAdapterConfiguration configuration, string filePath )
        {
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

            var blockBlobReference = container.GetBlockBlobReference( filePath );
            if ( blockBlobReference.ExistsAsync().GetAwaiter().GetResult() )
            {
                RootDirectory = new BlobStorageDirectory( container, Path.GetFullPath( Path.Combine( filePath, "../" ) ) );
                File = RootDirectory.GetFile( Path.GetFileName( filePath ) );
            }
            else
            {
                RootDirectory = new BlobStorageDirectory( container, filePath );
            }
        }
    }
}