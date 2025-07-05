namespace Enchilada.S3
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Infrastructure.Extensions;
    using Infrastructure.Interface;
    using Amazon.S3;
    using Amazon.S3.Model;
    using FileMode = Enchilada.Infrastructure.FileMode;

    public class S3File : IFile
    {
        private readonly IAmazonS3 s3Client;
        private readonly string bucketName;
        private bool exists;

        public string Name => RealPath.GetFilenameFromPath();
        public string RealPath { get; }
        public IDirectory? Parent { get; }
        public bool IsDirectory => false;
        public DateTime? LastModified { get; private set; }
        public long Size { get; private set; }
        public bool Exists {
            get {
                // refresh metadata on every check to reflect external changes (e.g. deletes)
                UpdateMetadata().GetAwaiter().GetResult();
                return exists;
            }
            private set { exists = value; }
        }

        public S3File( IAmazonS3 s3Client, string bucketName, string key )
        {
            this.s3Client = s3Client;
            this.bucketName = bucketName;
            RealPath = key;
            Parent = new S3Directory( this.s3Client, this.bucketName, RealPath.RemoveFilename() );
            UpdateMetadata().GetAwaiter().GetResult();
        }

        private async Task UpdateMetadata()
        {
            try
            {
                var request = new GetObjectMetadataRequest
                {
                    BucketName = bucketName,
                    Key = RealPath
                };
                var response = await s3Client.GetObjectMetadataAsync( request );
                Size = response.ContentLength;
                LastModified = response.LastModified;
                Exists = true;
            }
            catch ( AmazonS3Exception e )
            {
                if ( e.ErrorCode == "NoSuchKey" || e.ErrorCode == "NotFound" )
                {
                    Exists = false;
                    Size = 0;
                    LastModified = DateTime.MinValue;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<Stream> OpenReadAsync()
        {
            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = RealPath
                };
                var response = await s3Client.GetObjectAsync( request );
                return response.ResponseStream;
            }
            catch ( AmazonS3Exception e )
            {
                if ( e.ErrorCode == "NoSuchKey" || e.ErrorCode == "NotFound" )
                {
                    return null;
                }
                throw;
            }
        }

        public async Task<Stream> OpenWriteAsync( FileMode mode = FileMode.Overwrite )
        {
            var writeStream = new S3WriteStream( s3Client, bucketName, RealPath, async () => await UpdateMetadata() );

            switch ( mode )
            {
                case FileMode.Append:
                    if ( Exists )
                    {
                        await using var existing = await OpenReadAsync();
                        if ( existing != null )
                        {
                            await existing.CopyToAsync( writeStream );
                        }
                    }
                    break;
                case FileMode.Truncate:
                case FileMode.Overwrite:
                default:
                    // For overwrite or truncate we start with empty stream (default behaviour)
                    break;
            }

            return writeStream;
        }

        public async Task DeleteAsync()
        {
            var request = new DeleteObjectRequest
            {
                BucketName = bucketName,
                Key = RealPath
            };
            await s3Client.DeleteObjectAsync( request );
            Exists = false;
            Size = 0;
            LastModified = DateTime.MinValue;
        }

        public async Task<byte[]> ReadToEndAsync()
        {
            using ( var stream = await OpenReadAsync() )
            {
                if ( stream == null )
                {
                    return [];
                }
                using ( var memoryStream = new MemoryStream() )
                {
                    await stream.CopyToAsync( memoryStream );
                    return memoryStream.ToArray();
                }
            }
        }

        public async Task<string> GetHashAsync()
        {
            try
            {
                var request = new GetObjectMetadataRequest
                {
                    BucketName = bucketName,
                    Key = RealPath
                };
                var response = await s3Client.GetObjectMetadataAsync( request );
                return response.ETag.Replace( "\"", "" ); // ETag is usually the MD5 hash for non-multipart uploads
            }
            catch ( AmazonS3Exception e )
            {
                if ( e.ErrorCode == "NoSuchKey" || e.ErrorCode == "NotFound" )
                {
                    return null;
                }
                throw;
            }
        }

        public async Task CopyFromAsync( IFile sourceFile )
        {
            using ( var stream = await sourceFile.OpenReadAsync() )
            {
                await CopyFromAsync( stream );
            }
        }

        public async Task CopyFromAsync( Stream sourceStream )
        {
            using ( var writeStream = await OpenWriteAsync() )
            {
                await sourceStream.CopyToAsync( writeStream );
            }
        }

        public void Dispose()
        {
            // No unmanaged resources to dispose directly in this class
        }
    }
}