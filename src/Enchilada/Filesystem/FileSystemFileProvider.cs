namespace Enchilada.Filesystem
{
    using System;
    using System.IO;
    using Infrastructure.Interface;

    public class FilesystemFileProvider : IFileProvider
    {
        private readonly char[] DirectorySeparators = { '/', '\\' };
        protected DirectoryInfo BackingRootDirectory;

        public IDirectory RootDirectory { get; protected set; }
        public IFile File { get; protected set; }

        public bool IsDirectory => !IsFile;
        public bool IsFile => File != null;

        public FilesystemFileProvider( FilesystemAdapterConfiguration configuration, string filePath )
        {
            string combinedPath = Path.Combine( configuration.Directory, filePath.Trim( DirectorySeparators ) );

            if ( System.IO.File.Exists( combinedPath ) || Path.HasExtension( combinedPath ) )
            {
                SetRootDirectory( Directory.GetParent( combinedPath ).FullName );
                File = RootDirectory.GetFile( Path.GetFileName( combinedPath ) );
            }
            else
            {
                SetRootDirectory( combinedPath );
            }
        }

        private void SetRootDirectory( string path )
        {
            var directoryInfo = new DirectoryInfo( path );

            if ( directoryInfo == null )
                throw new ArgumentNullException( nameof( directoryInfo ) );

            BackingRootDirectory = directoryInfo;
            RootDirectory = new FilesystemDirectory( BackingRootDirectory );
        }
    }
}