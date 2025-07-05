namespace Enchilada.Ftp.Tests.Integration.Ftp
{
    using System;
    using Microsoft.Extensions.Logging;
    using Xunit.Abstractions;
    using Enchilada.Ftp.Tests.Integration.Logger;

    /// <summary>
    /// Base class for FTP integration tests that ensures the Docker-ised FTP server
    /// (see <see cref="FtpTestcontainer"/>) is running and provides easy access to
    /// its connection details.
    /// </summary>
    public abstract class FtpTestBase : IDisposable
    {
        protected ILogger Logger { get; set; }

        protected static readonly FtpConfiguration FtpConfig = new()
        {
            Host = FtpTestcontainer.GetHost(),
            Port = FtpTestcontainer.GetPort(),
            Username = FtpTestcontainer.GetUsername(),
            Password = FtpTestcontainer.GetPassword()
        };

        protected FtpTestBase( ITestOutputHelper outputHelper = null )
        {
            Logger = new XUnitConsoleLogger(GetType().FullName, (category, level) => true, outputHelper);
            // Ensure the container is started. Accessing any property triggers the lazy initialisation.
            _ = FtpConfig.Host;
        }

        protected virtual void Dispose( bool disposing )
        {
            if ( disposing )
            {
                Logger = null;
            }
        }

        public void Dispose()
        {
            Dispose( true );
        }
    }
}