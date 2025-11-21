using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using Testcontainers.Azurite;

namespace Enchilada.Azure.Tests.Integration
{
    /// <summary>
    /// Spins up an Azurite (Azure Storage emulator) Docker container for the duration of the test run.
    /// The container is started lazily on first access.
    /// </summary>
    internal static class AzuriteTestcontainer
    {
        private static readonly Lazy<Task<AzuriteContainer>> containerBuilder = new( StartContainerAsync );

        private static async Task<AzuriteContainer> StartContainerAsync()
        {
            // Mirror the MinIO/FTP pattern: explicitly wait for the blob service port to be reachable
            // before considering the container ready for tests.
            var container = new AzuriteBuilder()
                           .WithImage( "mcr.microsoft.com/azure-storage/azurite:latest" )
                           .WithWaitStrategy( Wait.ForUnixContainer().UntilPortIsAvailable( 10000 ) )
                           .Build();

            await container.StartAsync();

            return container;
        }

        private static async Task<AzuriteContainer> GetContainerAsync()
        {
            return await containerBuilder.Value.ConfigureAwait( false );
        }

        /// <summary>
        /// Ensures the Azurite container is running and returns the connection string that targets the correct mapped host port.
        /// </summary>
        public static string GetConnectionString()
        {
            var container = GetContainerAsync().GetAwaiter().GetResult();

            return container.GetConnectionString();
        }
    }
}