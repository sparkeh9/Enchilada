namespace Enchilada.Tests.Integration.FileSystem.FilesystemFileTests
{
    using System.Threading.Tasks;
    using Filesystem;
    using Helpers;
    using Xunit;
    using FluentAssertions;

    public class When_getting_hash_of_a_file
    {
        [ Fact ]
        public async Task Should_give_md5_hash_of_file()
        {
            var sut = new FilesystemFile( ResourceHelpers.GetResourceFileInfo( "SampleContent.txt" ) );
            var hash = await sut.GetHashAsync();

            hash.Should().NotBeEmpty();
            hash.Should().Be( "0edb2a42eee7dc39e8a9d15ecd827000" );
        }
    }
}