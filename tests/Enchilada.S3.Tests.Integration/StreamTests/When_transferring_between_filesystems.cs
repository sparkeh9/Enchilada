namespace Enchilada.S3.Tests.Integration.StreamTests
{
    using System.IO;
    using System.Threading.Tasks;
    using Enchilada.S3.Tests.Integration;
    using Filesystem;
    using Shouldly;
    using Xunit;
    using global::Enchilada.S3.S3;

    public class When_transferring_between_filesystems
    {
        [Fact]
        public async Task Should_copy_file_from_local_to_s3()
        {
            var localProvider = new FilesystemFileProvider( new FilesystemAdapterConfiguration
            {
                Directory = Path.GetTempPath()
            }, "temp.txt" );

            const string content = "cross provider";
            await File.WriteAllTextAsync( localProvider.File.RealPath, content );

            var s3Provider = new S3FileProvider( new S3AdapterConfiguration
            {
                ServiceUrl = MinioTestcontainer.GetServiceUrl(),
                AccessKey = MinioTestcontainer.GetAccessKey(),
                SecretKey = MinioTestcontainer.GetSecretKey(),
                BucketName = "test-bucket-transfer",
                CreateBucket = true
            }, "copy.txt" );

            await s3Provider.File.CopyFromAsync( localProvider.File );

            var bytes = await s3Provider.File.ReadToEndAsync();
            var read = System.Text.Encoding.UTF8.GetString( bytes );
            read.ShouldBe( content );
        }
    }
} 