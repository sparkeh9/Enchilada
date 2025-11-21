namespace Enchilada.S3.Tests.Integration.Infrastructure.EnchiladaFileProviderResolverTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Linq;
    using Shouldly;
    using Xunit;
    using global::Enchilada.S3.S3;
    using global::Enchilada.Infrastructure;
    using Configuration;
    using Enchilada.S3.Tests.Integration;

    public class When_enumerating_root
    {
        [Fact]
        public async Task Should_list_all_root_nodes_via_resolver()
        {
            var cfg = new S3AdapterConfiguration
            {
                ServiceUrl = MinioTestcontainer.GetServiceUrl(),
                AccessKey = MinioTestcontainer.GetAccessKey(),
                SecretKey = MinioTestcontainer.GetSecretKey(),
                BucketName = "test-bucket-root-resolver",
                CreateBucket = true
            };

            cfg.AdapterName = "s3_filesystem";
            var resolver = new EnchiladaFileProviderResolver( new EnchiladaConfiguration
            {
                Adapters = new List<IEnchiladaAdapterConfiguration> { cfg }
            } );

            var rootDir = resolver.OpenDirectoryReference( "enchilada://s3_filesystem" );

            var childDir = rootDir.GetDirectory( "child/" );
            await childDir.CreateDirectoryAsync();
            var childFile = rootDir.GetFile( "file.txt" );
            await using ( var stream = await childFile.OpenWriteAsync() )
            {
                await stream.WriteAsync( "content"u8.ToArray() );
            }

            var nodes = await rootDir.GetAllNodesAsync();
            nodes.Count().ShouldBe( 2 );
        }
    }
} 