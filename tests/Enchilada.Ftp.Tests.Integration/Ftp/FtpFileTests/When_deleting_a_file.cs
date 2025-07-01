namespace Enchilada.Ftp.Tests.Integration.Ftp.FtpFileTests
{
    using System;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.Extensions.Configuration;
    using Xunit;
    using FluentAssertions;
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

            var config = new ConfigurationBuilder()
                .SetBasePath(System.AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            await ResourceHelpers.CreateFileWithContentAsync(fileName, "test", config, Logger);
            using (var localFtpClient = ResourceHelpers.GetLocalFtpClient(config, Logger))
            {
                localFtpClient.Logger = Logger;
                var sut = new FtpFile(localFtpClient, fileName);

                sut.Exists.Should().BeTrue();

                await sut.DeleteAsync();

                sut.Exists.Should().BeFalse();
            }
        }


        [Fact]
        public async Task Should_not_blow_up_if_file_does_not_exist()
        {
            string fileName = $"{Guid.NewGuid()}.txt";
            var config = new ConfigurationBuilder()
                .SetBasePath(System.AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            using (var localFtpClient = ResourceHelpers.GetLocalFtpClient(config, Logger))
            {
                var sut = new FtpFile(localFtpClient, fileName);

                sut.Exists.Should().BeFalse();

                await sut.DeleteAsync();
            }
        }
    }
}