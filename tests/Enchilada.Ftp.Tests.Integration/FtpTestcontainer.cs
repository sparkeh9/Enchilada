using System;
using System.Threading.Tasks;

namespace Enchilada.Ftp.Tests.Integration
{
    using DotNet.Testcontainers.Builders;
    using DotNet.Testcontainers.Containers;

    /// <summary>
    ///  Lazily starts an instance of the garethflowers/ftp-server Docker image and exposes
    ///  the connection details for integration tests. The container is torn down when the
    ///  current process exits.
    /// </summary>
    internal static class FtpTestcontainer
    {
        private const string Username = "user";
        private const string Password = "123";

        // We keep the container instance in a Lazy<Task<ITestcontainersContainer>> so that it starts only once,
        // even if many tests attempt to access it concurrently.
        private static readonly Lazy<Task<IContainer>> ContainerBuilder = new(StartContainerAsync);

        private static async Task<IContainer> StartContainerAsync()
        {
            var builder = new ContainerBuilder()
                .WithName("enchilada_ftp_test")
                .WithReuse(true)
                .WithImage("garethflowers/ftp-server:latest")
                .WithEnvironment("FTP_USER", Username)
                .WithEnvironment("FTP_PASS", Password)
                // Required so that the PASV response contains the host IP we are actually connecting to.
                .WithEnvironment("PUBLIC_IP", "127.0.0.1")
                .WithExposedPort(21)
                .WithPortBinding(21, 21) // map control port directly
                // Passive ports: expose and map to random host ports as well.
                .WithExposedPort(20)
                .WithPortBinding(20, 20);

            // Expose and bind the passive port range 40000-40009.
            for ( ushort port = 40000; port <= 40009; port++ )
            {
                builder = builder.WithExposedPort(port).WithPortBinding(port, port);
            }

            builder = builder.WithWaitStrategy( Wait.ForUnixContainer().UntilPortIsAvailable(21) );

            var container = builder.Build();
            await container.StartAsync();

            return container;
        }

        private static async Task<IContainer> GetContainerAsync()
        {
            return await ContainerBuilder.Value.ConfigureAwait(false);
        }

        public static string GetHost()
        {
            var container = GetContainerAsync().GetAwaiter().GetResult();
            return container.Hostname;
        }

        public static int GetPort()
        {
            var container = GetContainerAsync().GetAwaiter().GetResult();
            return container.GetMappedPublicPort(21);
        }

        public static string GetUsername() => Username;
        public static string GetPassword() => Password;
    }
} 