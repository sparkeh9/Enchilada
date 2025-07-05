namespace Enchilada.S3.Tests.Integration.S3.S3DirectoryTests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Shouldly;
    using Xunit;
    using global::Enchilada.S3.S3;
    using Enchilada.S3.Tests.Integration;
    using global::Enchilada.Infrastructure;

    public class When_enumerating_root
    {
        [Fact]
        public async Task Should_list_all_root_nodes()
        {
            var cfg = new S3AdapterConfiguration
            {
                ServiceUrl = MinioTestcontainer.GetServiceUrl(),
                AccessKey = MinioTestcontainer.GetAccessKey(),
                SecretKey = MinioTestcontainer.GetSecretKey(),
                BucketName = "test-bucket-root",
                CreateBucket = true
            };

            var provider = new S3FileProvider( cfg, string.Empty );
            var root = provider.RootDirectory;

            var childDir = root.GetDirectory( "child/" );
            await childDir.CreateDirectoryAsync();
            var childFile = root.GetFile( "file.txt" );
            await using ( var stream = await childFile.OpenWriteAsync() )
            {
                await stream.WriteAsync( "content"u8.ToArray() );
            }

            var nodes = ( await root.GetAllNodesAsync() ).ToList();
            nodes.Count.ShouldBe( 2 );
        }
    }
} 