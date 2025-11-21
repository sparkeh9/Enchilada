namespace Enchilada.S3.Tests.Integration.S3.S3DirectoryTests
{
    using System.Threading.Tasks;
    using Shouldly;
    using Xunit;
    using Enchilada.S3;
    using Enchilada.S3.Tests.Integration;
    using global::Enchilada.S3.S3;

    public class When_deleting_a_directory
    {
        [Fact]
        public async Task Should_delete_directory_and_contents()
        {
            var config = new S3AdapterConfiguration
            {
                ServiceUrl = MinioTestcontainer.GetServiceUrl(),
                AccessKey = MinioTestcontainer.GetAccessKey(),
                SecretKey = MinioTestcontainer.GetSecretKey(),
                BucketName = "test-bucket-delete",
                CreateBucket = true
            };

            var s3Provider = new S3FileProvider( config, "test-directory/" );
            await s3Provider.RootDirectory.CreateDirectoryAsync();

            var file = s3Provider.RootDirectory.GetFile( "test-directory/test-file.txt" );
            await using ( var stream = await file.OpenWriteAsync() )
            {
                await stream.WriteAsync( "test content"u8.ToArray() );
            }

            await s3Provider.RootDirectory.DeleteAsync();

            file.Exists.ShouldBeFalse();
            // S3 doesn't have explicit directories, so checking for directory existence is tricky.
            // We rely on the file being gone to confirm the directory is effectively deleted.
        }
    }
}