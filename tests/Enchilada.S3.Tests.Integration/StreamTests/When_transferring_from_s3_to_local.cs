namespace Enchilada.S3.Tests.Integration.StreamTests
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Shouldly;
    using Xunit;
    using global::Enchilada.S3.S3;
    using Enchilada.S3.Tests.Integration;
    using Filesystem;
    using global::Enchilada.Infrastructure;

    public class When_transferring_from_s3_to_local
    {
        [Fact]
        public async Task Should_transfer_content_between_filesystems()
        {
            var cfg = new S3AdapterConfiguration
            {
                ServiceUrl = MinioTestcontainer.GetServiceUrl(),
                AccessKey = MinioTestcontainer.GetAccessKey(),
                SecretKey = MinioTestcontainer.GetSecretKey(),
                BucketName = "test-bucket-transfer-reverse",
                CreateBucket = true
            };

            var s3Provider = new S3FileProvider( cfg, "" );
            var fsConfig = new FilesystemAdapterConfiguration
            {
                AdapterName = "local_filesystem",
                Directory = Path.GetTempPath()
            };
            var localProvider = new FilesystemFileProvider( fsConfig, "" );

            var s3File = s3Provider.RootDirectory.GetFile( "source.txt" );
            await using ( var writeSrc = await s3File.OpenWriteAsync() )
            {
                await writeSrc.WriteAsync( "content"u8.ToArray() );
            }

            var localFile = localProvider.RootDirectory.GetFile( "dest.txt" );

            await using ( var read = await s3File.OpenReadAsync() )
            await using ( var write = await localFile.OpenWriteAsync() )
            {
                await read.CopyToAsync( write );
            }

            var bytes = await localFile.ReadToEndAsync();
            Encoding.UTF8.GetString( bytes ).ShouldBe( "content" );
        }
    }
} 