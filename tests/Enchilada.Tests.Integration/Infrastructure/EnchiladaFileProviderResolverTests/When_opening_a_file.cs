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

    public class When_opening_a_file
    {
        [ Fact ]
        public void Should_throw_exception_when_no_configuration_provided()
        {
            var sut = new EnchiladaFileProviderResolver();
            Assert.Throws<ConfigurationMissingException>( () => sut.OpenFile( "abc" ) );
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

            Assert.Throws<UriFormatException>( () => sut.OpenFile( "abc" ) );
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

            var firstProvider = sut.OpenFile( "enchilada://test_filesystem/abc.txt" );
            firstProvider.Should().BeOfType<FilesystemFile>();
            firstProvider.RealPath.Should().Be( $"{firstProviderPath}\\abc.txt" );

            var secondProvider = sut.OpenFile( "enchilada://another_filesystem/123.txt" );
            secondProvider.Should().BeOfType<FilesystemFile>();
            secondProvider.RealPath.Should().Be( $"{secondProviderPath}\\123.txt" );
        }
    }
}