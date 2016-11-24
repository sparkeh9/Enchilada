namespace Enchilada.Ftp.Tests.Integration.Ftp.FtpFileTests
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Helpers;
    using Xunit;

    public class When_opening_a_stream_to_write : FtpTestBase
    {
        private const string WRITE_CONTENT = "this is a test - e3c49c83-c2ce-4a07-955a-f0555fc4e39c";

        [ Fact ]
        public void Should_not_have_file_in_place_before_creation()
        {
            var sut = new FtpFile( ResourceHelpers.GetLocalFtpClient(), $"{Guid.NewGuid()}.txt" );
            sut.Exists.Should().BeFalse();
        }


        [ Fact ]
        public async Task Should_write_content_to_file()
        {
            var sut = new FtpFile( ResourceHelpers.GetLocalFtpClient(), $"{Guid.NewGuid()}.txt" );

            using ( var stream = await sut.OpenWriteAsync() )
            using ( var writer = new StreamWriter( stream ) )
            {
                writer.Write( WRITE_CONTENT );
            }

            string fileContents = Encoding.UTF8.GetString( await sut.ReadToEndAsync() );

            fileContents.Should().Be( WRITE_CONTENT );

            await sut.DeleteAsync();
        }


        [ Fact ]
        public async Task Should_write_content_to_file_in_deep_structure()
        {
            var sut = new FtpFile( ResourceHelpers.GetLocalFtpClient(), $"{Guid.NewGuid()}.txt", "test1" );

            using ( var stream = await sut.OpenWriteAsync() )
            using ( var writer = new StreamWriter( stream ) )
            {
                writer.Write( WRITE_CONTENT );
            }

            string fileContents = Encoding.UTF8.GetString( await sut.ReadToEndAsync() );

            fileContents.Should().Be( WRITE_CONTENT );

            await sut.DeleteAsync();
        }

        [ Fact ]
        public async Task Should_write_content_to_file_with_base_path()
        {
            var sut = new FtpFile( ResourceHelpers.GetLocalFtpClient($"/{Guid.NewGuid()}/abc/123"), $"{Guid.NewGuid()}.txt", "test1" );

            using ( var stream = await sut.OpenWriteAsync() )
            using ( var writer = new StreamWriter( stream ) )
            {
                writer.Write( WRITE_CONTENT );
            }

            string fileContents = Encoding.UTF8.GetString( await sut.ReadToEndAsync() );

            fileContents.Should().Be( WRITE_CONTENT );

            await sut.DeleteAsync();
        }
    }
}