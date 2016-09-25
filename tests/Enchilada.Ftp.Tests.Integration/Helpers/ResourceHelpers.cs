namespace Enchilada.Ftp.Tests.Integration.Helpers
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using CoreFtp;

    public static class ResourceHelpers
    {
        public static FtpClient GetLocalFtpClient()
        {
            Program.Initialise();

            return new FtpClient( new FtpClientConfiguration
                                  {
                                      Host = Program.FtpConfiguration.Host,
                                      Username = Program.FtpConfiguration.Username,
                                      Password = Program.FtpConfiguration.Password,
                                      Port = Program.FtpConfiguration.Port
                                  } );
        }

        public static async Task<FtpFile> CreateFileWithContentAsync( string filename, string content )
        {
            using ( var client = GetLocalFtpClient() )
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