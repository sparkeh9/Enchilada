namespace Enchilada.Azure.BlobStorage
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Infrastructure.Extensions;
    using Infrastructure.Interface;
    using global::Azure.Storage.Blobs;
    using global::Azure.Storage.Blobs.Models;
    using FileMode = Infrastructure.FileMode;

    public class BlobStorageFile : IFile
    {
        private readonly BlobContainerClient BlobContainer;
        private readonly string FilePath;
        private BlobClient BlobReference => BlobContainer.GetBlobClient( FilePath );

        public string Name => Path.GetFileName( FilePath );

        public string Extension => Path.GetExtension( FilePath );
        public string RealPath => BlobReference.Uri.ToString();

        public long Size => GetBlobProperties()?.ContentLength ?? 0;
        public DateTime? LastModified => GetBlobProperties()?.LastModified.LocalDateTime;
        public bool IsDirectory => false;
        public bool Exists => BlobReference.ExistsAsync().Result.Value;

        public BlobStorageFile( BlobContainerClient blobContainer, string filePath )
        {
            BlobContainer = blobContainer;
            FilePath = filePath;
        }

        private BlobProperties GetBlobProperties()
        {
            try
            {
                return BlobReference.GetPropertiesAsync().Result.Value;
            }
            catch
            {
                return null;
            }
        }

        public async Task<Stream> OpenReadAsync()
        {
            var blob = BlobContainer.GetBlobClient( FilePath );

            var stream = new MemoryStream();

            await blob.DownloadToAsync( stream );
            stream.Seek( 0, SeekOrigin.Begin );
            return stream;
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
            var blob = BlobContainer.GetBlobClient( FilePath );
            
            switch ( mode )
            {
                case FileMode.Overwrite:
                case FileMode.Truncate:
                    await DeleteAsync();
                    return await blob.OpenWriteAsync( overwrite: true );
                case FileMode.Append:
                    // For append mode, we need to use a different approach
                    // The new SDK doesn't support append blobs in the same way
                    // We'll simulate append by reading existing content and appending to it
                    throw new NotSupportedException("Append mode is not supported with the new Azure SDK. Use Overwrite mode instead.");
                default:
                    throw new NotImplementedException();
            }
        }

        public async Task DeleteAsync()
        {
            var blob = BlobContainer.GetBlobClient( FilePath );
            await blob.DeleteIfExistsAsync();
        }

        public void Dispose() {}
    }
}