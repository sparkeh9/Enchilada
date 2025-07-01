namespace Enchilada.Azure.Tests.Integration.Infrastructure.EnchiladaFileProviderResolverTests
{
    using System;
    using System.Collections.Generic;
    using Azure.BlobStorage;
    using Configuration;
    using Enchilada.Infrastructure;
    using Filesystem;
    using Xunit;
    using Shouldly;

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
                        ConnectionString = AzuriteTestcontainer.GetConnectionString(),
                        ContainerReference = "test",
                        IsPublicAccess = true
                    }
                }
            } );

            Assert.Throws<UriFormatException>( () => sut.OpenFileReference( "abc" ) );
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

            var provider = sut.OpenFileReference( "enchilada://blob_filesystem/ABC123.txt" );
            provider.ShouldBeOfType<BlobStorageFile>();
            provider.RealPath.ShouldEndWith( "/devstoreaccount1/test/ABC123.txt" );
            provider.Name.ShouldBe( "ABC123.txt" );
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
                    }
                }
            } );

            var firstProvider = sut.OpenFileReference( "enchilada://blob_filesystem/SampleContent.txt" );
            firstProvider.ShouldBeOfType<BlobStorageFile>();
            firstProvider.RealPath.ShouldEndWith( "/devstoreaccount1/test/SampleContent.txt" );
            firstProvider.Name.ShouldBe( "SampleContent.txt" );

            var secondProvider = sut.OpenFileReference( "enchilada://another_filesystem/SampleContent.txt" );
            secondProvider.ShouldBeOfType<BlobStorageFile>();
            secondProvider.RealPath.ShouldEndWith( "/devstoreaccount1/enchilada-test/SampleContent.txt" );
            secondProvider.Name.ShouldBe( "SampleContent.txt" );
        }
    }
}