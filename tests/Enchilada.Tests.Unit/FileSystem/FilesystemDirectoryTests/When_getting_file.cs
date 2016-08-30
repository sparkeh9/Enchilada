namespace Enchilada.Tests.Unit.FileSystem.FilesystemDirectoryTests
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Filesystem;
    using FluentAssertions;
    using Helpers;
    using Xunit;

    public class When_getting_file
    {
        private const string WRITE_CONTENT = "this is a test - 88e416a7-f5fe-4f21-9c5e-168ae5efb87c";

        [ Fact ]
        public async Task Should_give_handle_to_nonexistent_file()
        {
            const string tempFileName = "test.txt";

            var resourceDirectory = new FilesystemDirectory( ResourceHelpers.GetResourceDirectoryInfo() );
            var directory = resourceDirectory.GetDirectory( $"testfolder_{Guid.NewGuid()}" );

            var tempFile = directory.GetFile( tempFileName );

            tempFile.Exists.Should().BeFalse();
            tempFile.RealPath.Should().Be( Path.Combine( directory.RealPath, tempFileName ) );

            await directory.DeleteAsync();
        }

        [ Fact ]
        public async Task Should_be_able_to_write_to_file()
        {
            const string tempFileName = "test.txt";

            var resourceDirectory = new FilesystemDirectory( ResourceHelpers.GetResourceDirectoryInfo() );
            var directory = resourceDirectory.GetDirectory( $"level1/testfolder_{Guid.NewGuid()}" );

            var tempFile = directory.GetFile( tempFileName );

            using ( var stream = await tempFile.OpenWriteAsync() )
            using ( var writer = new StreamWriter( stream ) )
            {
                writer.Write( WRITE_CONTENT );
            }

            File.Exists( tempFile.RealPath ).Should().BeTrue();
            File.Delete( tempFile.RealPath );

            await directory.DeleteAsync();
        }
    }
}