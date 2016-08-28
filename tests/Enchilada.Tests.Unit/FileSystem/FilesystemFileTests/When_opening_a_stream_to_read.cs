namespace Enchilada.Tests.Unit.FileSystem.FilesystemFileTests
{
    using System.IO;
    using Filesystem;
    using FluentAssertions;
    using Helpers;
    using Xunit;

    public class When_opening_a_stream_to_read
    {
        [ Fact ]
        public void Should_give_stream()
        {
            var sut = new FilesystemFile( ResourceHelpers.GetResourceFileInfo( "SampleContent.txt" ) );
            using ( var stream = sut.OpenReadAsync().Result )
            {
                stream.Should().NotBe( null );
                stream.Position.Should().Be( 0 );
            }
        }

        [ Fact ]
        public void Should_be_able_to_read_file_contents_from_stream()
        {
            var sut = new FilesystemFile( ResourceHelpers.GetResourceFileInfo( "SampleContent.txt" ) );
            using ( var stream = sut.OpenReadAsync().Result )
            using ( var reader = new StreamReader( stream ) )
            {
                var content = reader.ReadToEnd();

                content.Should().NotBeEmpty();
                content.Should().StartWith( "Lorem ipsum" );
            }
        }
    }
}