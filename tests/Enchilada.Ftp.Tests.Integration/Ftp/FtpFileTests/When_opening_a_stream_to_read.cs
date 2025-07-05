namespace Enchilada.Ftp.Tests.Integration.Ftp.FtpFileTests
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Shouldly;
    using Helpers;
    using Xunit;
    using Xunit.Abstractions;

    public class When_opening_a_stream_to_read : FtpTestBase
    {
        private string FileContent = "When_opening_a_stream_to_read";

        public When_opening_a_stream_to_read(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task Should_give_stream()
        {
            string fileName = $"{Guid.NewGuid()}.txt";
            var ftpConfig = FtpConfig;
            await ResourceHelpers.CreateFileWithContentAsync(fileName, FileContent, ftpConfig, Logger);
            var sut = new FtpFile(ResourceHelpers.GetLocalFtpClient(ftpConfig, Logger), fileName);

            using (var stream = await sut.OpenReadAsync())
            {
                stream.ShouldNotBeNull();
            }

            await sut.DeleteAsync();
        }


        [Fact]
        public async Task Should_be_able_to_read_file_contents_from_stream()
        {
            string fileName = $"{Guid.NewGuid()}.txt";
            var ftpConfig = FtpConfig;
            await ResourceHelpers.CreateFileWithContentAsync(fileName, FileContent, ftpConfig, Logger);
            using (var localFtpClient = ResourceHelpers.GetLocalFtpClient(ftpConfig, Logger))
            {
                localFtpClient.Logger = Logger;

                var sut = new FtpFile(localFtpClient, fileName);
                using (var stream = await sut.OpenReadAsync())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        string content = await reader.ReadToEndAsync();

                        content.ShouldNotBeEmpty();
                        content.ShouldStartWith(FileContent);
                    }
                }

                await sut.DeleteAsync();
            }
        }
    }
}