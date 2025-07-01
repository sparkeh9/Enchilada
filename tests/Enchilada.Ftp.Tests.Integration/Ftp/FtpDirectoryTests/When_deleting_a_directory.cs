namespace Enchilada.Ftp.Tests.Integration.Ftp.FtpDirectoryTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Helpers;
    using Microsoft.Extensions.Configuration;
    using Xunit;
    using Xunit.Abstractions;

    public class When_deleting_a_directory : FtpTestBase
    {
        public When_deleting_a_directory(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public async Task Should_delete_deep_structure()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(System.AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            await ResourceHelpers.CreateFileWithContentAsync($"folder1/folder2/folder3/{Guid.NewGuid()}.txt", "stuff", config, Logger);

            using (var ftpClient = ResourceHelpers.GetLocalFtpClient(config, Logger))
            {
                ftpClient.Logger = Logger;
                var sut = new FtpDirectory(ftpClient, "folder1");
                await sut.DeleteAsync();

                var folders = await sut.GetDirectoriesAsync();
                folders.Any(x => x.Name == "folder1").Should().BeFalse();
            }
        }


//        [ Fact ]
//        public async Task Should_delete_a_directory()
//        {
//            var file = new BlobStorageFile( ResourceHelpers.GetLocalDevelopmentContainer(), $"folder1/folder2/folder3/{Guid.NewGuid()}.txt" );
//
//            using ( var stream = await file.OpenWriteAsync() )
//            using ( var writer = new StreamWriter( stream ) )
//            {
//                writer.Write( "test" );
//            }
//
//            var sut = new BlobStorageDirectory( ResourceHelpers.GetLocalDevelopmentContainer(), "folder1" );
//
//            sut.Exists.Should().BeTrue();
//            await sut.DeleteAsync();
//        }
//
//        [ Fact ]
//        public async Task Should_not_throw_exception_when_directory_not_present()
//        {
//            var sut = new BlobStorageDirectory( ResourceHelpers.GetLocalDevelopmentContainer(), "does_not_exist" );
//            await sut.DeleteAsync();
//        }
    }
}