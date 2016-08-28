namespace Enchilada.Tests.Unit.FileSystem.FilesystemFileTests
{
    using System;
    using System.IO;
    using Filesystem;
    using FluentAssertions;
    using Helpers;
    using Xunit;

    public class When_deleting_a_file
    {
        [ Fact ]
        public void Should_delete_file_properly()
        {
            string tempFileInfo = $"{ResourceHelpers.GetTempFilePath()}/{Guid.NewGuid()}.txt";
            var sut = new FilesystemFile( new FileInfo( tempFileInfo ) );

            File.Exists( sut.RealPath ).Should().BeFalse();

            using ( sut.OpenWrite() ) {}

            File.Exists( sut.RealPath ).Should().BeTrue();

            sut.Delete();

            File.Exists( sut.RealPath ).Should().BeFalse();
        }


        [ Fact ]
        public void Should_not_blow_up_if_file_does_not_exist()
        {
            string tempFileInfo = $"{ResourceHelpers.GetTempFilePath()}/{Guid.NewGuid()}.txt";
            var sut = new FilesystemFile( new FileInfo( tempFileInfo ) );

            File.Exists( sut.RealPath ).Should().BeFalse();

            sut.Delete();
        }
    }
}