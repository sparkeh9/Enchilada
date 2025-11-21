namespace Enchilada.S3.Tests.Integration.S3.S3FileTests
{
    using System.Threading.Tasks;
    using Shouldly;
    using Xunit;
    using Enchilada.S3;
    using Enchilada.S3.Tests.Integration;
    using global::Enchilada.S3.S3;

    public class When_deleting_a_file
    {
        [Fact]
        public async Task Should_delete_file()
        {
            var config = new S3AdapterConfiguration
            {
                ServiceUrl = MinioTestcontainer.GetServiceUrl(),
                AccessKey = MinioTestcontainer.GetAccessKey(),
                SecretKey = MinioTestcontainer.GetSecretKey(),
                BucketName = "test-bucket-delete-file",
                CreateBucket = true
            };

            var s3Provider = new S3FileProvider( config, "" );
            var file = s3Provider.RootDirectory.GetFile( "file-to-delete.txt" );

            await using ( var stream = await file.OpenWriteAsync() )
            {
                await stream.WriteAsync( "test content"u8.ToArray() );
            }

            file.Exists.ShouldBeTrue();

            await file.DeleteAsync();

            file.Exists.ShouldBeFalse();
        }
    }
}