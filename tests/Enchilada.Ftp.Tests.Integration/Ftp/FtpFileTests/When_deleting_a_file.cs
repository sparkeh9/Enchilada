namespace Enchilada.Ftp.Tests.Integration.Ftp.FtpFileTests
{
    using System;
    using System.Threading.Tasks;
    using Helpers;
    using Xunit;
    using Shouldly;
    using Xunit.Abstractions;

    public class When_deleting_a_file : FtpTestBase
    {
        public When_deleting_a_file(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task Should_delete_file_properly()
        {
            string fileName = $"{Guid.NewGuid()}.txt";

            var ftpConfig = FtpConfig;

            await ResourceHelpers.CreateFileWithContentAsync(fileName, "test", ftpConfig, Logger);
            using (var localFtpClient = ResourceHelpers.GetLocalFtpClient(ftpConfig, Logger))
            {
                localFtpClient.Logger = Logger;
                var sut = new FtpFile(localFtpClient, fileName);

                sut.Exists.ShouldBeTrue();

                await sut.DeleteAsync();

                sut.Exists.ShouldBeFalse();
            }
        }


        [Fact]
        public async Task Should_not_blow_up_if_file_does_not_exist()
        {
            string fileName = $"{Guid.NewGuid()}.txt";
            var ftpConfig = FtpConfig;
            using (var localFtpClient = ResourceHelpers.GetLocalFtpClient(ftpConfig, Logger))
            {
                var sut = new FtpFile(localFtpClient, fileName);

                sut.Exists.ShouldBeFalse();

                await sut.DeleteAsync();
            }
        }
    }
}