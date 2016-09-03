namespace Enchilada.Azure.Tests.Integration.BlobStorage.BlobStorageFileTests
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Azure.BlobStorage;
    using FluentAssertions;
    using Helpers;
    using Xunit;

    public class When_opening_a_stream_to_read
    {
        private string FileContent = "When_opening_a_stream_to_read";

        [ Fact ]
        public async Task Should_give_stream()
        {
            string fileName = $"{Guid.NewGuid()}.txt";
            await ResourceHelpers.CreateFileWithContentAsync( ResourceHelpers.GetLocalDevelopmentContainer(), fileName, FileContent );
            var sut = new BlobStorageFile( ResourceHelpers.GetLocalDevelopmentContainer(), fileName );

            using ( var stream = await sut.OpenReadAsync() )
            {
                stream.Should().NotBe( null );
                stream.Position.Should().Be( 0 );
            }

            await sut.DeleteAsync();
        }


        [ Fact ]
        public async Task Should_be_able_to_read_file_contents_from_stream()
        {
            string fileName = $"{Guid.NewGuid()}.txt";
            await ResourceHelpers.CreateFileWithContentAsync( ResourceHelpers.GetLocalDevelopmentContainer(), fileName, FileContent );
            var sut = new BlobStorageFile( ResourceHelpers.GetLocalDevelopmentContainer(), fileName );

            using ( var stream = await sut.OpenReadAsync() )
            using ( var reader = new StreamReader( stream ) )
            {
                string content = await reader.ReadToEndAsync();

                content.Should().NotBeEmpty();
                content.Should().StartWith( FileContent );
            }

            await sut.DeleteAsync();
        }
    }
}