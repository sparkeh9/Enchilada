namespace Enchilada.Ftp.Tests.Integration.Ftp.FtpFileProviderTests
{
    using System.Threading.Tasks;
    using Helpers;
    using Xunit;
    using Shouldly;
    using Xunit.Abstractions;

    public class When_opening_a_directory : FtpTestBase
    {
        public When_opening_a_directory(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task Should_present_directory()
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
            }, "level1/level2/");
            
            filesystemProvider.ShouldBeOfType<FtpFileProvider>();
            filesystemProvider.RootDirectory.RealPath.ShouldEndWith("/level1/level2/");
        }
    }
}