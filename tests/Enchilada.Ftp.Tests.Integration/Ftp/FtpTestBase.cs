namespace Enchilada.Ftp.Tests.Integration.Ftp
{
    using System;
    using Microsoft.Extensions.Logging;
    using Xunit.Abstractions;

    public abstract class FtpTestBase : IDisposable
    {
        protected ILogger Logger { get; set; }

        protected FtpTestBase( ITestOutputHelper outputHelper = null )
        {
            Program.Initialise( outputHelper );
            Logger = Program.LoggerFactory.CreateLogger( GetType().Name );
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