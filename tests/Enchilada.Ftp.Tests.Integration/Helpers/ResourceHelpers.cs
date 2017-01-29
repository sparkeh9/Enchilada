namespace Enchilada.Ftp.Tests.Integration.Helpers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using CoreFtp;
    using Microsoft.Extensions.Logging;

    public static class ResourceHelpers
    {
        private static FtpClient ftpClient;

        public static FtpClient GetLocalFtpClient( ILogger logger, string baseDirectory = "/" )
        {
            Program.Initialise();

            ftpClient = new FtpClient( new FtpClientConfiguration
            {
                Host = Program.FtpConfiguration.Host,
                Username = Program.FtpConfiguration.Username,
                Password = Program.FtpConfiguration.Password,
                Port = Program.FtpConfiguration.Port,
                BaseDirectory = baseDirectory
            } ) { Logger = logger };
            return ftpClient;
        }

        public static async Task<FtpFile> CreateFileWithContentAsync( string filename, string content, ILogger logger )
        {
            using ( var client = GetLocalFtpClient( logger ) )
            {
                var ftpFile = new FtpFile( client, filename );

                using ( var stream = await ftpFile.OpenWriteAsync() )
                {
                    using ( var writer = new StreamWriter( stream ) )
                    {
                        writer.Write( content );
                    }
                }

                await client.LogOutAsync();

                return ftpFile;
            }
        }

        public static DirectoryInfo GetResourceDirectoryInfo( string directory = "" )
        {
            return new DirectoryInfo( $"{AppContext.BaseDirectory}/Resources/{directory}" );
        }
    }
}