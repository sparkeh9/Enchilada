namespace Enchilada.Azure.Tests.Integration.Helpers
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Azure.BlobStorage;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    public static class ResourceHelpers
    {
        public static CloudBlobContainer GetContainer( string connectionString, string containerName )
        {
            var account = CloudStorageAccount.Parse( connectionString );
            var blobClient = account.CreateCloudBlobClient();
            return blobClient.GetContainerReference( containerName );
        }

        public static CloudBlobContainer GetLocalDevelopmentContainer()
        {
            return GetContainer( "UseDevelopmentStorage=true;", "test" );
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

        public static async Task CreateFileWithContentAsync( CloudBlobContainer container, string filename, string content )
        {
            var blobFile = new BlobStorageFile( container, filename );

            using ( var stream = await blobFile.OpenWriteAsync() )
            using ( var writer = new StreamWriter( stream ) )
            {
                writer.Write( content );
            }
        }

    }
}