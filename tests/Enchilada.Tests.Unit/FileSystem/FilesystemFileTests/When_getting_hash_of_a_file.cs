namespace Enchilada.Tests.Unit.FileSystem.FilesystemFileTests
{
    using Filesystem;
    using FluentAssertions;
    using Helpers;
    using Xunit;
    
    public class When_getting_hash_of_a_file
    {
        [ Fact ]
        public void Should_give_md5_hash_of_file()
        {
            var sut = new FilesystemFile( ResourceHelpers.GetResourceFileInfo( "SampleContent.txt" ) );
            var hash = sut.GetHashAsync().Result;

            hash.Should().NotBeEmpty();
            hash.Should().Be( "0edb2a42eee7dc39e8a9d15ecd827000" );
        }
    }
}