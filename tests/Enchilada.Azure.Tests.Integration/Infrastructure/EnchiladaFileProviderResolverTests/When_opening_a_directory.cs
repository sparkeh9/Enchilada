namespace Enchilada.Azure.Tests.Integration.Infrastructure.EnchiladaFileProviderResolverTests
{
    using System;
    using System.Collections.Generic;
    using Azure.BlobStorage;
    using Configuration;
    using Enchilada.Infrastructure.Exceptions;
    using Filesystem;
    using FluentAssertions;
    using Xunit;


    public class When_opening_a_directory
    {
        [ Fact ]
        public void Should_throw_exception_when_no_configuration_provided()
        {
            var sut = new EnchiladaFileProviderResolver();
            Assert.Throws<ConfigurationMissingException>( () => sut.OpenDirectory( "abc" ) );
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

            Assert.Throws<UriFormatException>( () => sut.OpenDirectory( "abc" ) );
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
            var directory = sut.OpenDirectory( "enchilada://blob_filesystem/" );
            directory.Should().BeOfType<BlobStorageDirectory>();
            directory.RealPath.Should().Be( "http://127.0.0.1:10000/devstoreaccount1/test/" );
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
                                                                 },
                                                                 new FilesystemAdapterConfiguration
                                                                 {
                                                                     AdapterName = "local_filesystem",
                                                                     Directory = "c:\\"
                                                                 },
                                                             }
                                                         } );

            var firstProvider = sut.OpenDirectory( "enchilada://blob_filesystem/" );
            firstProvider.Should().BeOfType<BlobStorageDirectory>();
            firstProvider.RealPath.Should().Be( "http://127.0.0.1:10000/devstoreaccount1/test/" );

            var secondProvider = sut.OpenDirectory( "enchilada://another_filesystem/abc123/" );
            secondProvider.Should().BeOfType<BlobStorageDirectory>();
            secondProvider.RealPath.Should().Be( "http://127.0.0.1:10000/devstoreaccount1/test123/abc123/" );

            var thirdProvider = sut.OpenDirectory( "enchilada://local_filesystem/abc123/" );
            thirdProvider.Should().BeOfType<FilesystemDirectory>();
            thirdProvider.RealPath.Should().Be( "c:\\abc123" );
        }
    }
}