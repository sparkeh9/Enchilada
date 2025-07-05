namespace Enchilada.S3.Tests.Integration.S3.S3FileTests
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Shouldly;
    using Xunit;
    using Enchilada.S3;
    using Enchilada.S3.Tests.Integration;
    using global::Enchilada.S3.S3;

    public class When_reading_to_end_of_stream
    {
        private const string FileContent = "When_reading_to_end_of_stream";

        [Fact]
        public async Task Should_return_contents_of_file()
        {
            var config = new S3AdapterConfiguration
            {
                ServiceUrl = MinioTestcontainer.GetServiceUrl(),
                AccessKey = MinioTestcontainer.GetAccessKey(),
                SecretKey = MinioTestcontainer.GetSecretKey(),
                BucketName = "test-bucket-read-to-end",
                CreateBucket = true
            };

            var provider = new S3FileProvider( config, "" );
            var fileName = $"{Guid.NewGuid()}.txt";
            var file = provider.RootDirectory.GetFile( fileName );

            // write content
            await using ( var stream = await file.OpenWriteAsync() )
            {
                await stream.WriteAsync( Encoding.UTF8.GetBytes( FileContent ) );
            }

            var bytes = await file.ReadToEndAsync();
            var content = Encoding.UTF8.GetString( bytes );
            content.ShouldBe( FileContent );

            await file.DeleteAsync();
        }
    }
}