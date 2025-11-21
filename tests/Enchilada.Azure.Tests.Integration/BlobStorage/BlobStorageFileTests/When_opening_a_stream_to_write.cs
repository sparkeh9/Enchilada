namespace Enchilada.Azure.Tests.Integration.BlobStorage.BlobStorageFileTests
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Azure.BlobStorage;
    using Shouldly;
    using Helpers;
    using Xunit;

    public class When_opening_a_stream_to_write
    {
        private const string WRITE_CONTENT = "this is a test - e3c49c83-c2ce-4a07-955a-f0555fc4e39c";

        [ Fact ]
        public void Should_not_have_file_in_place_before_creation()
        {
            var sut = new BlobStorageFile( ResourceHelpers.GetLocalDevelopmentContainer(), $"{Guid.NewGuid()}.txt" );
            sut.Exists.ShouldBeFalse();
        }

        [ Fact ]
        public async Task Should_create_file_when_stream_opened()
        {
            var sut = new BlobStorageFile( ResourceHelpers.GetLocalDevelopmentContainer(), $"{Guid.NewGuid()}.txt" );
            sut.Exists.ShouldBeFalse();

            using ( await sut.OpenWriteAsync() ) {}

            sut.Exists.ShouldBeTrue();
            await sut.DeleteAsync();
        }

        [ Fact ]
        public async Task Should_write_content_to_file()
        {
            var sut = new BlobStorageFile( ResourceHelpers.GetLocalDevelopmentContainer(), $"{Guid.NewGuid()}.txt" );

            using ( var stream = await sut.OpenWriteAsync() )
            using ( var writer = new StreamWriter( stream ) )
            {
                writer.Write( WRITE_CONTENT );
            }

            string fileContents = await sut.RealPath.MakeHttpRequestAsync();

            fileContents.ShouldBe( WRITE_CONTENT );

            await sut.DeleteAsync();
        }

        [ Fact ]
        public async Task Should_write_content_to_file_in_deep_structure()
        {
            var sut = new BlobStorageFile( ResourceHelpers.GetLocalDevelopmentContainer(), $"folder1/folder2/folder3/{Guid.NewGuid()}.txt" );

            using ( var stream = await sut.OpenWriteAsync() )
            using ( var writer = new StreamWriter( stream ) )
            {
                writer.Write( WRITE_CONTENT );
            }

            string fileContents = await sut.RealPath.MakeHttpRequestAsync();

            fileContents.ShouldBe( WRITE_CONTENT );

            await sut.DeleteAsync();
        }
    }
}