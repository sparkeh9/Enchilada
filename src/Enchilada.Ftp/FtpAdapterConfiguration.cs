namespace Enchilada.Ftp
{
    using System;
    using System.Security.Authentication;
    using Configuration;
    using CoreFtp.Enum;
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
        public IpVersion IpVersion { get; set; } = IpVersion.IpV4;
        public FtpEncryption EncryptionType { get; set; } = FtpEncryption.None;
        public bool IgnoreCertificateErrors { get; set; } = false;
        public SslProtocols SslProtocols { get; set; } = SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
    }
}