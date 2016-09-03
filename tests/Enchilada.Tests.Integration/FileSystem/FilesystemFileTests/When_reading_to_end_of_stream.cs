namespace Enchilada.Tests.Integration.FileSystem.FilesystemFileTests
{
    using System.Text;
    using System.Threading.Tasks;
    using Filesystem;
    using Helpers;
    using Xunit;
    using FluentAssertions;

    public class When_reading_to_end_of_stream
    {
        [ Fact ]
        public async Task Should_return_contents_of_file()
        {
            var sut = new FilesystemFile( ResourceHelpers.GetResourceFileInfo( "SampleContent.txt" ) );

            var bytes = await sut.ReadToEndAsync();
            string contents = Encoding.UTF8.GetString( bytes );

            contents.Should().StartWith( "Lorem ipsum" );
        }
    }
}