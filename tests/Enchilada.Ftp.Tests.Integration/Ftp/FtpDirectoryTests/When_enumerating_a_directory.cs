namespace Enchilada.Ftp.Tests.Integration.Ftp.FtpDirectoryTests
{
    using System;
    using System.Threading.Tasks;
    using Shouldly;
    using Helpers;
    using Xunit;
    using Xunit.Abstractions;

    public class When_enumerating_a_directory : FtpTestBase
    {
        public When_enumerating_a_directory(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task Should_represent_directory()
        {
            var ftpConfig = FtpConfig;
            await ResourceHelpers.CreateFileWithContentAsync($"folder1/folder2/folder3/{Guid.NewGuid()}.txt", "stuff", ftpConfig, Logger);

            using (var ftpClient = ResourceHelpers.GetLocalFtpClient(ftpConfig, Logger))
            {
                var sut = new FtpDirectory(ftpClient, "folder1");

                sut.IsDirectory.ShouldBeTrue();
                sut.Name.ShouldBe("folder1/");
            }
        }
    }
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