namespace Enchilada.Azure.Tests.Integration.Helpers
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Azure.BlobStorage;
    using Integration;
    using global::Azure.Storage.Blobs;

    public static class ResourceHelpers
    {
        private static string NormaliseConnectionString( string connectionString )
        {
            if ( connectionString.Trim().StartsWith( "UseDevelopmentStorage=true", StringComparison.OrdinalIgnoreCase ) )
            {
                // Ensure the Azurite container is running and retrieve the dynamic connection string.
                return AzuriteTestcontainer.GetConnectionString();
            }

            return connectionString;
        }

        public static BlobContainerClient GetContainer( string connectionString, string containerName )
        {
            var normalized = NormaliseConnectionString( connectionString );
            var serviceClient = new BlobServiceClient( normalized );
            return serviceClient.GetBlobContainerClient( containerName );
        }

        public static BlobContainerClient GetLocalDevelopmentContainer()
        {
            var container = GetContainer( AzuriteTestcontainer.GetConnectionString(), "enchilada-test" );
            container.CreateIfNotExists();
            // Ensure blobs can be downloaded without authentication inside tests.
            container.SetAccessPolicy( global::Azure.Storage.Blobs.Models.PublicAccessType.Blob );
            return container;
        }

        public static async Task<string> MakeHttpRequestAsync( this string url )
        {
            using (var httpClient = new HttpClient())
            {
                return await httpClient.GetStringAsync(url);
            }
        }

        public static async Task<BlobStorageFile> CreateFileWithContentAsync( BlobContainerClient container, string filename, string content )
        {
            var blobFile = new BlobStorageFile( container, filename );

            using ( var stream = await blobFile.OpenWriteAsync() )
            using ( var writer = new StreamWriter( stream ) )
            {
                writer.Write( content );
            }

            return blobFile;
        }

        public static DirectoryInfo GetResourceDirectoryInfo( string directory = "" )
        {
            return new DirectoryInfo( $"{AppContext.BaseDirectory}/Resources/{directory}" );
        }
    }
}