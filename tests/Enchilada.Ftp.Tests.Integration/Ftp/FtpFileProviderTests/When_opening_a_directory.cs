namespace Enchilada.Ftp.Tests.Integration.Ftp.FtpFileProviderTests
{
    using FluentAssertions;
    using Xunit;

    public class When_opening_a_directory : FtpTestBase
    {
        [ Fact ]
        public void Should_give_deep_path()
        {
            Program.Initialise();
            var filesystemProvider = new FtpFileProvider( new FtpAdapterConfiguration
            {
                AdapterName = "ftp_filesystem",
                Host = Program.FtpConfiguration.Host,
                Username = Program.FtpConfiguration.Username,
                Password = Program.FtpConfiguration.Password,
                Port = Program.FtpConfiguration.Port,
                Directory = "/"
            }, "level1/level2/" );

            filesystemProvider.Should().BeOfType<FtpFileProvider>();
            filesystemProvider.RootDirectory.RealPath.Should().Be( "/level1/level2/" );
        }
    }
}