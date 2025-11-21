namespace Enchilada.S3.Tests.Integration.S3.S3DirectoryTests
{
    using System;
    using System.Threading.Tasks;
    using Shouldly;
    using Xunit;
    using Enchilada.S3;
    using Enchilada.S3.Tests.Integration;
    using global::Enchilada.S3.S3;

    public class When_getting_file
    {
        [ Fact ]
        public async Task Should_return_file_object()
        {
            var config = new S3AdapterConfiguration
            {
                ServiceUrl = MinioTestcontainer.GetServiceUrl(),
                AccessKey = MinioTestcontainer.GetAccessKey(),
                SecretKey = MinioTestcontainer.GetSecretKey(),
                BucketName = "test-bucket-get-file",
                CreateBucket = true
            };

            var s3Provider = new S3FileProvider( config, "" );

            var file = s3Provider.RootDirectory.GetFile( "test-bucket-get-file/test-file.txt" );

            file.ShouldNotBeNull();
            file.Name.ShouldBe( "test-file.txt" );
            file.RealPath.ShouldBe( "test-bucket-get-file/test-file.txt" );
        }

        [Fact]
        public async Task Should_return_file_reference()
        {
            var config = new S3AdapterConfiguration
            {
                ServiceUrl = MinioTestcontainer.GetServiceUrl(),
                AccessKey = MinioTestcontainer.GetAccessKey(),
                SecretKey = MinioTestcontainer.GetSecretKey(),
                BucketName = "test-bucket-get-file",
                CreateBucket = true
            };

            var provider = new S3FileProvider( config, "folder1/" );
            var fileName = $"folder1/{Guid.NewGuid()}.txt";
            var file = provider.RootDirectory.GetFile( fileName );
            file.ShouldNotBeNull();
            file.Name.ShouldEndWith( ".txt" );
            file.IsDirectory.ShouldBeFalse();
        }
    }
}