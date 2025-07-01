namespace Enchilada.Ftp.Tests.Integration.Ftp.FtpFileTests
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Helpers;
    using Microsoft.Extensions.Configuration;
    using Xunit;
    using Xunit.Abstractions;

    public class When_reading_to_end_of_stream : FtpTestBase
    {
        private const string FileContent = "When_reading_to_end_of_stream";

        public When_reading_to_end_of_stream(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task Should_return_contents_of_file()
        {
            string fileName = $"{Guid.NewGuid()}.txt";
            var config = new ConfigurationBuilder()
                .SetBasePath(System.AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            await ResourceHelpers.CreateFileWithContentAsync(fileName, FileContent, config, Logger);
            using (var sut = new FtpFile(ResourceHelpers.GetLocalFtpClient(config, Logger), fileName))
            {
                var bytes = await sut.ReadToEndAsync();
                string contents = Encoding.UTF8.GetString(bytes);

                contents.Should().Be(FileContent);
                await sut.DeleteAsync();
            }
        }
    }
}