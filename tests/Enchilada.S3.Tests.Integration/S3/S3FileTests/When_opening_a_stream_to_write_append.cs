namespace Enchilada.S3.Tests.Integration.S3.S3FileTests
{
    using System.Text;
    using System.Threading.Tasks;
    using Shouldly;
    using Xunit;
    using global::Enchilada.S3.S3;
    using Enchilada.S3.Tests.Integration;
    using global::Enchilada.Infrastructure;

    public class When_opening_a_stream_to_write_append
    {
        [Fact]
        public async Task Should_append_content_to_file()
        {
            var cfg = new S3AdapterConfiguration
            {
                ServiceUrl = MinioTestcontainer.GetServiceUrl(),
                AccessKey = MinioTestcontainer.GetAccessKey(),
                SecretKey = MinioTestcontainer.GetSecretKey(),
                BucketName = "test-bucket-append",
                CreateBucket = true
            };

            var provider = new S3FileProvider( cfg, "" );
            var file = provider.RootDirectory.GetFile( "append.txt" );

            await using ( var stream = await file.OpenWriteAsync() )
            {
                await stream.WriteAsync( "first"u8.ToArray() );
            }

            await using ( var stream = await file.OpenWriteAsync( FileMode.Append ) )
            {
                await stream.WriteAsync( "second"u8.ToArray() );
            }

            var bytes = await file.ReadToEndAsync();
            var read = Encoding.UTF8.GetString( bytes );
            read.ShouldBe( "firstsecond" );
        }
    }
} 