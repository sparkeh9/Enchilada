namespace Enchilada.Tests.Unit.FileSystem.FilesystemFileTests
{
    using System.Text;
    using System.Threading.Tasks;
    using Filesystem;
    using Helpers;
    using FluentAssertions;
    using Xunit;

    public class When_reading_to_end_of_stream
    {
        [ Fact ]
        public void Should_return_contents_of_file()
        {
            var sut = new FilesystemFile( ResourceHelpers.GetResourceFileInfo( "SampleContent.txt" ) );

            var bytes = Task.Run( () => sut.ReadToEndAsync() ).Result;
            string contents = Encoding.UTF8.GetString( bytes );

            contents.Should().StartWith( "Lorem ipsum" );
        }
    }
}