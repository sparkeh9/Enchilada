namespace Enchilada.S3.Tests.Integration.S3.S3FileProviderTests
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Shouldly;
    using Xunit;
    using Enchilada.S3;
    using Enchilada.S3.Tests.Integration;
    using global::Enchilada.S3.S3;

    public class When_opening_a_file
    {
        [ Fact ]
        public async Task Should_have_root_directory_as_directory_containing_file()
        {
            var s3Provider = new S3FileProvider( new S3AdapterConfiguration
                                                 {
                                                     ServiceUrl = MinioTestcontainer.GetServiceUrl(),
                                                     AccessKey = MinioTestcontainer.GetAccessKey(),
                                                     SecretKey = MinioTestcontainer.GetSecretKey(),
                                                     BucketName = "test-bucket",
                                                     CreateBucket = true
                                                 }, "level1/level2/level2content.txt" );

            s3Provider.RootDirectory.RealPath.ShouldBe( "level1/level2/" );
        }

        [ Fact ]
        public async Task Should_present_file()
        {
            var s3Provider = new S3FileProvider( new S3AdapterConfiguration
                                                 {
                                                     ServiceUrl = MinioTestcontainer.GetServiceUrl(),
                                                     AccessKey = MinioTestcontainer.GetAccessKey(),
                                                     SecretKey = MinioTestcontainer.GetSecretKey(),
                                                     BucketName = "test-bucket",
                                                     CreateBucket = true
                                                 }, "level1/level2/level2content.txt" );

            // Create a dummy file for the test
            await using ( var stream = await s3Provider.File.OpenWriteAsync() )
            {
                var bytes = "Lorem ipsum dolor sit amet"u8.ToArray();
                await stream.WriteAsync( bytes, 0, bytes.Length );
            }

            s3Provider.IsFile.ShouldBeTrue();
            s3Provider.File.ShouldNotBeNull();

            var bytesRead = await s3Provider.File.ReadToEndAsync();
            string contents = Encoding.UTF8.GetString( bytesRead );

            contents.ShouldStartWith( "Lorem ipsum" );
        }

        [ Fact ]
        public async Task Should_present_file_when_not_created_yet()
        {
            var s3Provider = new S3FileProvider( new S3AdapterConfiguration
                                                 {
                                                     ServiceUrl = MinioTestcontainer.GetServiceUrl(),
                                                     AccessKey = MinioTestcontainer.GetAccessKey(),
                                                     SecretKey = MinioTestcontainer.GetSecretKey(),
                                                     BucketName = "test-bucket",
                                                     CreateBucket = true
                                                 }, $"level1/level2/not_a_real_file_{Guid.NewGuid()}.txt" );

            s3Provider.IsFile.ShouldBeTrue();
            s3Provider.File.ShouldNotBeNull();
            s3Provider.File.Exists.ShouldBeFalse();
            s3Provider.RootDirectory.ShouldNotBeNull();
            s3Provider.RootDirectory.RealPath.ShouldBe( "level1/level2/" );
        }

        [Fact]
        public void Should_present_file_in_simple_case()
        {
            var provider = new S3FileProvider( new S3AdapterConfiguration
            {
                ServiceUrl = MinioTestcontainer.GetServiceUrl(),
                AccessKey = MinioTestcontainer.GetAccessKey(),
                SecretKey = MinioTestcontainer.GetSecretKey(),
                BucketName = "test-bucket-provider-file",
                CreateBucket = true
            }, "level1/level2/test.txt" );

            provider.IsFile.ShouldBeTrue();
            provider.File.ShouldNotBeNull();
            provider.File.Name.ShouldBe( "test.txt" );
        }
    }
}