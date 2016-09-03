namespace Enchilada.Azure.Tests.Integration.BlobStorage.StreamTests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Azure.BlobStorage;
    using Configuration;
    using Enchilada.Infrastructure;
    using Filesystem;
    using FluentAssertions;
    using Helpers;
    using Xunit;

    public class When_transferring_between_filesystems
    {
        [ Fact ]
        public async Task Should_write_to_blob_storage()
        {
            var providerPath = ResourceHelpers.GetResourceDirectoryInfo();
            var sut = new EnchiladaFileProviderResolver( new EnchiladaConfiguration
                                                         {
                                                             Adapters = new List<IEnchiladaAdapterConfiguration>
                                                             {
                                                                 new BlobStorageAdapterConfiguration
                                                                 {
                                                                     AdapterName = "blob_filesystem",
                                                                     CreateContainer = true,
                                                                     ConnectionString = "UseDevelopmentStorage=true;",
                                                                     ContainerReference = "test",
                                                                     IsPublicAccess = true
                                                                 },
                                                                 new FilesystemAdapterConfiguration
                                                                 {
                                                                     AdapterName = "local_filesystem",
                                                                     Directory = providerPath.FullName
                                                                 }
                                                             }
                                                         } );

            var sourceFile = sut.OpenFileReference( "enchilada://local_filesystem/SampleContent.txt" );
            var targetFile = sut.OpenFileReference( "enchilada://blob_filesystem/SampleContent.txt" );

            await targetFile.CopyFromAsync( sourceFile );

            string sourceHash = await sourceFile.GetHashAsync();
            string targetHash = await targetFile.GetHashAsync();

            sourceHash.Should().Be( targetHash );

            await targetFile.DeleteAsync();
        }
    }
}