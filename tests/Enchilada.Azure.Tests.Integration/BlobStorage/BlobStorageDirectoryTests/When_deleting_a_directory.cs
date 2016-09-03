namespace Enchilada.Azure.Tests.Integration.BlobStorage.BlobStorageDirectoryTests
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Azure.BlobStorage;
    using Filesystem;
    using FluentAssertions;
    using Helpers;
    using Xunit;

    public class When_deleting_a_directory
    {
        [ Fact ]
        public async Task Should_delete_a_directory()
        {
            var file = new BlobStorageFile( ResourceHelpers.GetLocalDevelopmentContainer(), $"folder1/folder2/folder3/{Guid.NewGuid()}.txt" );

            using ( var stream = await file.OpenWriteAsync() )
            using ( var writer = new StreamWriter( stream ) )
            {
                writer.Write( "test" );
            }

            var sut = new BlobStorageDirectory( ResourceHelpers.GetLocalDevelopmentContainer(), "folder1" );

            sut.Exists.Should().BeTrue();
            await sut.DeleteAsync();
        }

        [ Fact ]
        public async Task Should_not_throw_exception_when_directory_not_present()
        {
            var sut = new BlobStorageDirectory( ResourceHelpers.GetLocalDevelopmentContainer(), "does_not_exist" );
            await sut.DeleteAsync();
        }
    }
}