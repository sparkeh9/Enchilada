namespace Enchilada.Azure.BlobStorage
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Infrastructure.Extensions;
    using Infrastructure.Interface;
    using Microsoft.WindowsAzure.Storage.Blob;
    using FileMode = Infrastructure.FileMode;

    public class BlobStorageFile : IFile
    {
        private readonly CloudBlobContainer BlobContainer;
        private readonly string FilePath;
        private CloudBlockBlob BlockBlobReference => BlobContainer.GetBlockBlobReference( FilePath );

        public string Name => Path.GetFileName( FilePath );

        public string Extension => Path.GetExtension( FilePath );
        public string RealPath => BlockBlobReference.Uri.ToString();

        public long Size => BlockBlobReference.Properties.Length;
        public DateTime? LastModified => BlockBlobReference.Properties.LastModified?.LocalDateTime;
        public bool IsDirectory => false;
        public bool Exists => BlockBlobReference.ExistsAsync().Result;

        public BlobStorageFile( CloudBlobContainer blobContainer, string filePath )
        {
            BlobContainer = blobContainer;
            FilePath = filePath;
        }

        public async Task<Stream> OpenReadAsync()
        {
            var blob = BlobContainer.GetBlockBlobReference( FilePath );

            var stream = new MemoryStream();

            await blob.DownloadToStreamAsync( stream );
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
            switch ( mode )
            {
                case FileMode.Overwrite:
                case FileMode.Truncate:
                    await DeleteAsync();
                    return await BlobContainer.GetBlockBlobReference( FilePath )
                                              .OpenWriteAsync();
                case FileMode.Append:
                    return await BlobContainer.GetAppendBlobReference( FilePath )
                                              .OpenWriteAsync( true );
                default:
                    throw new NotImplementedException();
            }
        }

        public async Task DeleteAsync()
        {
            var blob = BlobContainer.GetBlockBlobReference( FilePath );
            await blob.DeleteIfExistsAsync();
        }

        public void Dispose() {}
    }
}