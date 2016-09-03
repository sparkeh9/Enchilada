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
    using FluentAssertions;

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
                                                                 new FilesystemAdapterConfiguration
                                                                 {
                                                                     AdapterName = "test_filesystem",
                                                                     Directory = ResourceHelpers.GetResourceDirectoryInfo().FullName
                                                                 }
                                                             }
                                                         } );

            Assert.Throws<UriFormatException>( () => sut.OpenDirectory( "abc" ) );
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
            var provider = sut.OpenDirectory( "enchilada://test_filesystem/" );
            provider.Should().BeOfType<FilesystemDirectory>();
            provider.RealPath.Should().Be( filesystemPath );
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

            var firstProvider = sut.OpenDirectory( "enchilada://test_filesystem/" );
            firstProvider.Should().BeOfType<FilesystemDirectory>();
            firstProvider.RealPath.Should().Be( firstProviderPath );

            var secondProvider = sut.OpenDirectory( "enchilada://another_filesystem/" );
            secondProvider.Should().BeOfType<FilesystemDirectory>();
            secondProvider.RealPath.Should().Be( secondProviderPath );
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
            var provider = sut.OpenDirectory( "enchilada://test_filesystem/test.folder/" );
            provider.Should().BeOfType<FilesystemDirectory>();
            provider.RealPath.Should().EndWith( "test.folder" );
        }
    }
}