using System;
using System.IO;
using System.Threading.Tasks;
using CoreFtp;
using Microsoft.Extensions.Logging;

namespace Enchilada.Ftp.Tests.Integration.Helpers
{
    public static class ResourceHelpers
    {
        private static FtpClient ftpClient;

        public static FtpClient GetLocalFtpClient(FtpConfiguration ftpConfig, ILogger logger, string baseDirectory = "/")
        {
            ftpClient = new FtpClient(new FtpClientConfiguration
            {
                Host = ftpConfig.Host,
                Username = ftpConfig.Username,
                Password = ftpConfig.Password,
                Port = ftpConfig.Port,
                BaseDirectory = baseDirectory
            }) { Logger = logger };
            return ftpClient;
        }

        public static async Task<FtpFile> CreateFileWithContentAsync(string filename, string content, FtpConfiguration ftpConfig, ILogger logger)
        {
            using (var client = GetLocalFtpClient(ftpConfig, logger))
            {
                var ftpFile = new FtpFile(client, filename);

                using (var stream = await ftpFile.OpenWriteAsync())
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(content);
                    }
                }

                await client.LogOutAsync();

                return ftpFile;
            }
        }

        public static DirectoryInfo GetResourceDirectoryInfo(string directory = "")
        {
            return new DirectoryInfo($"{AppContext.BaseDirectory}/Resources/{directory}");
        }
    }
}