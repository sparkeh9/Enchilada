namespace Enchilada.Ftp.Tests.Integration.Ftp.FtpFileProviderTests
{
    using System.Text;
    using System.Threading.Tasks;
    using Helpers;
    using Xunit;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using Xunit.Abstractions;

    public class When_opening_a_file : FtpTestBase
    {
        public When_opening_a_file(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task Should_present_file()
        {
            var ftpConfig = new ConfigurationBuilder()
                .SetBasePath(System.AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build().GetSection("Ftp").Get<FtpConfiguration>();

            await ResourceHelpers.CreateFileWithContentAsync("level1/level2/level2content.txt", "Lorem ipsum dolor sit amet", ftpConfig, Logger);

            var filesystemProvider = new FtpFileProvider(new FtpAdapterConfiguration
            {
                AdapterName = "ftp_filesystem",
                Host = ftpConfig.Host,
                Username = ftpConfig.Username,
                Password = ftpConfig.Password,
                Port = ftpConfig.Port,
                Directory = "/"
            }, "level1/level2/level2content.txt");

            filesystemProvider.IsFile.Should().BeTrue();
            filesystemProvider.File.Should().NotBeNull();

            var bytes = await filesystemProvider.File.ReadToEndAsync();
            string contents = Encoding.UTF8.GetString(bytes);

            contents.Should().StartWith("Lorem ipsum");
        }
    }
}