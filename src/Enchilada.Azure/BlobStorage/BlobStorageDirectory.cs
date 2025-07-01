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
    using global::Azure.Storage.Blobs.Models;

    public class BlobStorageDirectory : IDirectory
    {
        private readonly BlobContainerClient BlobContainer;
        private readonly string Path;

        public string Name => Path;
        public DateTime? LastModified => GetBlobFiles().OrderBy( x => x.LastModified ).FirstOrDefault()?.LastModified;

        public bool IsDirectory => true;

        public string RealPath => BlobContainer.Uri.ToString().TrimEnd('/') + "/" + Path.TrimStart('/');

        public bool Exists => true;

        public BlobStorageDirectory( BlobContainerClient blobContainer, string path )
        {
            BlobContainer = blobContainer;
            Path = path;
        }

        public async Task DeleteAsync()
        {
            var blobs = BlobContainer.GetBlobsAsync( prefix: Path );
            await foreach ( var blob in blobs )
            {
                var blobClient = BlobContainer.GetBlobClient( blob.Name );
                await blobClient.DeleteIfExistsAsync();
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
            var hierarchyItems = BlobContainer.GetBlobsByHierarchy( prefix: Path, delimiter: "/" );
            var directories = new HashSet<string>();
            
            foreach ( var item in hierarchyItems )
            {
                if ( item.IsPrefix )
                {
                    var directoryPath = item.Prefix.TrimEnd('/');
                    if ( path == null || directoryPath.EndsWith( $"/{path}" ) || directoryPath == path )
                    {
                        if ( !directories.Contains( directoryPath ) )
                        {
                            directories.Add( directoryPath );
                            yield return new BlobStorageDirectory( BlobContainer, directoryPath );
                        }
                    }
                }
            }
        }

        private IEnumerable<IFile> GetBlobFiles()
        {
            var blobs = BlobContainer.GetBlobs( prefix: Path );
            
            foreach ( var blob in blobs )
            {
                // Only return files that are directly in this directory, not in subdirectories
                var relativePath = blob.Name.Substring( Path.Length ).TrimStart('/');
                if ( !relativePath.Contains('/') )
                {
                    yield return new BlobStorageFile( BlobContainer, blob.Name );
                }
            }
        }
    }
}