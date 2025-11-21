namespace Enchilada.Tests.Integration.FileSystem.FilesystemFileProviderTests
{
    using System.IO;
    using Filesystem;
    using Helpers;
    using Xunit;
    using Shouldly;

    public class When_opening_a_directory
    {
        [ Fact ]
        public void Should_give_deep_path()
        {
            string filesystemPath = ResourceHelpers.GetResourceDirectoryInfo( "test_filesystem" ).FullName;
            var filesystemProvider = new FilesystemFileProvider( new FilesystemAdapterConfiguration
                                                                 {
                                                                     Directory = filesystemPath
                                                                 }, "level1/level2" );

            filesystemProvider.ShouldBeOfType<FilesystemFileProvider>();
            filesystemProvider.RootDirectory.RealPath.ShouldBe( string.Format( "{0}{1}level1{1}level2", filesystemPath, Path.DirectorySeparatorChar ) );
        }
    }
}