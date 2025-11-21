namespace Enchilada.S3.Tests.Integration.S3.S3DirectoryTests
{
    using System;
    using System.Threading.Tasks;
    using Enchilada.S3.Tests.Integration;
    using Shouldly;
    using Xunit;
    using global::Enchilada.S3.S3;

    public class When_enumerating_search
    {
        [Fact]
        public async Task Should_return_subdirectories()
        {
            var config = new S3AdapterConfiguration
            {
                ServiceUrl = MinioTestcontainer.GetServiceUrl(),
                AccessKey = MinioTestcontainer.GetAccessKey(),
                SecretKey = MinioTestcontainer.GetSecretKey(),
                BucketName = "test-bucket-search",
                CreateBucket = true
            };

            var provider = new S3FileProvider( config, "" );
            var dir = provider.RootDirectory.GetDirectory( "level1/" );
            await dir.CreateDirectoryAsync();

            var sub = dir.GetDirectory( "level1/level2/" );
            await sub.CreateDirectoryAsync();

            var dirs = await provider.RootDirectory.GetDirectoriesAsync();
            dirs.ShouldContain( d => d.Name == "level1/" );
        }
    }
} 