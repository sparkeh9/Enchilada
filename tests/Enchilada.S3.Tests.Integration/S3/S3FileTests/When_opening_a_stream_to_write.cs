namespace Enchilada.S3.Tests.Integration.S3.S3FileTests
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Shouldly;
    using Xunit;
    using Enchilada.S3;
    using Enchilada.S3.Tests.Integration;
    using global::Enchilada.S3.S3;

    public class When_opening_a_stream_to_write
    {
        private const string WriteContent = "this is a test - s3 stream write";

        [Fact]
        public async Task Should_write_content_to_file()
        {
            var config = new S3AdapterConfiguration
            {
                ServiceUrl = MinioTestcontainer.GetServiceUrl(),
                AccessKey = MinioTestcontainer.GetAccessKey(),
                SecretKey = MinioTestcontainer.GetSecretKey(),
                BucketName = "test-bucket-stream-write",
                CreateBucket = true
            };

            var provider = new S3FileProvider( config, "" );
            var filePath = $"folder1/folder2/{Guid.NewGuid()}.txt";
            var file = provider.RootDirectory.GetFile( filePath );

            // Write
            await using ( var stream = await file.OpenWriteAsync() )
            await using ( var writer = new StreamWriter( stream ) )
            {
                writer.Write( WriteContent );
            }

            // Read back
            var bytes = await file.ReadToEndAsync();
            var content = Encoding.UTF8.GetString( bytes );
            content.ShouldBe( WriteContent );

            await file.DeleteAsync();
        }
    }
}