namespace Enchilada.Tests.Integration.FileSystem.FilesystemDirectoryTests
{
    using System;
    using System.Threading.Tasks;
    using Filesystem;
    using Helpers;
    using Xunit;
    using Shouldly;

    public class When_creating_a_directory
    {
        [ Fact ]
        public async Task Should_create_a_directory()
        {
            var resourceDirectory = new FilesystemDirectory( ResourceHelpers.GetResourceDirectoryInfo() );

            var sut = resourceDirectory.GetDirectory( $"testfolder_{Guid.NewGuid()}" );
            sut.Exists.ShouldBeFalse();

            await sut.CreateDirectoryAsync();

            sut.Exists.ShouldBeTrue();

            await sut.DeleteAsync();
        }

        [ Fact ]
        public async Task Should_not_blow_up_if_folder_already_exists()
        {
            var resourceDirectory = new FilesystemDirectory( ResourceHelpers.GetResourceDirectoryInfo() );

            var sut = resourceDirectory.GetDirectory( $"testfolder_{Guid.NewGuid()}" );

            await sut.CreateDirectoryAsync();
            sut.Exists.ShouldBeTrue();
            await sut.CreateDirectoryAsync();

            await sut.DeleteAsync();
        }
    }
}