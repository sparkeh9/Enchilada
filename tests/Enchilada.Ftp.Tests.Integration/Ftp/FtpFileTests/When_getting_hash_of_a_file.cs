namespace Enchilada.Ftp.Tests.Integration.Ftp.FtpFileTests
{
    using System;
    using System.Threading.Tasks;
    using Helpers;
    using Xunit;
    using Shouldly;
    using Xunit.Abstractions;

    public class When_getting_hash_of_a_file : FtpTestBase
    {
        private const string FileContent = "When_getting_hash_of_a_file";

        public When_getting_hash_of_a_file(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task Should_give_md5_hash_of_file()
        {
            string fileName = $"{Guid.NewGuid()}.txt";

            var ftpConfig = FtpConfig;

            await ResourceHelpers.CreateFileWithContentAsync(fileName, FileContent, ftpConfig, Logger);
            using (var localFtpClient = ResourceHelpers.GetLocalFtpClient(ftpConfig, Logger))
            {
                localFtpClient.Logger = Logger;
                var sut = new FtpFile(localFtpClient, fileName);
                var hash = await sut.GetHashAsync();

                hash.ShouldNotBeEmpty();
                hash.ShouldBe("081cb72eaaacae3df4502708ff956d23");

                await sut.DeleteAsync();
            }
        }
    }
}