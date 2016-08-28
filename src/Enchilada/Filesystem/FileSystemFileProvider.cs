namespace Enchilada.Filesystem
{
    using System;
    using System.IO;
    using Infrastructure;
    using Infrastructure.Interface;

    public class FilesystemFileProvider : IFileProvider
    {
        private readonly char[] DirectorySeparators = { '/', '\\' };
        protected DirectoryInfo BackingRootDirectory;

        public IDirectory RootDirectory { get; protected set; }
        public IFile File { get; protected set; }

        public bool IsDirectory => !IsFile;
        public bool IsFile => File != null;

        public FilesystemFileProvider( FilesystemAdapterConfiguration configuration, string directory )
        {
            string combinedPath = Path.Combine( configuration.Directory, directory.Trim( DirectorySeparators ) );

            if ( System.IO.File.Exists( combinedPath ) )
            {
                SetRootDirectory( Directory.GetParent( combinedPath ).FullName );
                File = RootDirectory.GetFile( Path.GetFileName( combinedPath ) );
            }
            else
            {
                if ( Path.HasExtension( combinedPath ) )
                {
                    SetRootDirectory( Directory.GetParent( combinedPath ).FullName );
                    return;
                }

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