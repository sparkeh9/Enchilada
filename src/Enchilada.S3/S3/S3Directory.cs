namespace Enchilada.S3
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Amazon.S3;
    using Amazon.S3.Model;
    using Infrastructure.Extensions;
    using Infrastructure.Interface;

    public class S3Directory : IDirectory
    {
        private readonly IAmazonS3 s3Client;
        private readonly string bucketName;
        private readonly string prefix;

        public string Name => RealPath.GetDirectoryNameFromPath();
        public string RealPath { get; }
        public IDirectory? Parent { get; }
        public bool IsDirectory => true;

        public bool Exists
        {
            get
            {
                try
                {
                    var listObjectsV2Request = new ListObjectsV2Request
                    {
                        BucketName = bucketName,
                        Prefix = prefix,
                        MaxKeys = 1
                    };
                    var response = s3Client.ListObjectsV2Async( listObjectsV2Request ).GetAwaiter().GetResult();
                    if ( response == null )
                    {
                        return false;
                    }

                    return ( response.S3Objects?.Any() ?? false ) || ( response.CommonPrefixes?.Any() ?? false );
                }
                catch ( AmazonS3Exception e )
                {
                    if ( e.ErrorCode == "NoSuchBucket" )
                    {
                        return false;
                    }

                    return false; // Return false for other S3 exceptions indicating non-existence
                }
                catch ( Exception )
                {
                    return false; // Catch all other exceptions and return false
                }
            }
        }

        public DateTime? LastModified { get; private set; }

        public S3Directory( IAmazonS3 s3Client, string bucketName, string prefix )
        {
            this.s3Client = s3Client;
            this.bucketName = bucketName;
            this.prefix = string.IsNullOrEmpty( prefix ) ? string.Empty : prefix.EnsureTrailingSlash();
            RealPath = this.prefix;
            LastModified = DateTime.UtcNow; // Placeholder for now, will update if needed
        }

        public async Task<IReadOnlyCollection<INode>> GetAllNodesAsync()
        {
            var listObjectsRequest = new ListObjectsV2Request
            {
                BucketName = bucketName,
                Prefix = prefix,
                Delimiter = "/"
            };

            var response = await s3Client.ListObjectsV2Async( listObjectsRequest );

            var directories = response.CommonPrefixes?.Select( p => new S3Directory( s3Client, bucketName, p ) )?.ToList() ?? new List<S3Directory>();

            // Also treat any zero-byte objects that end with '/' as directory placeholders (created via CreateDirectoryAsync)
            var placeholderDirs = response.S3Objects?
                                        .Where( o => o.Key != prefix && o.Key.EndsWith( "/" ) )
                                        .Select( o => new S3Directory( s3Client, bucketName, o.Key ) )
                                        .ToList() ?? new List<S3Directory>();

            directories.AddRange( placeholderDirs );

            var files = response.S3Objects?
                            .Where( o => o.Key != prefix && !o.Key.EndsWith( "/" ) )
                            .Select( o => new S3File( s3Client, bucketName, o.Key ) )
                            .ToList() ?? new List<S3File>();

            return directories.Cast<INode>().Concat( files ).ToList();
        }

        public async Task<IReadOnlyCollection<IDirectory>> GetDirectoriesAsync()
        {
            var listObjectsRequest = new ListObjectsV2Request
            {
                BucketName = bucketName,
                Prefix = prefix,
                Delimiter = "/"
            };

            var response = await s3Client.ListObjectsV2Async( listObjectsRequest );

            var directories = response.CommonPrefixes?.Select( p => new S3Directory( s3Client, bucketName, p ) ).ToList() ?? new List<S3Directory>();

            // include placeholder directory objects
            if ( response.S3Objects is not null )
            {
                directories.AddRange( response.S3Objects.Where( o => o.Key != prefix && o.Key.EndsWith( "/" ) )
                                                       .Select( o => new S3Directory( s3Client, bucketName, o.Key ) ) );
            }

            return directories;
        }

        public async Task<IReadOnlyCollection<IFile>> GetFilesAsync()
        {
            var listObjectsRequest = new ListObjectsV2Request
            {
                BucketName = bucketName,
                Prefix = prefix,
                Delimiter = "/"
            };

            var response = await s3Client.ListObjectsV2Async( listObjectsRequest );

            var s3Objects = response.S3Objects ?? new List<S3Object>();

            return s3Objects.Where( o => o.Key != prefix && !o.Key.EndsWith( "/" ) )
                           .Select( o => new S3File( s3Client, bucketName, o.Key ) ).ToList();
        }

        public IFile GetFile( string fileName )
        {
            return new S3File( s3Client, bucketName, fileName );
        }

        public IDirectory GetDirectory( string directoryName )
        {
            return new S3Directory( s3Client, bucketName, directoryName );
        }

        public async Task CreateDirectoryAsync()
        {
            // S3 does not have explicit directories. Creating a 0-byte object with a trailing slash simulates a directory.
            var putObjectRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = prefix,
                ContentBody = ""
            };
            await s3Client.PutObjectAsync( putObjectRequest );
        }

        public async Task DeleteAsync()
        {
            var listObjectsRequest = new ListObjectsV2Request
            {
                BucketName = bucketName,
                Prefix = prefix
            };

            ListObjectsV2Response response;
            do
            {
                response = await s3Client.ListObjectsV2Async( listObjectsRequest );
                var deleteObjectsRequest = new DeleteObjectsRequest
                {
                    BucketName = bucketName,
                    Objects = response.S3Objects.Select( o => new KeyVersion { Key = o.Key } ).ToList()
                };

                if ( deleteObjectsRequest.Objects.Any() )
                {
                    await s3Client.DeleteObjectsAsync( deleteObjectsRequest );
                }

                listObjectsRequest.ContinuationToken = response.NextContinuationToken;
            } while ( response.IsTruncated ?? false );
        }

        public IEnumerator<INode> GetEnumerator()
        {
            return GetAllNodesAsync().GetAwaiter().GetResult().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}