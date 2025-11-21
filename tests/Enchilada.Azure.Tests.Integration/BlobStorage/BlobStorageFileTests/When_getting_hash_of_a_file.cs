namespace Enchilada.Azure.Tests.Integration.BlobStorage.BlobStorageFileTests
{
    using System;
    using System.Threading.Tasks;
    using Azure.BlobStorage;
    using Shouldly;
    using Helpers;
    using Xunit;

    public class When_getting_hash_of_a_file
    {
        private const string FileContent = "When_getting_hash_of_a_file";

        [ Fact ]
        public async Task Should_give_md5_hash_of_file()
        {
            string fileName = $"{Guid.NewGuid()}.txt";

            await ResourceHelpers.CreateFileWithContentAsync( ResourceHelpers.GetLocalDevelopmentContainer(), fileName, FileContent );
            var sut = new BlobStorageFile( ResourceHelpers.GetLocalDevelopmentContainer(), fileName );

            var hash = await sut.GetHashAsync();

            hash.ShouldNotBeEmpty();
            hash.ShouldBe( "081cb72eaaacae3df4502708ff956d23" );

            await sut.DeleteAsync();
        }
    }
}