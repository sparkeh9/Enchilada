namespace Enchilada.Tests.Integration.FileSystem.FilesystemFileTests
{
    using System.IO;
    using System.Threading.Tasks;
    using Filesystem;
    using Helpers;
    using Xunit;
    using Shouldly;

    public class When_opening_a_stream_to_read
    {
        [ Fact ]
        public async Task Should_give_stream()
        {
            var sut = new FilesystemFile( ResourceHelpers.GetResourceFileInfo( "SampleContent.txt" ) );
            using ( var stream = await sut.OpenReadAsync() )
            {
                stream.ShouldNotBeNull();
                stream.Position.ShouldBe( 0 );
            }
        }

        [ Fact ]
        public async Task Should_be_able_to_read_file_contents_from_stream()
        {
            var sut = new FilesystemFile( ResourceHelpers.GetResourceFileInfo( "SampleContent.txt" ) );
            using ( var stream = await sut.OpenReadAsync() )
            using ( var reader = new StreamReader( stream ) )
            {
                var content = reader.ReadToEnd();

                content.ShouldNotBeEmpty();
                content.ShouldStartWith( "Lorem ipsum" );
            }
        }
    }
}