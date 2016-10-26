namespace Enchilada.Filesystem
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
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
            return GetAllNodesAsync().GetAwaiter()
                                     .GetResult()
                                     .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public async Task<IReadOnlyCollection<INode>> GetAllNodesAsync()
        {
            var directories = await GetDirectoriesAsync();
            var files = await GetFilesAsync();

            return directories.Union( files.Cast<INode>() )
                              .ToList()
                              .AsReadOnly();
        }

        public async Task<IReadOnlyCollection<IDirectory>> GetDirectoriesAsync()
        {
            await Task.Delay( 0 );

            var list = BackingDirectory.GetDirectories()
                                       .Select( x => new FilesystemDirectory( x ) )
                                       .ToList();

            return list.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<IDirectory>> GetDirectoriesAsync( string path )
        {
            await Task.Delay( 0 );
            var list = BackingDirectory.GetDirectories( path, SearchOption.TopDirectoryOnly )
                                       .Select( x => new FilesystemDirectory( x ) )
                                       .ToList();
            return list.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<IFile>> GetFilesAsync()
        {
            await Task.Delay( 0 );
            var list = BackingDirectory.GetFiles()
                                       .Select( x => new FilesystemFile( x ) )
                                       .ToList();
            return list.AsReadOnly();
        }

        public async Task<IReadOnlyCollection<IFile>> GetFilesAsync( string path )
        {
            await Task.Delay( 0 );
            var list = ( from file in BackingDirectory.GetFiles()
                         select new FilesystemFile( file ) ).ToList();
            return list.AsReadOnly();
        }


        public IDirectory GetDirectory( string path )
        {
            return new FilesystemDirectory( new DirectoryInfo( $"{BackingDirectory.FullName}/{path}" ) );
        }

        public IFile GetFile( string fileName )
        {
            return new FilesystemFile( new FileInfo( Path.Combine( RealPath, fileName ) ) );
        }

        public async Task CreateDirectoryAsync()
        {
            BackingDirectory.Create();
            BackingDirectory.Refresh();
            await Task.Delay( 0 );
        }
    }
}