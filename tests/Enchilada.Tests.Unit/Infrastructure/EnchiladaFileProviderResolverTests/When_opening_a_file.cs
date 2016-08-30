namespace Enchilada.Tests.Unit.Infrastructure.EnchiladaFileProviderResolverTests
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Enchilada.Infrastructure;
    using Enchilada.Infrastructure.Exceptions;
    using Filesystem;
    using FluentAssertions;
    using Helpers;
    using Xunit;

    public class When_opening_a_file
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
                                                                 new FilesystemAdapterConfiguration
                                                                 {
                                                                     AdapterName = "test_filesystem",
                                                                     Directory = ResourceHelpers.GetResourceDirectoryInfo().FullName
                                                                 }
                                                             }
                                                         } );

            Assert.Throws<UriFormatException>( () => sut.OpenProvider( "abc" ) );
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

            var firstProvider = sut.OpenProvider( "enchilada://test_filesystem/" );
            firstProvider.Should().BeOfType<FilesystemFileProvider>();
            firstProvider.RootDirectory.RealPath.Should().Be( firstProviderPath );

            var secondProvider = sut.OpenProvider( "enchilada://another_filesystem/" );
            secondProvider.Should().BeOfType<FilesystemFileProvider>();
            secondProvider.RootDirectory.RealPath.Should().Be( secondProviderPath );
        }
    }
}