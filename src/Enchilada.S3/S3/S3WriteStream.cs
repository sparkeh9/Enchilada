namespace Enchilada.S3
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Amazon.S3;
    using Amazon.S3.Model;

    public class S3WriteStream : Stream
    {
        private readonly IAmazonS3 s3Client;
        private readonly string bucketName;
        private readonly string key;
        private readonly MemoryStream memoryStream;
        private readonly Func<Task>? onCompleted;
        private bool uploaded;

        public S3WriteStream( IAmazonS3 s3Client, string bucketName, string key, Func<Task>? onCompleted = null )
        {
            this.s3Client = s3Client;
            this.bucketName = bucketName;
            this.key = key;
            memoryStream = new MemoryStream();
            this.onCompleted = onCompleted;
        }

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => memoryStream.Length;

        public override long Position
        {
            get => memoryStream.Position;
            set => memoryStream.Position = value;
        }

        public override void Flush()
        {
            memoryStream.Flush();
        }

        public override int Read( byte[] buffer, int offset, int count )
        {
            throw new NotSupportedException();
        }

        public override long Seek( long offset, SeekOrigin origin )
        {
            throw new NotSupportedException();
        }

        public override void SetLength( long value )
        {
            memoryStream.SetLength( value );
        }

        public override void Write( byte[] buffer, int offset, int count )
        {
            memoryStream.Write( buffer, offset, count );
        }

        public override async Task WriteAsync( byte[] buffer, int offset, int count, CancellationToken cancellationToken )
        {
            await memoryStream.WriteAsync( buffer, offset, count, cancellationToken );
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( !uploaded )
                {
                    memoryStream.Position = 0;
                    var request = new PutObjectRequest
                    {
                        BucketName = bucketName,
                        Key = key,
                        InputStream = memoryStream
                    };
                    s3Client.PutObjectAsync( request ).GetAwaiter().GetResult();
                    if ( onCompleted is not null )
                    {
                        onCompleted().GetAwaiter().GetResult();
                    }
                    uploaded = true;
                }
                memoryStream.Dispose();
            }
            base.Dispose( disposing );
        }
    }
}