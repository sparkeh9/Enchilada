namespace Enchilada.Ftp.Tests.Integration.Ftp.FtpDirectoryTests
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Helpers;
    using Xunit;

    public class When_enumerating_a_directory : FtpTestBase
    {
//        [ Fact ]
//        public async Task Should_represent_directory()
//        {
//            await ResourceHelpers.CreateFileWithContentAsync( $"folder1/folder2/folder3/{Guid.NewGuid()}.txt", "stuff" );
//
//            using ( var ftpClient = ResourceHelpers.GetLocalFtpClient() )
//            {
//                var sut = new FtpDirectory( ftpClient, "folder1" );
//
//                sut.IsDirectory.Should().BeTrue();
//                sut.Name.Should().Be( "folder1/" );
//            }


//            var sut = new BlobStorageDirectory( ResourceHelpers.GetLocalDevelopmentContainer(), "folder1" );
//            sut.IsDirectory.Should().BeTrue();
//            sut.Name.Should().Be( "folder1/" );
//

//            await sut.DeleteAsync();
    }

//
//        [ Fact ]
//        public async Task Should_be_able_to_search()
//        {
//            var file = await ResourceHelpers.CreateFileWithContentAsync( ResourceHelpers.GetLocalDevelopmentContainer(), $"folder1/folder2/folder3/{Guid.NewGuid()}.txt", "stuff" );
//
//            var sut = new BlobStorageDirectory( ResourceHelpers.GetLocalDevelopmentContainer(), "folder1" );
//
//            var level1List = sut.GetDirectories( "folder1" );
//
//            level1List.Should().NotBeEmpty();
//
//            await sut.DeleteAsync();
//        }
//
//
//        [ Fact ]
//        public async Task Should_not_find_nonexistent_path()
//        {
//            await ResourceHelpers.CreateFileWithContentAsync( ResourceHelpers.GetLocalDevelopmentContainer(), $"folder1/folder2/folder3/{Guid.NewGuid()}.txt", "stuff" );
//
//            var sut = new BlobStorageDirectory( ResourceHelpers.GetLocalDevelopmentContainer(), "folder1" );
//
//            var nonexistentPathList = sut.GetDirectories( "abc123" );
//
//            nonexistentPathList.Should().BeEmpty();
//
//            await sut.DeleteAsync();
//        }
//
//        [ Fact ]
//        public async Task Should_list_all_nodes()
//        {
//            var file = await ResourceHelpers.CreateFileWithContentAsync( ResourceHelpers.GetLocalDevelopmentContainer(), $"folder1/{Guid.NewGuid()}.txt", "stuff" );
//            var file2 = await ResourceHelpers.CreateFileWithContentAsync( ResourceHelpers.GetLocalDevelopmentContainer(), $"folder1/folder2/{Guid.NewGuid()}.txt", "stuff" );
//
//            var sut = new BlobStorageDirectory( ResourceHelpers.GetLocalDevelopmentContainer(), "folder1" );
//
//            var nodes = sut.ToList();
//
//            nodes.Count( x => x.IsDirectory ).Should().Be( 1 );
//            nodes.Count( x => !x.IsDirectory ).Should().Be( 1 );
//
//            await sut.DeleteAsync();
//        }
}