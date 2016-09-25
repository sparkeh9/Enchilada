namespace Enchilada.Ftp.Tests.Integration.Ftp.FtpFileTests
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Helpers;
    using Xunit;

    public class When_opening_a_stream_to_read : FtpTestBase
    {
        private string FileContent = "When_opening_a_stream_to_read";

        [ Fact ]
        public async Task Should_give_stream()
        {
            string fileName = $"{Guid.NewGuid()}.txt";
            await ResourceHelpers.CreateFileWithContentAsync( fileName, FileContent );
            var sut = new FtpFile( ResourceHelpers.GetLocalFtpClient(), fileName );

            using ( var stream = await sut.OpenReadAsync() )
            {
                stream.Should().NotBe( null );
            }

            await sut.DeleteAsync();
        }


        [ Fact ]
        public async Task Should_be_able_to_read_file_contents_from_stream()
        {
            string fileName = $"{Guid.NewGuid()}.txt";
            await ResourceHelpers.CreateFileWithContentAsync( fileName, FileContent );
            using ( var localFtpClient = ResourceHelpers.GetLocalFtpClient() )
            {
                localFtpClient.Logger = Logger;

                var sut = new FtpFile( localFtpClient, fileName );
                using ( var stream = await sut.OpenReadAsync() )
                {
                    using ( var reader = new StreamReader( stream ) )
                    {
                        string content = await reader.ReadToEndAsync();

                        content.Should().NotBeEmpty();
                        content.Should().StartWith( FileContent );
                    }
                }

                await sut.DeleteAsync();
            }
        }
    }
}