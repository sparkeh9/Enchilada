namespace Enchilada.Filesystem
{
    using System;
    using Configuration;
    using Infrastructure.Attributes;

    [ FilesystemAdapterMoniker( "local" ) ]
    public class FilesystemAdapterConfiguration : IEnchiladaAdapterConfiguration
    {
        public string AdapterName { get; set; }
        public Type FileProvider => typeof( FilesystemFileProvider );
        public string Directory { get; set; }
    }
}