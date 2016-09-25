namespace Enchilada.Tests.Integration.Helpers
{
    using System;
    using System.IO;

    public static class ResourceHelpers
    {
        public static DirectoryInfo GetResourceDirectoryInfo( string directory = "" )
        {
            return new DirectoryInfo( $"{AppContext.BaseDirectory}/Resources/{directory}" );
        }

        public static FileInfo GetResourceFileInfo( string filename )
        {
            return new FileInfo( $"{AppContext.BaseDirectory}/Resources/{filename}" );
        }

        public static string GetTempFilePath()
        {
            return Path.GetTempPath();
        }

        public static FileInfo GetTempFileInfo()
        {
            return new FileInfo( Path.GetTempFileName() );
        }
    }
}