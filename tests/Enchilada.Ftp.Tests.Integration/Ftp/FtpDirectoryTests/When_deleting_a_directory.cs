namespace Enchilada.Ftp.Tests.Integration.Ftp.FtpDirectoryTests
{
    using System;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Helpers;
    using Xunit;

    public class When_deleting_a_directory : FtpTestBase
    {
        [ Fact ]
        public async Task When_creating_nested_folders()
        {
            await ResourceHelpers.CreateFileWithContentAsync( $"folder1/folder2/folder3/{Guid.NewGuid()}.txt", "stuff" );

            using ( var ftpClient = ResourceHelpers.GetLocalFtpClient() )
            {
                ftpClient.Logger = Logger;
                var sut = new FtpDirectory( ftpClient, "folder1" );

                sut.IsDirectory.Should().BeTrue();
                sut.Name.Should().Be( "folder1/" );
            }
        }


//        [ Fact ]
//        public async Task Should_delete_a_directory()
//        {
//            var file = new BlobStorageFile( ResourceHelpers.GetLocalDevelopmentContainer(), $"folder1/folder2/folder3/{Guid.NewGuid()}.txt" );
//
//            using ( var stream = await file.OpenWriteAsync() )
//            using ( var writer = new StreamWriter( stream ) )
//            {
//                writer.Write( "test" );
//            }
//
//            var sut = new BlobStorageDirectory( ResourceHelpers.GetLocalDevelopmentContainer(), "folder1" );
//
//            sut.Exists.Should().BeTrue();
//            await sut.DeleteAsync();
//        }
//
//        [ Fact ]
//        public async Task Should_not_throw_exception_when_directory_not_present()
//        {
//            var sut = new BlobStorageDirectory( ResourceHelpers.GetLocalDevelopmentContainer(), "does_not_exist" );
//            await sut.DeleteAsync();
//        }
    }
}