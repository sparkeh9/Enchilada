namespace Enchilada.Filesystem
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Infrastructure.Interface;

    public class FilesystemDirectory : IDirectory
    {
        protected DirectoryInfo BackingDirectory;

        public string Name => BackingDirectory.Name;
        public DateTime? LastModified => BackingDirectory.LastWriteTime;

        public bool IsDirectory => true;

        public string RealPath => BackingDirectory.FullName;

        public IEnumerable<IFile> Files => from file in BackingDirectory.GetFiles()
                                           select new FilesystemFile( file );

        public IEnumerable<IDirectory> Directories => from directory in BackingDirectory.GetDirectories()
                                                      select new FilesystemDirectory( directory );

        public bool Exists => Directory.Exists( BackingDirectory.FullName );

        public FilesystemDirectory( DirectoryInfo directoryInfo )
        {
            BackingDirectory = directoryInfo;
        }

        public async Task DeleteAsync( bool recursive = true )
        {
            try
            {
                var task = new Task( BackingDirectory.Delete );
                task.Start();
                await task;
            }
            catch ( Exception )
            {
                // ignored
            }
        }

        public IEnumerator<INode> GetEnumerator()
        {
            return Directories
                .Union( Files.Cast<INode>() )
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IReadOnlyCollection<IDirectory> GetDirectories( string path )
        {
            var list = BackingDirectory.GetDirectories( path, SearchOption.TopDirectoryOnly )
                                       .Select( x => new FilesystemDirectory( x ) )
                                       .ToList();
            return new ReadOnlyCollection<FilesystemDirectory>( list );
        }

        public IDirectory GetDirectory( string path )
        {
            return new FilesystemDirectory( new DirectoryInfo( $"{BackingDirectory.FullName}/{path}" ) );
        }

        public IFile GetFile( string fileName )
        {
            return new FilesystemFile( new FileInfo( Path.Combine( RealPath, fileName ) ) );
        }

        public void CreateDirectory()
        {
            BackingDirectory.Create();
            BackingDirectory.Refresh();
        }
    }
}