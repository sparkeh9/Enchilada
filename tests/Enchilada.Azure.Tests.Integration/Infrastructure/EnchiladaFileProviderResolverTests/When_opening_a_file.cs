namespace Enchilada.Azure.Tests.Integration.Infrastructure.EnchiladaFileProviderResolverTests
{
    using System;
    using System.Collections.Generic;
    using Azure.BlobStorage;
    using Configuration;
    using Enchilada.Infrastructure;
    using Filesystem;
    using Xunit;
    using FluentAssertions;

    public class When_opening_a_file
    {
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

            Assert.Throws<UriFormatException>( () => sut.OpenFile( "abc" ) );
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

            var provider = sut.OpenFile( "enchilada://blob_filesystem/ABC123.txt" );
            provider.Should().BeOfType<BlobStorageFile>();
            provider.RealPath.Should().Be( "http://127.0.0.1:10000/devstoreaccount1/test/ABC123.txt" );
            provider.Name.Should().Be( "ABC123.txt" );
        }

        [ Fact ]
        public void Should_give_correct_blob_filesystem_provider()
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

            var firstProvider = sut.OpenFile( "enchilada://blob_filesystem/SampleContent.txt" );
            firstProvider.Should().BeOfType<BlobStorageFile>();
            firstProvider.RealPath.Should().Be( "http://127.0.0.1:10000/devstoreaccount1/test/SampleContent.txt" );
            firstProvider.Name.Should().Be( "SampleContent.txt" );

            var secondProvider = sut.OpenFile( "enchilada://another_filesystem/SampleContent.txt" );
            secondProvider.Should().BeOfType<BlobStorageFile>();
            secondProvider.RealPath.Should().Be( "http://127.0.0.1:10000/devstoreaccount1/test123/SampleContent.txt" );
            secondProvider.Name.Should().Be( "SampleContent.txt" );
        }
    }
}