namespace Enchilada.Tests.Integration.FileSystem.FilesystemFileTests
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Filesystem;
    using Helpers;
    using Xunit;
    using Shouldly;

    public class When_opening_a_stream_to_write
    {
        private const string WRITE_CONTENT = "this is a test - e3c49c83-c2ce-4a07-955a-f0555fc4e39c";

        [ Fact ]
        public void Should_not_have_file_in_place_before_creation()
        {
            var sut = new FilesystemFile( new FileInfo( $"{ResourceHelpers.GetTempFilePath()}/{Guid.NewGuid()}.txt" ) );
            File.Exists( sut.RealPath ).ShouldBeFalse();
        }

        [ Fact ]
        public async Task Should_create_file_when_stream_opened()
        {
            string tempFileInfo = $"{ResourceHelpers.GetTempFilePath()}/{Guid.NewGuid()}.txt";
            var sut = new FilesystemFile( new FileInfo( tempFileInfo ) );

            File.Exists( sut.RealPath ).ShouldBeFalse();

            using ( await sut.OpenWriteAsync() ) {}

            File.Exists( sut.RealPath ).ShouldBeTrue();
            File.Delete( sut.RealPath );
        }

        [ Fact ]
        public async Task Should_write_content_to_file()
        {
            string tempFileInfo = $"{ResourceHelpers.GetTempFilePath()}/{Guid.NewGuid()}.txt";
            var sut = new FilesystemFile( new FileInfo( tempFileInfo ) );

            using ( var stream = await sut.OpenWriteAsync() )
            using ( var writer = new StreamWriter( stream ) )
            {
                writer.Write( WRITE_CONTENT );
            }

            string fileContents = File.ReadAllText( sut.RealPath );
            fileContents.ShouldBe( WRITE_CONTENT );

            File.Delete( sut.RealPath );
        }
    }
}