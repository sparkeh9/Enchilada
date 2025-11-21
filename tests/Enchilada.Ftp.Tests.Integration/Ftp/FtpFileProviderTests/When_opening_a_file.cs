namespace Enchilada.Ftp.Tests.Integration.Ftp.FtpFileProviderTests
{
    using System.Text;
    using System.Threading.Tasks;
    using Helpers;
    using Xunit;
    using Shouldly;
    using Xunit.Abstractions;

    public class When_opening_a_file : FtpTestBase
    {
        public When_opening_a_file(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task Should_present_file()
        {
            var ftpConfig = FtpConfig;

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

            filesystemProvider.IsFile.ShouldBeTrue();
            filesystemProvider.File.ShouldNotBeNull();

            var bytes = await filesystemProvider.File.ReadToEndAsync();
            string contents = Encoding.UTF8.GetString(bytes);

            contents.ShouldStartWith("Lorem ipsum");
        }
    }
}