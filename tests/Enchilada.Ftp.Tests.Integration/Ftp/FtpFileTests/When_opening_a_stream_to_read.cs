namespace Enchilada.Ftp.Tests.Integration.Ftp.FtpFileTests
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Helpers;
    using Microsoft.Extensions.Configuration;
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
            var config = new ConfigurationBuilder()
                .SetBasePath(System.AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            await ResourceHelpers.CreateFileWithContentAsync(fileName, FileContent, config, Logger);
            var sut = new FtpFile(ResourceHelpers.GetLocalFtpClient(config, Logger), fileName);

            using (var stream = await sut.OpenReadAsync())
            {
                stream.Should().NotBeNull();
            }

            await sut.DeleteAsync();
        }


        [Fact]
        public async Task Should_be_able_to_read_file_contents_from_stream()
        {
            string fileName = $"{Guid.NewGuid()}.txt";
            var config = new ConfigurationBuilder()
                .SetBasePath(System.AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            await ResourceHelpers.CreateFileWithContentAsync(fileName, FileContent, config, Logger);
            using (var localFtpClient = ResourceHelpers.GetLocalFtpClient(config, Logger))
            {
                localFtpClient.Logger = Logger;

                var sut = new FtpFile(localFtpClient, fileName);
                using (var stream = await sut.OpenReadAsync())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        string content = await reader.ReadToEndAsync();

                        content.Should().NotBeEmpty();
                        content.Should().StartWith(FileContent);
                    }
                }

                await sut.DeleteAsync();
            }
        }
    }
}