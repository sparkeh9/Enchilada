namespace Enchilada.Ftp.Tests.Integration.Ftp.FtpDirectoryTests
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Filesystem;
    using Shouldly;
    using Helpers;
    using Xunit;
    using Xunit.Abstractions;

    public class When_getting_file : FtpTestBase
    {
        private const string WRITE_CONTENT = "this is a test - 88e416a7-f5fe-4f21-9c5e-168ae5efb87c";

        public When_getting_file(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task Should_give_handle_to_nonexistent_file()
        {
            const string tempFileName = "test.txt";

            var resourceDirectory = new FilesystemDirectory(ResourceHelpers.GetResourceDirectoryInfo());
            var directory = resourceDirectory.GetDirectory($"testfolder_{Guid.NewGuid()}");

            var tempFile = directory.GetFile(tempFileName);

            tempFile.Exists.ShouldBeFalse();
            tempFile.RealPath.ShouldBe(Path.Combine(directory.RealPath, tempFileName));

            await directory.DeleteAsync();
        }

        [Fact]
        public async Task Should_be_able_to_write_to_file()
        {
            const string tempFileName = "test.txt";

            var resourceDirectory = new FilesystemDirectory(ResourceHelpers.GetResourceDirectoryInfo());
            var directory = resourceDirectory.GetDirectory($"level1/testfolder_{Guid.NewGuid()}");

            var tempFile = directory.GetFile(tempFileName);

            using (var stream = await tempFile.OpenWriteAsync())
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(WRITE_CONTENT);
            }

            File.Exists(tempFile.RealPath).ShouldBeTrue();
            File.Delete(tempFile.RealPath);

            await directory.DeleteAsync();
        }
    }
}