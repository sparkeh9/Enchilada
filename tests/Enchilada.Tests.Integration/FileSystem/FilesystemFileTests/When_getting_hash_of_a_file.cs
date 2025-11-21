namespace Enchilada.Tests.Integration.FileSystem.FilesystemFileTests
{
    using System.Threading.Tasks;
    using Filesystem;
    using Helpers;
    using Xunit;
    using Shouldly;

    public class When_getting_hash_of_a_file
    {
        [ Fact ]
        public async Task Should_give_md5_hash_of_file()
        {
            var sut = new FilesystemFile( ResourceHelpers.GetResourceFileInfo( "SampleContent.txt" ) );
            var hash = await sut.GetHashAsync();

            hash.ShouldNotBeEmpty();
            hash.Length.ShouldBe( 32 );
        }
    }
}