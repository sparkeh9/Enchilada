namespace Enchilada.Tests.Integration.Infrastructure.EnchiladaFileProviderResolverTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Configuration;
    using Enchilada.Infrastructure;
    using Enchilada.Infrastructure.Exceptions;
    using Filesystem;
    using Helpers;
    using Xunit;
    using Shouldly;

    public class When_opening_a_file
    {
        [ Fact ]
        public void Should_throw_exception_when_no_configuration_provided()
        {
            var sut = new EnchiladaFileProviderResolver();
            Assert.Throws<ConfigurationMissingException>( () => sut.OpenFileReference( "abc" ) );
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

            Assert.Throws<UriFormatException>( () => sut.OpenFileReference( "abc" ) );
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

            var firstProvider = sut.OpenFileReference( "enchilada://test_filesystem/abc.txt" );
            firstProvider.ShouldBeOfType<FilesystemFile>();
            firstProvider.RealPath.ShouldBe( Path.Combine( firstProviderPath, "abc.txt" ) );

            var secondProvider = sut.OpenFileReference( "enchilada://another_filesystem/123.txt" );
            secondProvider.ShouldBeOfType<FilesystemFile>();
            secondProvider.RealPath.ShouldBe( Path.Combine( secondProviderPath, "123.txt" ) );
        }
    }
}