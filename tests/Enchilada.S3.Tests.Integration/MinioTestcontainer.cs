namespace Enchilada.S3.Tests.Integration
{
    using DotNet.Testcontainers.Builders;
    using Testcontainers.Minio;

    namespace Enchilada.S3.Tests.Integration
    {
        /// <summary>
        /// Spins up a MinIO (S3-compatible storage) Docker container for the duration of the test run.
        /// The container is started lazily on first access and terminated when the process exits.
        /// </summary>
        internal static class MinioTestcontainer
        {
            private const string MinioAccessKey = "minioadmin";
            private const string MinioSecretKey = "minioadmin";

            private static readonly Lazy<Task<MinioContainer>> containerBuilder = new( StartContainerAsync );


            private static async Task<MinioContainer> StartContainerAsync()
            {
                var container = new MinioBuilder()
                               .WithImage( "minio/minio:latest" )
                               // .WithCommand( "/data" )
                               .WithEnvironment( "MINIO_ROOT_USER", MinioAccessKey )
                               .WithEnvironment( "MINIO_ROOT_PASSWORD", MinioSecretKey )
                               .WithWaitStrategy( Wait.ForUnixContainer().UntilPortIsAvailable( 9000 ) )
                               .Build();

                await container.StartAsync();

                AppDomain.CurrentDomain.ProcessExit += async ( _, _ ) => await container.DisposeAsync();

                return container;
            }

            private static async Task<MinioContainer> GetContainerAsync()
            {
                return await containerBuilder.Value.ConfigureAwait( false );
            }

            /// <summary>
            /// Ensures the MinIO container is running and returns the connection string that targets the correct mapped host port.
            /// </summary>
            public static string GetServiceUrl()
            {
                var container = GetContainerAsync().GetAwaiter().GetResult();

                return $"http://{container.Hostname}:{container.GetMappedPublicPort( 9000 )}";
            }

            public static string GetAccessKey()
            {
                return MinioAccessKey;
            }

            public static string GetSecretKey()
            {
                return MinioSecretKey;
            }
        }
    }
}