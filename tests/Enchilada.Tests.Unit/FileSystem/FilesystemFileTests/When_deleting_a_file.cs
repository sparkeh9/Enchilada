namespace Enchilada.Tests.Unit.FileSystem.FilesystemFileTests
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Filesystem;
    using FluentAssertions;
    using Helpers;
    using Xunit;

    public class When_deleting_a_file
    {
        [ Fact ]
        public async Task Should_delete_file_properly()
        {
            string tempFileInfo = $"{ResourceHelpers.GetTempFilePath()}/{Guid.NewGuid()}.txt";
            var sut = new FilesystemFile( new FileInfo( tempFileInfo ) );

            File.Exists( sut.RealPath ).Should().BeFalse();

            using ( await sut.OpenWriteAsync() ) {}

            File.Exists( sut.RealPath ).Should().BeTrue();

            await sut.DeleteAsync();

            File.Exists( sut.RealPath ).Should().BeFalse();
        }


        [ Fact ]
        public async Task Should_not_blow_up_if_file_does_not_exist()
        {
            string tempFileInfo = $"{ResourceHelpers.GetTempFilePath()}/{Guid.NewGuid()}.txt";
            var sut = new FilesystemFile( new FileInfo( tempFileInfo ) );

            File.Exists( sut.RealPath ).Should().BeFalse();

            await sut.DeleteAsync();
        }
    }
}