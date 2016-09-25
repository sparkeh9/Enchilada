namespace Enchilada.Ftp.Tests.Integration.Ftp.FtpFileTests
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Helpers;
    using Xunit;

    public class When_reading_to_end_of_stream : FtpTestBase
    {
        private const string FileContent = "When_reading_to_end_of_stream";

        [ Fact ]
        public async Task Should_return_contents_of_file()
        {
            string fileName = $"{Guid.NewGuid()}.txt";
            await ResourceHelpers.CreateFileWithContentAsync( fileName, FileContent );
            using ( var sut = new FtpFile( ResourceHelpers.GetLocalFtpClient(), fileName ) )
            {
                var bytes = await sut.ReadToEndAsync();
                string contents = Encoding.UTF8.GetString( bytes );

                contents.Should().Be( FileContent );
                await sut.DeleteAsync();
            }
        }
    }
}