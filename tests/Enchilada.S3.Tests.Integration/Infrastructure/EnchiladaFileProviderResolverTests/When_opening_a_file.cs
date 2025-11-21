namespace Enchilada.S3.Tests.Integration.Infrastructure.EnchiladaFileProviderResolverTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Configuration;
    using Enchilada.S3.Tests.Integration;
    using Shouldly;
    using Xunit;
    using global::Enchilada.Infrastructure;
    using global::Enchilada.S3.S3;

    public class When_opening_a_file
    {
        [Fact]
        public void Should_give_correct_s3_file()
        {
            var resolver = new EnchiladaFileProviderResolver( new EnchiladaConfiguration
            {
                Adapters = new List<IEnchiladaAdapterConfiguration>
                {
                    new S3AdapterConfiguration
                    {
                        AdapterName = "s3_file_adapter",
                        ServiceUrl = MinioTestcontainer.GetServiceUrl(),
                        AccessKey = MinioTestcontainer.GetAccessKey(),
                        SecretKey = MinioTestcontainer.GetSecretKey(),
                        BucketName = "test-resolver-file",
                        CreateBucket = true
                    }
                }
            } );

            var file = resolver.OpenFileReference( "enchilada://s3_file_adapter/test.txt" );
            file.ShouldNotBeNull();
            file.Name.ShouldBe( "test.txt" );
        }

        [ Fact ]
        public async Task Should_return_s3_file_provider()
        {
            var resolver = new EnchiladaFileProviderResolver( new EnchiladaConfiguration
            {
                Adapters =
                [
                    new S3AdapterConfiguration
                    {
                        AdapterName = "s3-test",
                        ServiceUrl = MinioTestcontainer.GetServiceUrl(),
                        AccessKey = MinioTestcontainer.GetAccessKey(),
                        SecretKey = MinioTestcontainer.GetSecretKey(),
                        BucketName = "test-bucket-resolver",
                        CreateBucket = true
                    }
                ]
            } );

            var provider = resolver.OpenFileReference( "s3-test://s3-test/test-file.txt" );

            provider.ShouldNotBeNull();
            provider.ShouldNotBeNull();
            provider.ShouldBeOfType<S3File>();
        }
    }
}