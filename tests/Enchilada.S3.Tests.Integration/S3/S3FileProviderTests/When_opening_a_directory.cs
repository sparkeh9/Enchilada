namespace Enchilada.S3.Tests.Integration.S3.S3FileProviderTests
{
    using Enchilada.S3.Tests.Integration;
    using Shouldly;
    using Xunit;
    using global::Enchilada.S3.S3;

    public class When_opening_a_directory
    {
        [Fact]
        public void Should_give_deep_path()
        {
            var provider = new S3FileProvider( new S3AdapterConfiguration
            {
                AdapterName = "s3_filesystem",
                ServiceUrl = MinioTestcontainer.GetServiceUrl(),
                AccessKey = MinioTestcontainer.GetAccessKey(),
                SecretKey = MinioTestcontainer.GetSecretKey(),
                BucketName = "test-bucket-provider-dir",
                CreateBucket = true
            }, "level1/level2/" );

            provider.RootDirectory.RealPath.ShouldEndWith( "level1/level2/" );
            provider.IsDirectory.ShouldBeTrue();
        }
    }
} 