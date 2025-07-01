namespace Enchilada.Tests.Integration.FileSystem.FilesystemDirectoryTests
{
    using Filesystem;
    using Shouldly;
    using Helpers;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class When_enumerating_a_directory
    {
        [ Fact ]
        public void Should_represent_directory()
        {
            var sut = new FilesystemDirectory( ResourceHelpers.GetResourceDirectoryInfo() );
            sut.IsDirectory.ShouldBeTrue();
            sut.Name.ShouldBe( "Resources" );
        }

        [ Fact ]
        public async Task Should_contain_a_single_file()
        {
            var sut = new FilesystemDirectory( ResourceHelpers.GetResourceDirectoryInfo() );

            var fileList = ( await sut.GetFilesAsync() ).ToList();
            fileList.Count.ShouldBe( 1 );

            var firstFile = fileList.First();
            firstFile.IsDirectory.ShouldBeFalse();
            firstFile.Name.ShouldBe( "SampleContent.txt" );
        }


        [ Fact ]
        public async Task Should_be_able_to_recurse_down_to_deepest_level()
        {
            var sut = new FilesystemDirectory( ResourceHelpers.GetResourceDirectoryInfo() );

            var deepestDirectory = ( await ( await sut.GetDirectoriesAsync() ).First().GetDirectoriesAsync() ).First();

            deepestDirectory.Name.ShouldBe( "level2" );
            ( await deepestDirectory.GetFilesAsync() ).First().Name.ShouldBe( "level2content.txt" );
        }

        [ Fact ]
        public void Should_list_all_nodes()
        {
            var sut = new FilesystemDirectory( ResourceHelpers.GetResourceDirectoryInfo( "level1" ) );

            var nodes = sut.ToList();

            nodes.Any( x => x.Name == "level2" ).ShouldBeTrue();
            nodes.Any( x => x.Name == "level1content.txt" ).ShouldBeTrue();
        }
    }
}