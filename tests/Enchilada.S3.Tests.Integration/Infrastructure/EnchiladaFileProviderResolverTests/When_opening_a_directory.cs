namespace Enchilada.S3.Tests.Integration.Infrastructure.EnchiladaFileProviderResolverTests
{
    using System.Threading.Tasks;
    using Configuration;
    using Enchilada.S3.Tests.Integration;
    using Shouldly;
    using Xunit;
    using global::Enchilada.Infrastructure;
    using global::Enchilada.S3.S3;
    using System.Collections.Generic;

    public class When_opening_a_directory
    {
        [ Fact ]
        public async Task Should_return_s3_directory_provider()
        {
            var resolver = new EnchiladaFileProviderResolver( new EnchiladaConfiguration
            {
                Adapters =
                [
                    new S3AdapterConfiguration
                    {
                        AdapterName = "blob_filesystem",
                        ServiceUrl = MinioTestcontainer.GetServiceUrl(),
                        AccessKey = MinioTestcontainer.GetAccessKey(),
                        SecretKey = MinioTestcontainer.GetSecretKey(),
                        BucketName = "test-bucket-resolver",
                        CreateBucket = true
                    }
                ]
            } );

            var provider = resolver.OpenDirectoryReference( "s3-test://blob_filesystem/test-directory/" );

            provider.ShouldNotBeNull();
            provider.ShouldNotBeNull();
            provider.ShouldBeOfType<S3Directory>();
        }

        [Fact]
        public void Should_give_correct_s3_provider()
        {
            var resolver = new EnchiladaFileProviderResolver( new EnchiladaConfiguration
            {
                Adapters = new List<IEnchiladaAdapterConfiguration>
                {
                    new S3AdapterConfiguration
                    {
                        AdapterName = "s3_adapter",
                        ServiceUrl = MinioTestcontainer.GetServiceUrl(),
                        AccessKey = MinioTestcontainer.GetAccessKey(),
                        SecretKey = MinioTestcontainer.GetSecretKey(),
                        BucketName = "test-resolver-dir",
                        CreateBucket = true
                    }
                }
            } );

            var directory = resolver.OpenDirectoryReference( "enchilada://s3_adapter/" );
            directory.ShouldNotBeNull();
            directory.RealPath.ShouldBe( "" );
        }
    }
}