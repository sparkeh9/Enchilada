namespace Enchilada.Ftp
{
    using System.IO;
    using CoreFtp;
    using Infrastructure.Extensions;
    using Infrastructure.Interface;

    public class FtpFileProvider : IFileProvider
    {
        private readonly char[] DirectorySeparators = ['/', '\\'];
        protected DirectoryInfo BackingRootDirectory;
        private readonly FtpClient ftpClient;

        public IDirectory RootDirectory { get; protected set; }
        public IFile File { get; protected set; }

        public bool IsDirectory => !IsFile;
        public bool IsFile => File != null;

        public FtpFileProvider( FtpAdapterConfiguration configuration, string filePath )
        {
            bool isDirectory = filePath.EndsWith( "/" );
            string ftpPath = filePath.StripLeadingSlash();
            bool isRootContainer = ftpPath.IsNullOrWhiteSpace();


            ftpClient = new FtpClient( new FtpClientConfiguration
            {
                Host = configuration.Host,
                Port = configuration.Port,
                Username = configuration.Username,
                Password = configuration.Password,
                BaseDirectory = configuration.Directory,
                IpVersion = configuration.IpVersion,
                EncryptionType = configuration.EncryptionType,
                IgnoreCertificateErrors = configuration.IgnoreCertificateErrors,
                SslProtocols = configuration.SslProtocols
            } );

            if ( isRootContainer )
            {
                RootDirectory = new FtpDirectory( ftpClient, "/" );
                return;
            }

            if ( isDirectory )
            {
                RootDirectory = new FtpDirectory( ftpClient, ftpPath );
                return;
            }

            RootDirectory = new FtpDirectory( ftpClient, ftpPath.RemoveFilename() );
            File = RootDirectory.GetFile( ftpPath );
        }

        public void Dispose()
        {
            ftpClient.Dispose();
        }
    }
}