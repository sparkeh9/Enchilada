namespace Enchilada.Azure.BlobStorage
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Infrastructure.Extensions;
    using Infrastructure.Interface;
    using global::Azure.Storage.Blobs;

    public class BlobStorageDirectory : IDirectory
    {
        private readonly BlobContainerClient BlobContainer;
        private readonly string Path;

        // Ensures the prefix used with Azure listing calls ends with a slash when targeting a sub-directory.
        private string Prefix => string.IsNullOrEmpty( Path )
                                    ? string.Empty
                                    : Path.TrimEnd( '/' ) + "/";

        public string Name => string.IsNullOrEmpty( Path ) ? $"{BlobContainer.Name}/" : $"{Path.TrimEnd('/')}/";
        public DateTime? LastModified => GetBlobFiles().OrderBy( x => x.LastModified ).FirstOrDefault()?.LastModified;

        public bool IsDirectory => true;

        public string RealPath => string.IsNullOrEmpty( Path )
            ? BlobContainer.Uri.ToString().StripDoubleSlash()
            : ($"{BlobContainer.Uri.ToString().StripDoubleSlash()}/{Path.TrimEnd('/')}/").StripDoubleSlash();

        public bool Exists => true;

        public BlobStorageDirectory( BlobContainerClient blobContainer, string path )
        {
            BlobContainer = blobContainer;
            Path = path;
        }

        public async Task DeleteAsync()
        {
            await foreach ( var blob in BlobContainer.GetBlobsAsync( prefix: Prefix ) )
            {
                await BlobContainer.DeleteBlobIfExistsAsync( blob.Name );
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


        public async Task<IReadOnlyCollection<IDirectory>> GetDirectoriesAsync()
        {
            await Task.Delay( 0 );
            return new ReadOnlyCollection<IDirectory>( GetBlobDirectories().ToList() );
        }

        public async Task<IReadOnlyCollection<IFile>> GetFilesAsync()
        {
            await Task.Delay( 0 );
            return new ReadOnlyCollection<IFile>( GetBlobFiles().ToList() );
        }

        public IDirectory GetDirectory( string path )
        {
            return new BlobStorageDirectory( BlobContainer, path );
        }

        public IFile GetFile( string fileName )
        {
            return new BlobStorageFile( BlobContainer, fileName );
        }

        public async Task<IReadOnlyCollection<INode>> GetAllNodesAsync()
        {
            var directories = await GetDirectoriesAsync();
            var files = await GetFilesAsync();

            return directories.Union( files.Cast<INode>() )
                              .ToList()
                              .AsReadOnly();
        }

        public Task CreateDirectoryAsync()
        {
            return Task.Delay( 0 );
        }

        private IEnumerable<IDirectory> GetBlobDirectories( string path = null )
        {
            var results = BlobContainer.GetBlobsByHierarchy( prefix: Prefix, delimiter: "/" );

            foreach ( var item in results )
            {
                if ( item.IsPrefix && ( path == null || item.Prefix.EndsWith( $"{path}/" ) ) )
                {
                    yield return new BlobStorageDirectory( BlobContainer, item.Prefix );
                }
            }
        }

        private IEnumerable<IFile> GetBlobFiles()
        {
            var results = BlobContainer.GetBlobsByHierarchy( prefix: Prefix, delimiter: "/" );

            foreach ( var item in results )
            {
                if ( !item.IsPrefix )
                {
                    yield return new BlobStorageFile( BlobContainer, item.Blob.Name );
                }
            }
        }
    }
}