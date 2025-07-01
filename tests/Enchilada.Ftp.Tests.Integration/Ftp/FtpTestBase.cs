namespace Enchilada.Ftp.Tests.Integration.Ftp
{
    using System;
    using Microsoft.Extensions.Logging;
    using Xunit.Abstractions;
    using Enchilada.Ftp.Tests.Integration.Logger;

    public abstract class FtpTestBase : IDisposable
    {
        protected ILogger Logger { get; set; }

        protected FtpTestBase( ITestOutputHelper outputHelper = null )
        {
            Logger = new XUnitConsoleLogger(GetType().FullName, (category, level) => true, outputHelper);
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