namespace Enchilada.Azure.Tests.Integration.BlobStorage.BlobStorageDirectoryTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Azure.BlobStorage;
    using Configuration;
    using Enchilada.Infrastructure;
    using FluentAssertions;
    using Helpers;
    using Xunit;

    public class When_enumerating_a_directory
    {
        [ Fact ]
        public async Task Should_represent_directory()
        {
            await ResourceHelpers.CreateFileWithContentAsync( ResourceHelpers.GetLocalDevelopmentContainer(), $"folder1/folder2/folder3/{Guid.NewGuid()}.txt", "stuff" );

            var sut = new BlobStorageDirectory( ResourceHelpers.GetLocalDevelopmentContainer(), "folder1" );
            sut.IsDirectory.Should().BeTrue();
            sut.Name.Should().Be( "folder1/" );


            await sut.DeleteAsync();
        }

        [ Fact ]
        public async Task Should_be_able_to_search()
        {
            var file = await ResourceHelpers.CreateFileWithContentAsync( ResourceHelpers.GetLocalDevelopmentContainer(), $"folder1/folder2/folder3/{Guid.NewGuid()}.txt", "stuff" );

            var sut = new BlobStorageDirectory( ResourceHelpers.GetLocalDevelopmentContainer(), "folder1" );

            var level1List = await sut.GetDirectoriesAsync();

            level1List.Should().NotBeEmpty();

            await sut.DeleteAsync();
        }

        [ Fact ]
        public async Task Should_list_all_nodes()
        {
            await ResourceHelpers.CreateFileWithContentAsync( ResourceHelpers.GetLocalDevelopmentContainer(), $"folder1/{Guid.NewGuid()}.txt", "stuff" );
            await ResourceHelpers.CreateFileWithContentAsync( ResourceHelpers.GetLocalDevelopmentContainer(), $"folder1/folder2/{Guid.NewGuid()}.txt", "stuff" );

            var sut = new BlobStorageDirectory( ResourceHelpers.GetLocalDevelopmentContainer(), "folder1" );

            var nodes = sut.ToList();

            nodes.Count( x => x.IsDirectory ).Should().Be( 1 );
            nodes.Count( x => !x.IsDirectory ).Should().Be( 1 );

            await sut.DeleteAsync();
      }

      [Fact]
      public async Task Should_list_all_nodes_at_root()
      {
         await ResourceHelpers.CreateFileWithContentAsync(ResourceHelpers.GetLocalDevelopmentContainer(), $"{Guid.NewGuid()}.txt", "stuff");

         var sut = new BlobStorageDirectory(ResourceHelpers.GetLocalDevelopmentContainer(), "");

         var nodes = await sut.GetFilesAsync();

         nodes.Count().Should().Be(1);

         await sut.DeleteAsync();
      }

      [Fact]
      public async Task Should_list_all_nodes_at_root_2()
      {
         await ResourceHelpers.CreateFileWithContentAsync(ResourceHelpers.GetLocalDevelopmentContainer(), $"{Guid.NewGuid()}.txt", "stuff");

         var sut = new EnchiladaFileProviderResolver(new EnchiladaConfiguration
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
                                                                 }
                                                             }
         });
         var directory = sut.OpenDirectoryReference("enchilada://blob_filesystem");

         var nodes = await directory.GetFilesAsync();

         nodes.Count().Should().Be(1);

         await directory.DeleteAsync();
      }
   }
}