namespace Enchilada.S3.Tests.Integration.S3.S3DirectoryTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Enchilada.S3.Tests.Integration;
    using Shouldly;
    using Xunit;
    using global::Enchilada.S3.S3;

    public class When_enumerating_nodes
    {
        [Fact]
        public async Task Should_list_all_nodes()
        {
            var config = new S3AdapterConfiguration
            {
                ServiceUrl = MinioTestcontainer.GetServiceUrl(),
                AccessKey = MinioTestcontainer.GetAccessKey(),
                SecretKey = MinioTestcontainer.GetSecretKey(),
                BucketName = "test-bucket-list-nodes",
                CreateBucket = true
            };

            var provider = new S3FileProvider( config, "folder1/" );

            // file at root folder1
            var file1 = provider.RootDirectory.GetFile( "folder1/" + Guid.NewGuid() + ".txt" );
            await using ( var s = await file1.OpenWriteAsync() )
            {
                await s.WriteAsync( "test"u8.ToArray() );
            }

            // nested file creates sub dir
            var file2 = provider.RootDirectory.GetFile( "folder1/folder2/" + Guid.NewGuid() + ".txt" );
            await using ( var s2 = await file2.OpenWriteAsync() )
            {
                await s2.WriteAsync( "test"u8.ToArray() );
            }

            var nodes = ( await provider.RootDirectory.GetAllNodesAsync() ).ToList();
            nodes.Count( n => n.IsDirectory ).ShouldBe( 1 );
            nodes.Count( n => !n.IsDirectory ).ShouldBe( 1 );
        }
    }
} 