namespace Enchilada.Azure.Tests.Integration.Infrastructure.EnchiladaFileProviderResolverTests
{
    using System;
    using System.Collections.Generic;
    using BlobStorage;
    using Configuration;
    using Enchilada.Infrastructure;
    using Enchilada.Infrastructure.Exceptions;
    using FluentAssertions;
    using Xunit;


    public class When_opening_a_directory
    {
        [ Fact ]
        public void Should_throw_exception_when_no_configuration_provided()
        {
            var sut = new EnchiladaFileProviderResolver();
            Assert.Throws<ConfigurationMissingException>( () => sut.OpenProvider( "abc" ) );
        }

        [ Fact ]
        public void Should_throw_exception_if_uri_invalid()
        {
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
                                                                 }
                                                             }
                                                         } );

            Assert.Throws<UriFormatException>( () => sut.OpenProvider( "abc" ) );
        }

        [ Fact ]
        public void Should_give_blob_provider()
        {
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
                                                                 }
                                                             }
                                                         } );
            var provider = sut.OpenProvider( "enchilada://blob_filesystem/" );
            provider.Should().BeOfType<BlobStorageFileProvider>();
            provider.RootDirectory.RealPath.Should().Be( "/" );
        }

        [ Fact ]
        public void Should_give_correct_blob_provider()
        {
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
                                                                 new BlobStorageAdapterConfiguration
                                                                 {
                                                                     AdapterName = "another_filesystem",
                                                                     CreateContainer = true,
                                                                     ConnectionString = "UseDevelopmentStorage=true;",
                                                                     ContainerReference = "test123"
                                                                 }
                                                             }
                                                         } );

            var firstProvider = sut.OpenProvider( "enchilada://blob_filesystem/" );
            firstProvider.Should().BeOfType<BlobStorageFileProvider>();
            firstProvider.RootDirectory.RealPath.Should().Be( "/" );

            var secondProvider = sut.OpenProvider( "enchilada://another_filesystem/abc123" );
            secondProvider.Should().BeOfType<BlobStorageFileProvider>();
            secondProvider.RootDirectory.RealPath.Should().Be( "/abc123/" );
        }
    }
}