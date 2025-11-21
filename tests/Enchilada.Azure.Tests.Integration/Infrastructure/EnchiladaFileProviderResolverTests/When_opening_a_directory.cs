namespace Enchilada.Azure.Tests.Integration.Infrastructure.EnchiladaFileProviderResolverTests
{
    using System;
    using System.Collections.Generic;
    using Azure.BlobStorage;
    using Configuration;
    using Enchilada.Infrastructure;
    using Enchilada.Infrastructure.Exceptions;
    using Filesystem;
    using Shouldly;
    using Xunit;


    public class When_opening_a_directory
    {
        [ Fact ]
        public void Should_throw_exception_when_no_configuration_provided()
        {
            var sut = new EnchiladaFileProviderResolver();
            Assert.Throws<ConfigurationMissingException>( () => sut.OpenDirectoryReference( "abc" ) );
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
                                                                     ConnectionString = AzuriteTestcontainer.GetConnectionString(),
                                                                     ContainerReference = "test",
                                                                     IsPublicAccess = true
                                                                 }
                                                             }
                                                         } );

            Assert.Throws<UriFormatException>( () => sut.OpenDirectoryReference( "abc" ) );
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
                                                                     ConnectionString = AzuriteTestcontainer.GetConnectionString(),
                                                                     ContainerReference = "test",
                                                                     IsPublicAccess = true
                                                                 }
                                                             }
                                                         } );
            var directory = sut.OpenDirectoryReference( "enchilada://blob_filesystem/" );
            directory.ShouldBeOfType<BlobStorageDirectory>();
            // directory.RealPath.ShouldBe( $"http://127.0.0.1:10000/devstoreaccount1/test" );
            directory.RealPath.ShouldMatch( @"^http:\/\/127\.0\.0\.1:\d+\/devstoreaccount1\/test" );
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
                                                                     ConnectionString = AzuriteTestcontainer.GetConnectionString(),
                                                                     ContainerReference = "test",
                                                                     IsPublicAccess = true
                                                                 },
                                                                 new BlobStorageAdapterConfiguration
                                                                 {
                                                                     AdapterName = "another_filesystem",
                                                                     CreateContainer = true,
                                                                     ConnectionString = AzuriteTestcontainer.GetConnectionString(),
                                                                     ContainerReference = "enchilada-test"
                                                                 },
                                                                 new FilesystemAdapterConfiguration
                                                                 {
                                                                     AdapterName = "local_filesystem",
                                                                     Directory = "c:\\"
                                                                 },
                                                             }
                                                         } );

            var firstProvider = sut.OpenDirectoryReference( "enchilada://blob_filesystem/" );
            firstProvider.ShouldBeOfType<BlobStorageDirectory>();
            firstProvider.RealPath.ShouldEndWith( "/devstoreaccount1/test" );

            var secondProvider = sut.OpenDirectoryReference( "enchilada://another_filesystem/abc123/" );
            secondProvider.ShouldBeOfType<BlobStorageDirectory>();
            secondProvider.RealPath.ShouldEndWith("/devstoreaccount1/enchilada-test/abc123/");

            var thirdProvider = sut.OpenDirectoryReference( "enchilada://local_filesystem/abc123/" );
            thirdProvider.ShouldBeOfType<FilesystemDirectory>();
            thirdProvider.RealPath.ShouldBe( "c:\\abc123" );
        }
    }
}