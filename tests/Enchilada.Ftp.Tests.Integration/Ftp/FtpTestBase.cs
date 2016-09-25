namespace Enchilada.Ftp.Tests.Integration.Ftp
{
    using Microsoft.Extensions.Logging;

    public abstract class FtpTestBase
    {
        protected ILogger Logger { get; set; }

        protected FtpTestBase()
        {
            Program.Initialise();
            Logger = Program.LoggerFactory.CreateLogger( GetType().Name );
        }
    }
}