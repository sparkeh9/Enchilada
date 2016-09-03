namespace Enchilada.Azure.Tests.Integration.BlobStorage.BlobStorageFileProviderTests
{
    using System.IO;
    using Azure.BlobStorage;
    using Filesystem;
    using FluentAssertions;
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
                                                                      ConnectionString = "UseDevelopmentStorage=true;",
                                                                      ContainerReference = "test",
                                                                      IsPublicAccess = true
                                                                  }, "level1/level2/" );

            filesystemProvider.Should().BeOfType<BlobStorageFileProvider>();
            filesystemProvider.RootDirectory.RealPath.Should().Be( "http://127.0.0.1:10000/devstoreaccount1/test/level1/level2/" );
        }
    }
}