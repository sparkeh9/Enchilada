namespace Enchilada.Tests.Integration.Infrastructure.EnchiladaFileProviderResolverTests
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Enchilada.Infrastructure;
    using Enchilada.Infrastructure.Exceptions;
    using Filesystem;
    using Helpers;
    using Xunit;
    using Shouldly;

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
                                                                 new FilesystemAdapterConfiguration
                                                                 {
                                                                     AdapterName = "test_filesystem",
                                                                     Directory = ResourceHelpers.GetResourceDirectoryInfo().FullName
                                                                 }
                                                             }
                                                         } );

            Assert.Throws<UriFormatException>( () => sut.OpenDirectoryReference( "abc" ) );
        }

        [ Fact ]
        public void Should_give_filesystem_provider()
        {
            string filesystemPath = ResourceHelpers.GetResourceDirectoryInfo( "test_filesystem" ).FullName;
            var sut = new EnchiladaFileProviderResolver( new EnchiladaConfiguration
                                                         {
                                                             Adapters = new List<IEnchiladaAdapterConfiguration>
                                                             {
                                                                 new FilesystemAdapterConfiguration
                                                                 {
                                                                     AdapterName = "test_filesystem",
                                                                     Directory = filesystemPath
                                                                 }
                                                             }
                                                         } );
            var provider = sut.OpenDirectoryReference( "enchilada://test_filesystem/" );
            provider.ShouldBeOfType<FilesystemDirectory>();
            provider.RealPath.ShouldBe( filesystemPath );
        }

        [ Fact ]
        public void Should_give_correct_filesystem_provider()
        {
            string firstProviderPath = ResourceHelpers.GetResourceDirectoryInfo( "test_filesystem" ).FullName;
            string secondProviderPath = ResourceHelpers.GetResourceDirectoryInfo( "another_filesystem" ).FullName;
            var sut = new EnchiladaFileProviderResolver( new EnchiladaConfiguration
                                                         {
                                                             Adapters = new List<IEnchiladaAdapterConfiguration>
                                                             {
                                                                 new FilesystemAdapterConfiguration
                                                                 {
                                                                     AdapterName = "test_filesystem",
                                                                     Directory = firstProviderPath
                                                                 },
                                                                 new FilesystemAdapterConfiguration
                                                                 {
                                                                     AdapterName = "another_filesystem",
                                                                     Directory = secondProviderPath
                                                                 }
                                                             }
                                                         } );

            var firstProvider = sut.OpenDirectoryReference( "enchilada://test_filesystem/" );
            firstProvider.ShouldBeOfType<FilesystemDirectory>();
            firstProvider.RealPath.ShouldBe( firstProviderPath );

            var secondProvider = sut.OpenDirectoryReference( "enchilada://another_filesystem/" );
            secondProvider.ShouldBeOfType<FilesystemDirectory>();
            secondProvider.RealPath.ShouldBe( secondProviderPath );
        }


        [ Fact ]
        public void Should_allow_path_with_extension_in_name()
        {
            string filesystemPath = ResourceHelpers.GetResourceDirectoryInfo( "test_filesystem" ).FullName;
            var sut = new EnchiladaFileProviderResolver( new EnchiladaConfiguration
                                                         {
                                                             Adapters = new List<IEnchiladaAdapterConfiguration>
                                                             {
                                                                 new FilesystemAdapterConfiguration
                                                                 {
                                                                     AdapterName = "test_filesystem",
                                                                     Directory = filesystemPath
                                                                 }
                                                             }
                                                         } );
            var provider = sut.OpenDirectoryReference( "enchilada://test_filesystem/test.folder/" );
            provider.ShouldBeOfType<FilesystemDirectory>();
            provider.RealPath.ShouldEndWith( "test.folder" );
        }
    }
}