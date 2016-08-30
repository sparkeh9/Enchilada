namespace Enchilada.Azure.Tests.Integration.BlobStorage.BlobStorageFileTests
{
    using System;
    using System.Threading.Tasks;
    using Azure.BlobStorage;
    using FluentAssertions;
    using Helpers;
    using Xunit;

    public class When_getting_hash_of_a_file
    {
        private const string FileContent = "Lorem Ipsum";

        [ Fact ]
        public async Task Should_give_md5_hash_of_file()
        {
            string fileName = $"{Guid.NewGuid()}.txt";

            await ResourceHelpers.CreateFileWithContentAsync( ResourceHelpers.GetLocalDevelopmentContainer(), fileName, FileContent );
            var sut = new BlobStorageFile( ResourceHelpers.GetLocalDevelopmentContainer(), fileName );

            var hash = await sut.GetHashAsync();

            hash.Should().NotBeEmpty();
            hash.Should().Be("6dbd01b4309de2c22b027eb35a3ce18b");

            await sut.DeleteAsync();
        }
    }
}