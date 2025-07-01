namespace Enchilada.Azure.Tests.Integration.BlobStorage.BlobStorageFileProviderTests
{
    using System.IO;
    using Azure.BlobStorage;
    using Filesystem;
    using Shouldly;
    using Helpers;
    using Xunit;

    public class When_opening_a_directory
    {
        [ Fact ]
        public void Should_give_deep_path()
        {
            var filesystemProvider = new BlobStorageFileProvider( new BlobStorageAdapterConfiguration
                                                                  {
                                                                      AdapterName = "blob_filesystem",
                                                                      CreateContainer = true,
                                                                      ConnectionString = AzuriteTestcontainer.GetConnectionString(),
                                                                      ContainerReference = "test",
                                                                      IsPublicAccess = true
                                                                  }, "level1/level2/" );

            filesystemProvider.ShouldBeOfType<BlobStorageFileProvider>();
            filesystemProvider.RootDirectory.RealPath.ShouldEndWith( "/level1/level2/" );
        }
    }
}