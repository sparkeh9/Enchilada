namespace Enchilada.Ftp.Tests.Integration.Ftp.FtpFileProviderTests
{
    using System.Text;
    using System.Threading.Tasks;
    using Helpers;
    using Xunit;
    using FluentAssertions;

    public class When_opening_a_file : FtpTestBase
    {
        [ Fact ]
        public async Task Should_present_file()
        {
            Program.Initialise();

            await ResourceHelpers.CreateFileWithContentAsync( "level1/level2/level2content.txt", "Lorem ipsum dolor sit amet" );

            var filesystemProvider = new FtpFileProvider( new FtpAdapterConfiguration
            {
                AdapterName = "ftp_filesystem",
                Host = Program.FtpConfiguration.Host,
                Username = Program.FtpConfiguration.Username,
                Password = Program.FtpConfiguration.Password,
                Port = Program.FtpConfiguration.Port,
                Directory = "/"
            }, "level1/level2/level2content.txt" );

            filesystemProvider.IsFile.Should().BeTrue();
            filesystemProvider.File.Should().NotBeNull();

            var bytes = await filesystemProvider.File.ReadToEndAsync();
            string contents = Encoding.UTF8.GetString( bytes );

            contents.Should().StartWith( "Lorem ipsum" );
        }
    }
}