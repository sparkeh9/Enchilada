namespace Enchilada.Tests.Integration.FileSystem.FilesystemFileTests
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Filesystem;
    using Helpers;
    using Xunit;
    using Shouldly;

    public class When_deleting_a_file
    {
        [ Fact ]
        public async Task Should_delete_file_properly()
        {
            string tempFileInfo = $"{ResourceHelpers.GetTempFilePath()}/{Guid.NewGuid()}.txt";
            var sut = new FilesystemFile( new FileInfo( tempFileInfo ) );

            File.Exists( sut.RealPath ).ShouldBeFalse();

            using ( await sut.OpenWriteAsync() ) {}

            File.Exists( sut.RealPath ).ShouldBeTrue();

            await sut.DeleteAsync();

            File.Exists( sut.RealPath ).ShouldBeFalse();
        }


        [ Fact ]
        public async Task Should_not_blow_up_if_file_does_not_exist()
        {
            string tempFileInfo = $"{ResourceHelpers.GetTempFilePath()}/{Guid.NewGuid()}.txt";
            var sut = new FilesystemFile( new FileInfo( tempFileInfo ) );

            File.Exists( sut.RealPath ).ShouldBeFalse();

            await sut.DeleteAsync();
        }
    }
}