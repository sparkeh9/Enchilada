namespace Enchilada.Tests.Unit.FileSystem.FilesystemDirectoryTests
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Filesystem;
    using FluentAssertions;
    using Helpers;
    using Xunit;

    public class When_deleting_a_directory
    {
        [ Fact ]
        public async Task Should_delete_a_directory()
        {
            var resourceDirectory = new FilesystemDirectory( ResourceHelpers.GetResourceDirectoryInfo() );

            var sut = resourceDirectory.GetDirectory( $"testfolder_{Guid.NewGuid()}" );
            sut.CreateDirectory();

            sut.Exists.Should().BeTrue();
            await sut.DeleteAsync();

            sut.Exists.Should().BeFalse();
        }

        [ Fact ]
        public async Task Should_not_throw_exception_when_directory_not_present()
        {
            var resourceDirectory = new FilesystemDirectory( ResourceHelpers.GetResourceDirectoryInfo() );

            var sut = resourceDirectory.GetDirectory( $"does_not_exist_{Guid.NewGuid()}" );
            await sut.DeleteAsync();
        }
    }
}