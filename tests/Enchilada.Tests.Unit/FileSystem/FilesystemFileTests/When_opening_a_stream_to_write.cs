namespace Enchilada.Tests.Unit.FileSystem.FilesystemFileTests
{
    using System;
    using System.IO;
    using Filesystem;
    using FluentAssertions;
    using Helpers;
    using Xunit;

    public class When_opening_a_stream_to_write
    {
        private const string WRITE_CONTENT = "this is a test - e3c49c83-c2ce-4a07-955a-f0555fc4e39c";

        [ Fact ]
        public void Should_not_have_file_in_place_before_creation()
        {
            var sut = new FilesystemFile( new FileInfo( $"{ResourceHelpers.GetTempFilePath()}/{Guid.NewGuid()}.txt" ) );
            File.Exists( sut.RealPath ).Should().BeFalse();
        }

        [ Fact ]
        public void Should_create_file_when_stream_opened()
        {
            string tempFileInfo = $"{ResourceHelpers.GetTempFilePath()}/{Guid.NewGuid()}.txt";
            var sut = new FilesystemFile( new FileInfo( tempFileInfo ) );

            File.Exists( sut.RealPath ).Should().BeFalse();

            using ( sut.OpenWrite() ) {}

            File.Exists( sut.RealPath ).Should().BeTrue();
            File.Delete( sut.RealPath );
        }

        [ Fact ]
        public void Should_write_content_to_file()
        {
            string tempFileInfo = $"{ResourceHelpers.GetTempFilePath()}/{Guid.NewGuid()}.txt";
            var sut = new FilesystemFile( new FileInfo( tempFileInfo ) );

            using ( var stream = sut.OpenWrite() )
            using ( var writer = new StreamWriter( stream ) )
            {
                writer.Write( WRITE_CONTENT );
            }

            string fileContents = File.ReadAllText( sut.RealPath );
            fileContents.Should().Be( WRITE_CONTENT );

            File.Delete( sut.RealPath );
        }
    }
}