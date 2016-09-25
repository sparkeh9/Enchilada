namespace Enchilada.Ftp.Tests.Integration.Ftp.FtpFileTests
{
    using System;
    using System.Threading.Tasks;
    using Helpers;
    using Xunit;
    using FluentAssertions;

    public class When_deleting_a_file : FtpTestBase
    {
        [ Fact ]
        public async Task Should_delete_file_properly()
        {
            string fileName = $"{Guid.NewGuid()}.txt";

            await ResourceHelpers.CreateFileWithContentAsync( fileName, "test" );
            using ( var localFtpClient = ResourceHelpers.GetLocalFtpClient() )
            {
                localFtpClient.Logger = Logger;
                var sut = new FtpFile( localFtpClient, fileName );

                sut.Exists.Should().BeTrue();

                await sut.DeleteAsync();

                sut.Exists.Should().BeFalse();
            }
        }


        [ Fact ]
        public async Task Should_not_blow_up_if_file_does_not_exist()
        {
            string fileName = $"{Guid.NewGuid()}.txt";
            using ( var localFtpClient = ResourceHelpers.GetLocalFtpClient() )
            {
                var sut = new FtpFile( localFtpClient, fileName );

                sut.Exists.Should().BeFalse();

                await sut.DeleteAsync();
            }
        }
    }
}