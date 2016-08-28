namespace Enchilada.Tests.Unit.FileSystem.FilesystemDirectoryTests
{
    using System;
    using Filesystem;
    using FluentAssertions;
    using Helpers;
    using Xunit;

    public class When_creating_a_directory
    {
        [ Fact ]
        public void Should_create_a_directory()
        {
            var resourceDirectory = new FilesystemDirectory( ResourceHelpers.GetResourceDirectoryInfo() );

            var sut = resourceDirectory.GetDirectory( $"testfolder_{Guid.NewGuid()}" );
            sut.Exists.Should().BeFalse();

            sut.CreateDirectory();

            sut.Exists.Should().BeTrue();

            sut.DeleteAsync();
        }

        [ Fact ]
        public void Should_not_blow_up_if_folder_already_exists()
        {
            var resourceDirectory = new FilesystemDirectory( ResourceHelpers.GetResourceDirectoryInfo() );

            var sut = resourceDirectory.GetDirectory( $"testfolder_{Guid.NewGuid()}" );

            sut.CreateDirectory();
            sut.Exists.Should().BeTrue();
            sut.CreateDirectory();

            sut.DeleteAsync();
        }
    }
}