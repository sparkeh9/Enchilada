namespace Enchilada.S3.S3
{
    using System.IO;
    using Amazon.S3;
    using Amazon.S3.Model;
    using Infrastructure.Extensions;
    using Infrastructure.Interface;

    public class S3FileProvider : IFileProvider
    {
        protected DirectoryInfo BackingRootDirectory;

        public IDirectory? RootDirectory { get; protected set; }
        public IFile? File { get; protected set; }

        public bool IsDirectory => !IsFile;
        public bool IsFile => File != null;

        public S3FileProvider( S3AdapterConfiguration configuration, string filePath )
        {
            bool isDirectory = filePath.EndsWith( "/" );
            string s3Path = filePath.StripLeadingSlash();
            bool isRootBucket = s3Path.IsNullOrWhiteSpace();

            var s3ClientConfig = new AmazonS3Config
            {
                ServiceURL = configuration.ServiceUrl,
                ForcePathStyle = true // Required for MinIO
            };

            var s3Client = new AmazonS3Client( configuration.AccessKey, configuration.SecretKey, s3ClientConfig );

            if ( configuration.CreateBucket )
            {
                try
                {
                    s3Client.PutBucketAsync( new PutBucketRequest { BucketName = configuration.BucketName } ).GetAwaiter().GetResult();
                }
                catch ( AmazonS3Exception e )
                {
                    if ( e.ErrorCode != "BucketAlreadyOwnedByYou" )
                    {
                        throw;
                    }
                }
            }

            if ( isRootBucket )
            {
                RootDirectory = new S3Directory( s3Client, configuration.BucketName, "" );
                return;
            }

            if ( isDirectory )
            {
                RootDirectory = new S3Directory( s3Client, configuration.BucketName, s3Path );
                return;
            }

            RootDirectory = new S3Directory( s3Client, configuration.BucketName, s3Path.RemoveFilename() );
            File = RootDirectory.GetFile( s3Path );
        }

        public void Dispose() { }
    }
}