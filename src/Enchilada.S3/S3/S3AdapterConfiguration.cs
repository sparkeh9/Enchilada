namespace Enchilada.S3.S3
{
    using System;
    using Configuration;
    using Infrastructure.Attributes;

    [ FilesystemAdapterMoniker( "s3" ) ]
    public class S3AdapterConfiguration : IEnchiladaAdapterConfiguration
    {
        public string AdapterName { get; set; }
        public Type FileProvider => typeof( S3FileProvider );
        public string ServiceUrl { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public required string BucketName { get; set; }
        public bool CreateBucket { get; set; }
    }
}