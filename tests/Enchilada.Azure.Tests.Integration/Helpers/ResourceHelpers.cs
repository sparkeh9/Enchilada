namespace Enchilada.Azure.Tests.Integration.Helpers
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Azure.BlobStorage;
    using Azure.Storage.Blobs;

    public static class ResourceHelpers
    {
        private static string NormalizeConnectionString( string connectionString )
        {
            if ( connectionString.Trim().StartsWith( "UseDevelopmentStorage=true", StringComparison.OrdinalIgnoreCase ) )
            {
                return "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFePZ7T6W+JdYemMFETyjZMF+hxFIs=;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;";
            }

            return connectionString;
        }

        public static BlobContainerClient GetContainer( string connectionString, string containerName )
        {
            var normalized = NormalizeConnectionString( connectionString );
            var serviceClient = new BlobServiceClient( normalized );
            return serviceClient.GetBlobContainerClient( containerName );
        }

        public static BlobContainerClient GetLocalDevelopmentContainer()
        {
            var container = GetContainer( "UseDevelopmentStorage=true;", "enchilada-test" );
            container.CreateIfNotExists();
            return container;
        }

        public static async Task<string> MakeHttpRequestAsync( this string url )
        {
            var webRequest = WebRequest.Create( url );
            using ( var stream = ( await webRequest.GetResponseAsync() ).GetResponseStream() )
            {
                var reader = new StreamReader( stream, Encoding.UTF8 );
                return reader.ReadToEnd();
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