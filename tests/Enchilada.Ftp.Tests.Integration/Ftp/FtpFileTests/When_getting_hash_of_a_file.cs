namespace Enchilada.Ftp.Tests.Integration.Ftp.FtpFileTests
{
    using System;
    using System.Threading.Tasks;
    using Helpers;
    using Xunit;
    using FluentAssertions;

    public class When_getting_hash_of_a_file : FtpTestBase
    {
        private const string FileContent = "When_getting_hash_of_a_file";

        [ Fact ]
        public async Task Should_give_md5_hash_of_file()
        {
            string fileName = $"{Guid.NewGuid()}.txt";

            await ResourceHelpers.CreateFileWithContentAsync( fileName, FileContent, Logger );
            using ( var localFtpClient = ResourceHelpers.GetLocalFtpClient( Logger ) )
            {
                localFtpClient.Logger = Logger;
                var sut = new FtpFile( localFtpClient, fileName );
                var hash = await sut.GetHashAsync();

                hash.Should().NotBeEmpty();
                hash.Should().Be( "081cb72eaaacae3df4502708ff956d23" );

                await sut.DeleteAsync();
            }
        }
    }
}