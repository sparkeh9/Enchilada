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

    public class When_opening_a_stream_to_read
    {
        private const string FileContent = "When_opening_a_stream_to_read";

        [Fact]
        public async Task Should_return_contents_of_file()
        {
            var config = new S3AdapterConfiguration
            {
                ServiceUrl = MinioTestcontainer.GetServiceUrl(),
                AccessKey = MinioTestcontainer.GetAccessKey(),
                SecretKey = MinioTestcontainer.GetSecretKey(),
                BucketName = "test-bucket-stream-read",
                CreateBucket = true
            };

            var s3Provider = new S3FileProvider( config, "" );
            var fileName = $"{Guid.NewGuid()}.txt";
            var file = s3Provider.RootDirectory.GetFile( fileName );

            // write the file first
            await using ( var write = await file.OpenWriteAsync() )
            {
                await write.WriteAsync( Encoding.UTF8.GetBytes( FileContent ) );
            }

            await using ( var stream = await file.OpenReadAsync() )
            {
                var buffer = new byte[ FileContent.Length ];
                await stream.ReadAsync( buffer );
                var contents = Encoding.UTF8.GetString( buffer );
                contents.ShouldBe( FileContent );
            }
        }
    }
}