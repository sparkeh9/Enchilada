namespace Enchilada.Azure.Tests.Integration.BlobStorage.StreamTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Azure.BlobStorage;
    using Configuration;
    using Enchilada.Infrastructure;
    using Filesystem;
    using Shouldly;
    using Helpers;
    using Xunit;

    public class When_transferring_between_filesystems
    {
        private readonly EnchiladaFileProviderResolver providerResolver;

        public When_transferring_between_filesystems()
        {
            providerResolver = new EnchiladaFileProviderResolver( new EnchiladaConfiguration
                                                                  {
                                                                      Adapters = new List<IEnchiladaAdapterConfiguration>
                                                                      {
                                                                          new BlobStorageAdapterConfiguration
                                                                          {
                                                                              AdapterName = "blob_filesystem",
                                                                              CreateContainer = true,
                                                                              ConnectionString = AzuriteTestcontainer.GetConnectionString(),
                                                                              ContainerReference = "test",
                                                                              IsPublicAccess = true
                                                                          },
                                                                          new FilesystemAdapterConfiguration
                                                                          {
                                                                              AdapterName = "local_filesystem",
                                                                              Directory = ResourceHelpers.GetResourceDirectoryInfo().FullName
                                                                          }
                                                                      }
                                                                  } );
        }

        [ Fact ]
        public async Task Should_write_text_file_to_blob_storage()
        {
            var sourceFile = providerResolver.OpenFileReference( "enchilada://local_filesystem/SampleContent.txt" );
            var targetFile = providerResolver.OpenFileReference( $"enchilada://blob_filesystem/SampleContent{Guid.NewGuid()}.txt" );

            await targetFile.CopyFromAsync( sourceFile );

            targetFile.Exists.ShouldBeTrue();

            string sourceHash = await sourceFile.GetHashAsync();
            string targetHash = await targetFile.GetHashAsync();

            sourceHash.ShouldBe( targetHash );

            await targetFile.DeleteAsync();
        }

        [ Fact ]
        public async Task Should_write_binary_file_to_blob_storage()
        {
            var sourceFile = providerResolver.OpenFileReference( "enchilada://local_filesystem/penguin.jpg" );
            var targetFile = providerResolver.OpenFileReference( $"enchilada://blob_filesystem/penguin{Guid.NewGuid()}.jpg" );

            await targetFile.CopyFromAsync( sourceFile );

            targetFile.Exists.ShouldBeTrue();

            string sourceHash = await sourceFile.GetHashAsync();
            string targetHash = await targetFile.GetHashAsync();

            sourceHash.ShouldBe( targetHash );

            await targetFile.DeleteAsync();
        }
    }
}