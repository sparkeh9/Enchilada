namespace Enchilada.S3.Tests.Integration.S3.S3DirectoryTests
{
    using System.Linq;
    using System.Threading.Tasks;
    using Shouldly;
    using Xunit;
    using Enchilada.S3;
    using Enchilada.S3.Tests.Integration;
    using global::Enchilada.S3.S3;

    public class When_enumerating_a_directory
    {
        [ Fact ]
        public async Task Should_enumerate_files_and_directories()
        {
            var config = new S3AdapterConfiguration
            {
                AdapterName = "blob_filesystem",
                ServiceUrl = MinioTestcontainer.GetServiceUrl(),
                AccessKey = MinioTestcontainer.GetAccessKey(),
                SecretKey = MinioTestcontainer.GetSecretKey(),
                BucketName = "test-bucket-enumerate",
                CreateBucket = true
            };

            var s3Provider = new S3FileProvider( config, "" );

            // Create some dummy files and directories
            var file1 = s3Provider.RootDirectory.GetFile( "file1.txt" );
            await using ( var stream = await file1.OpenWriteAsync() )
            {
                await stream.WriteAsync( "test"u8.ToArray() );
            }

            var dir1 = s3Provider.RootDirectory.GetDirectory( "dir1/" );
            await dir1.CreateDirectoryAsync();

            var file2 = s3Provider.RootDirectory.GetFile( "dir1/file2.txt" );
            await using ( var stream = await file2.OpenWriteAsync() )
            {
                await stream.WriteAsync( "test"u8.ToArray() );
            }

            var children = ( await s3Provider.RootDirectory.GetAllNodesAsync() ).ToList();

            children.ShouldNotBeEmpty();
            children.ShouldContain( x => x.Name == "file1.txt" );
            children.ShouldContain( x => x.Name == "dir1/" );
        }
    }
}