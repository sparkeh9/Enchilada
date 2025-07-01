namespace Enchilada.Azure.Tests.Integration.BlobStorage.BlobStorageFileTests
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Azure.BlobStorage;
    using Shouldly;
    using Helpers;
    using Xunit;

    public class When_reading_to_end_of_stream
    {
        private const string FileContent = "When_reading_to_end_of_stream";

        [ Fact ]
        public async Task Should_return_contents_of_file()
        {
            string fileName = $"{Guid.NewGuid()}.txt";
            await ResourceHelpers.CreateFileWithContentAsync( ResourceHelpers.GetLocalDevelopmentContainer(), fileName, FileContent );
            var sut = new BlobStorageFile( ResourceHelpers.GetLocalDevelopmentContainer(), fileName );

            var bytes = await sut.ReadToEndAsync();
            string contents = Encoding.UTF8.GetString( bytes );

            contents.ShouldBe( FileContent );
            await sut.DeleteAsync();
        }
    }
}