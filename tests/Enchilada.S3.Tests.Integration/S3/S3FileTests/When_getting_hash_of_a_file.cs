namespace Enchilada.S3.Tests.Integration.S3.S3FileTests
{
    using System.Text;
    using System.Threading.Tasks;
    using Shouldly;
    using Xunit;
    using Enchilada.S3;
    using Enchilada.S3.Tests.Integration;
    using global::Enchilada.S3.S3;

    public class When_getting_hash_of_a_file
    {
        [ Fact ]
        public async Task Should_return_md5_hash()
        {
            var config = new S3AdapterConfiguration
            {
                ServiceUrl = MinioTestcontainer.GetServiceUrl(),
                AccessKey = MinioTestcontainer.GetAccessKey(),
                SecretKey = MinioTestcontainer.GetSecretKey(),
                BucketName = "test-bucket-hash",
                CreateBucket = true
            };

            var s3Provider = new S3FileProvider( config, "" );
            var file = s3Provider.RootDirectory.GetFile( "file-to-hash.txt" );

            var content = "hello";
            await using ( var stream = await file.OpenWriteAsync() )
            {
                await stream.WriteAsync( Encoding.UTF8.GetBytes( content ) );
            }

            var expectedHash = "5d41402abc4b2a76b9719d911017c592"; // MD5 of "hello"
            var actualHash = await file.GetHashAsync();

            actualHash.ShouldBe( expectedHash );
        }
    }
}