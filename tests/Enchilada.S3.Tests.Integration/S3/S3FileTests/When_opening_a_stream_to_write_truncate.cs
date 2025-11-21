namespace Enchilada.S3.Tests.Integration.S3.S3FileTests
{
    using System.Text;
    using System.Threading.Tasks;
    using Shouldly;
    using Xunit;
    using global::Enchilada.S3.S3;
    using Enchilada.S3.Tests.Integration;
    using global::Enchilada.Infrastructure;

    public class When_opening_a_stream_to_write_truncate
    {
        [Fact]
        public async Task Should_truncate_and_write_content()
        {
            var cfg = new S3AdapterConfiguration
            {
                ServiceUrl = MinioTestcontainer.GetServiceUrl(),
                AccessKey = MinioTestcontainer.GetAccessKey(),
                SecretKey = MinioTestcontainer.GetSecretKey(),
                BucketName = "test-bucket-truncate",
                CreateBucket = true
            };

            var provider = new S3FileProvider( cfg, "" );
            var file = provider.RootDirectory.GetFile( "truncate.txt" );

            await using ( var stream = await file.OpenWriteAsync() )
            {
                await stream.WriteAsync( "longcontent"u8.ToArray() );
            }

            await using ( var stream = await file.OpenWriteAsync( FileMode.Truncate ) )
            {
                await stream.WriteAsync( "short"u8.ToArray() );
            }

            var bytes = await file.ReadToEndAsync();
            var read = Encoding.UTF8.GetString( bytes );
            read.ShouldBe( "short" );
        }
    }
} 