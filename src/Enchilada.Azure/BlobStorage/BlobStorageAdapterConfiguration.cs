namespace Enchilada.Azure.BlobStorage
{
    using System;
    using Configuration;
    using Infrastructure.Attributes;

    [ FilesystemAdapterMoniker( "azure-blob" ) ]
    public class BlobStorageAdapterConfiguration : IEnchiladaAdapterConfiguration
    {
        public string AdapterName { get; set; }
        public Type FileProvider => typeof( BlobStorageFileProvider );
        public bool CreateContainer { get; set; }
        public string ConnectionString { get; set; }
        public string ContainerReference { get; set; }
        public bool IsPublicAccess { get; set; }
    }
}