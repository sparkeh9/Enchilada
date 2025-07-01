namespace Enchilada.Ftp.Tests.Integration.Ftp.FtpFileProviderTests
{
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using Xunit;
    using Xunit.Abstractions;

    public class When_opening_a_directory : FtpTestBase
    {
        public When_opening_a_directory(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public void Should_give_deep_path()
        {
            var ftpConfig = new ConfigurationBuilder()
                .SetBasePath(System.AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build().GetSection("Ftp").Get<FtpConfiguration>();

            var filesystemProvider = new FtpFileProvider(new FtpAdapterConfiguration
            {
                AdapterName = "ftp_filesystem",
                Host = ftpConfig.Host,
                Username = ftpConfig.Username,
                Password = ftpConfig.Password,
                Port = ftpConfig.Port,
                Directory = "/"
            }, "level1/level2/");

            filesystemProvider.Should().BeOfType<FtpFileProvider>();
            filesystemProvider.RootDirectory.RealPath.Should().Be("/level1/level2/");
        }
    }
}