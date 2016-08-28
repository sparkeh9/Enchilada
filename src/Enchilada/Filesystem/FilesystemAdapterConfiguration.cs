namespace Enchilada.Filesystem
{
    using System;
    using Configuration;

    public class FilesystemAdapterConfiguration : IEnchiladaAdapterConfiguration
    {
        public string AdapterName { get; set; }
        public Type FileProvider => typeof( FilesystemFileProvider );
        public string Directory { get; set; }
    }
}