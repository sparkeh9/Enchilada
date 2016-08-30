namespace Enchilada.Azure.Tests.Integration.BlobStorage.BlobStorageFileTests
{
    using System;
    using System.Threading.Tasks;
    using Azure.BlobStorage;
    using FluentAssertions;
    using Helpers;
    using Xunit;

    public class When_deleting_a_file
    {
        [ Fact ]
        public async Task Should_delete_file_properly()
        {
            string fileName = $"{Guid.NewGuid()}.txt";

            await ResourceHelpers.CreateFileWithContentAsync( ResourceHelpers.GetLocalDevelopmentContainer(), fileName, "test" );
            var sut = new BlobStorageFile( ResourceHelpers.GetLocalDevelopmentContainer(), fileName );

            sut.Exists.Should().BeFalse();

            using ( await sut.OpenWriteAsync() ) {}

            sut.Exists.Should().BeTrue();

            await sut.DeleteAsync();

            sut.Exists.Should().BeFalse();
        }


        [ Fact ]
        public async Task Should_not_blow_up_if_file_does_not_exist()
        {
            string fileName = $"{Guid.NewGuid()}.txt";
            var sut = new BlobStorageFile( ResourceHelpers.GetLocalDevelopmentContainer(), fileName );

            sut.Exists.Should().BeFalse();

            await sut.DeleteAsync();
        }
    }
}