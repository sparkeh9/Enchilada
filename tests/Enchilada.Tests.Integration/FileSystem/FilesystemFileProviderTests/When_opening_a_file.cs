namespace Enchilada.Tests.Integration.FileSystem.FilesystemFileProviderTests
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    using Filesystem;
    using Helpers;
    using Xunit;
    using FluentAssertions;

    public class When_opening_a_file
    {
        [ Fact ]
        public void Should_have_root_directory_as_directory_containing_file()
        {
            string filesystemPath = ResourceHelpers.GetResourceDirectoryInfo().FullName;
            var filesystemProvider = new FilesystemFileProvider( new FilesystemAdapterConfiguration
                                                                 {
                                                                     Directory = filesystemPath
                                                                 }, "level1/level2/level2content.txt" );

            filesystemProvider.RootDirectory.RealPath.Should().Be( string.Format( "{0}level1{1}level2", filesystemPath, Path.DirectorySeparatorChar ) );
        }

        [ Fact ]
        public async Task Should_present_file()
        {
            string filesystemPath = ResourceHelpers.GetResourceDirectoryInfo().FullName;
            var filesystemProvider = new FilesystemFileProvider( new FilesystemAdapterConfiguration
                                                                 {
                                                                     Directory = filesystemPath
                                                                 }, "level1/level2/level2content.txt" );

            filesystemProvider.IsFile.Should().BeTrue();
            filesystemProvider.File.Should().NotBeNull();

            var bytes = await filesystemProvider.File.ReadToEndAsync();
            string contents = Encoding.UTF8.GetString( bytes );

            contents.Should().StartWith( "Lorem ipsum" );
        }

        [ Fact ]
        public void Should_present_file_when_not_created_yet()
        {
            string filesystemPath = ResourceHelpers.GetResourceDirectoryInfo().FullName;
            var filesystemProvider = new FilesystemFileProvider( new FilesystemAdapterConfiguration
                                                                 {
                                                                     Directory = filesystemPath
                                                                 }, $"level1/level2/not_a_real_file_{Guid.NewGuid()}.txt" );

            filesystemProvider.IsFile.Should().BeTrue();
            filesystemProvider.File.Should().NotBeNull();
            filesystemProvider.File.Exists.Should().BeFalse();
            filesystemProvider.RootDirectory.Should().NotBeNull();
            filesystemProvider.RootDirectory.RealPath.Should().Be( string.Format( "{0}level1{1}level2", filesystemPath, Path.DirectorySeparatorChar ) );
        }
    }
}