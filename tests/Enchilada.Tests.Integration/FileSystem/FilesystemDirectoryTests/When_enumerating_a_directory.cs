namespace Enchilada.Tests.Integration.FileSystem.FilesystemDirectoryTests
{
    using Filesystem;
    using FluentAssertions;
    using Helpers;
    using System.Linq;
    using Xunit;

    public class When_enumerating_a_directory
    {
        [ Fact ]
        public void Should_represent_directory()
        {
            var sut = new FilesystemDirectory( ResourceHelpers.GetResourceDirectoryInfo() );
            sut.IsDirectory.Should().BeTrue();
            sut.Name.Should().Be( "Resources" );
        }

        [ Fact ]
        public void Should_contain_a_single_file()
        {
            var sut = new FilesystemDirectory( ResourceHelpers.GetResourceDirectoryInfo() );

            var fileList = sut.Files.ToList();
            fileList.Count.Should().Be( 1 );

            var firstFile = fileList.First();
            firstFile.IsDirectory.Should().BeFalse();
            firstFile.Name.Should().Be( "SampleContent.txt" );
        }


        [ Fact ]
        public void Should_be_able_to_recurse_down_to_deepest_level()
        {
            var sut = new FilesystemDirectory( ResourceHelpers.GetResourceDirectoryInfo() );

            var deepestDirectory = sut.Directories.First().Directories.First();

            deepestDirectory.Name.Should().Be( "level2" );
            deepestDirectory.Files.First().Name.Should().Be( "level2content.txt" );
        }

        [ Fact ]
        public void Should_be_able_to_search()
        {
            var sut = new FilesystemDirectory( ResourceHelpers.GetResourceDirectoryInfo() );

            var level1List = sut.GetDirectories( "level1" );

            level1List.Should().NotBeEmpty();
        }

        [ Fact ]
        public void Should_not_find_nonexistent_path()
        {
            var sut = new FilesystemDirectory( ResourceHelpers.GetResourceDirectoryInfo() );

            var nonexistentPathList = sut.GetDirectories( "abc123" );

            nonexistentPathList.Should().BeEmpty();
        }

        [ Fact ]
        public void Should_list_all_nodes()
        {
            var sut = new FilesystemDirectory( ResourceHelpers.GetResourceDirectoryInfo( "level1" ) );

            var nodes = sut.ToList();

            nodes.Count( x => x.IsDirectory ).Should().Be( 1 );
            nodes.Count( x => !x.IsDirectory ).Should().Be( 1 );
        }
    }
}