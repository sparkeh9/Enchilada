using System;
using System.Threading.Tasks;
using Testcontainers.Azurite;

namespace Enchilada.Azure.Tests.Integration
{
    /// <summary>
    /// Spins up an Azurite (Azure Storage emulator) Docker container for the duration of the test run.
    /// The container is started lazily on first access and terminated when the process exits.
    /// </summary>
    internal static class AzuriteTestcontainer
    {
        private static readonly Lazy<Task<AzuriteContainer>> containerBuilder = new( StartContainerAsync );


        private static async Task<AzuriteContainer> StartContainerAsync()
        {
            var container = new AzuriteBuilder()
                           .WithImage( "mcr.microsoft.com/azure-storage/azurite:latest" )
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