namespace Enchilada.Azure.BlobStorage
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Infrastructure.Extensions;
    using Infrastructure.Interface;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Specialized;
    using Azure.Storage.Blobs.Models;
    using FileMode = Infrastructure.FileMode;

    public class BlobStorageFile : IFile
    {
        private readonly BlobContainerClient BlobContainer;
        private readonly string FilePath;
        private BlobClient BlobClient => BlobContainer.GetBlobClient( FilePath );

        public string Name => Path.GetFileName( FilePath );

        public string Extension => Path.GetExtension( FilePath );
        public string RealPath => BlobClient.Uri.ToString();

        public long Size => BlobClient.GetProperties().Value.ContentLength;
        public DateTime? LastModified => BlobClient.GetProperties().Value.LastModified.LocalDateTime;
        public bool IsDirectory => false;
        public bool Exists => BlobClient.Exists().Value;

        public BlobStorageFile( BlobContainerClient blobContainer, string filePath )
        {
            BlobContainer = blobContainer;
            FilePath = filePath;
        }

        public async Task<Stream> OpenReadAsync()
        {
            return await BlobClient.OpenReadAsync();
        }

        public async Task<string> GetHashAsync()
        {
            using ( var stream = await OpenReadAsync() )
            {
                return await stream.ToMd5HashAsync();
            }
        }

        public async Task CopyFromAsync( IFile sourceFile )
        {
            await CopyFromAsync( await sourceFile.OpenReadAsync() );
        }

        public async Task CopyFromAsync( Stream sourceStream )
        {
            using ( var targetStream = await OpenWriteAsync() )
            {
                await sourceStream.CopyToAsync( targetStream );
            }
        }

        public async Task<byte[]> ReadToEndAsync()
        {
            using ( var fileStream = await OpenReadAsync() )
            {
                return await fileStream.ReadStreamToEndResetAsync();
            }
        }

        public async Task<Stream> OpenWriteAsync( FileMode mode = FileMode.Overwrite )
        {
            switch ( mode )
            {
                case FileMode.Overwrite:
                case FileMode.Truncate:
                    await DeleteAsync();
                    return await BlobClient.OpenWriteAsync( overwrite: true );
                case FileMode.Append:
                    var appendClient = BlobContainer.GetAppendBlobClient( FilePath );
                    await appendClient.CreateIfNotExistsAsync();
                    return await appendClient.OpenWriteAsync( overwrite: false );
                default:
                    throw new NotImplementedException();
            }
        }

        public async Task DeleteAsync()
        {
            await BlobClient.DeleteIfExistsAsync();
        }

        public void Dispose() {}
    }
}