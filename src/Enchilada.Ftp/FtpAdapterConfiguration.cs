namespace Enchilada.Ftp
{
    using System;
    using Configuration;
    using Infrastructure.Attributes;

    [ FilesystemAdapterMoniker( "ftp" ) ]
    public class FtpAdapterConfiguration : IEnchiladaAdapterConfiguration
    {
        public string AdapterName { get; set; }
        public Type FileProvider => typeof( FtpFileProvider );
        public string Host { get; set; }
        public int Port { get; set; } = 21;
        public string Directory { get; set; } = "/";
        public string Username { get; set; } = "anonymous";
        public string Password { get; set; }
    }
}