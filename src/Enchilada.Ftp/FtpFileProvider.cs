namespace Enchilada.Ftp
{
    using System.IO;
    using CoreFtp;
    using Infrastructure.Extensions;
    using Infrastructure.Interface;

    public class FtpFileProvider : IFileProvider
    {
        private readonly char[] DirectorySeparators = { '/', '\\' };
        protected DirectoryInfo BackingRootDirectory;
        private readonly FtpClient ftpClient;

        public IDirectory RootDirectory { get; protected set; }
        public IFile File { get; protected set; }

        public bool IsDirectory => !IsFile;
        public bool IsFile => File != null;

        public FtpFileProvider( FtpAdapterConfiguration configuration, string filePath )
        {
            bool isDirectory = filePath.EndsWith( "/" );
            string blobPath = filePath.StripLeadingSlash();
            bool isRootContainer = blobPath.IsNullOrWhiteSpace();


            ftpClient = new FtpClient( new FtpClientConfiguration
                                       {
                                           Host = configuration.Host,
                                           Port = configuration.Port,
                                           Username = configuration.Username,
                                           Password = configuration.Password,
                                           BaseDirectory = configuration.Directory
                                       } );


//
//            var account = CloudStorageAccount.Parse( configuration.ConnectionString );
//            var blobClient = account.CreateCloudBlobClient();
//            var container = blobClient.GetContainerReference( configuration.ContainerReference );
//
//            if ( configuration.CreateContainer )
//            {
//                container.CreateIfNotExistsAsync().Wait();
//
//                container.SetPermissionsAsync( new BlobContainerPermissions
//                                               {
//                                                   PublicAccess = configuration.IsPublicAccess ? BlobContainerPublicAccessType.Container : BlobContainerPublicAccessType.Off,
//                                               } ).Wait();
//            }
//
//            if ( isRootContainer )
//            {
//                RootDirectory = new BlobStorageDirectory( container, "/" );
//                return;
//            }
//
//            if ( isDirectory )
//            {
//                RootDirectory = new BlobStorageDirectory( container, blobPath );
//                return;
//            }
//
//            RootDirectory = new BlobStorageDirectory( container, blobPath.RemoveFilename() );
//            File = RootDirectory.GetFile( blobPath );
        }

        public void Dispose()
        {
            ftpClient.Dispose();
        }
    }
}