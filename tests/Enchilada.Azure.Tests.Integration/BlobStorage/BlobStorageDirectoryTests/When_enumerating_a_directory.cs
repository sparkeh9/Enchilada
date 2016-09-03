namespace Enchilada.Azure.Tests.Integration.BlobStorage.BlobStorageDirectoryTests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Azure.BlobStorage;
    using Filesystem;
    using FluentAssertions;
    using Helpers;
    using Xunit;

    public class When_enumerating_a_directory
    {
        [ Fact ]
        public async Task Should_represent_directory()
        {
            await ResourceHelpers.CreateFileWithContentAsync( ResourceHelpers.GetLocalDevelopmentContainer(), $"folder1/folder2/folder3/{Guid.NewGuid()}.txt", "stuff" );

            var sut = new BlobStorageDirectory( ResourceHelpers.GetLocalDevelopmentContainer(), "folder1" );
            sut.IsDirectory.Should().BeTrue();
            sut.Name.Should().Be( "folder1/" );


            await sut.DeleteAsync();
        }

        [ Fact ]
        public async Task Should_be_able_to_search()
        {
            var file = await ResourceHelpers.CreateFileWithContentAsync( ResourceHelpers.GetLocalDevelopmentContainer(), $"folder1/folder2/folder3/{Guid.NewGuid()}.txt", "stuff" );

            var sut = new BlobStorageDirectory( ResourceHelpers.GetLocalDevelopmentContainer(), "folder1" );

            var level1List = sut.GetDirectories( "folder1" );

            level1List.Should().NotBeEmpty();

            await sut.DeleteAsync();
        }


        [ Fact ]
        public async Task Should_not_find_nonexistent_path()
        {
            await ResourceHelpers.CreateFileWithContentAsync( ResourceHelpers.GetLocalDevelopmentContainer(), $"folder1/folder2/folder3/{Guid.NewGuid()}.txt", "stuff" );

            var sut = new BlobStorageDirectory( ResourceHelpers.GetLocalDevelopmentContainer(), "folder1" );

            var nonexistentPathList = sut.GetDirectories( "abc123" );

            nonexistentPathList.Should().BeEmpty();

            await sut.DeleteAsync();
        }

//
        [ Fact ]
        public async Task Should_list_all_nodes()
        {
            var file = await ResourceHelpers.CreateFileWithContentAsync( ResourceHelpers.GetLocalDevelopmentContainer(), $"folder1/{Guid.NewGuid()}.txt", "stuff" );
            var file2 = await ResourceHelpers.CreateFileWithContentAsync( ResourceHelpers.GetLocalDevelopmentContainer(), $"folder1/folder2/{Guid.NewGuid()}.txt", "stuff" );

            var sut = new BlobStorageDirectory( ResourceHelpers.GetLocalDevelopmentContainer(), "folder1" );

            var nodes = sut.ToList();

            nodes.Count( x => x.IsDirectory ).Should().Be( 1 );
            nodes.Count( x => !x.IsDirectory ).Should().Be( 1 );

            await file.DeleteAsync();
            await file2.DeleteAsync();
        }
    }
}