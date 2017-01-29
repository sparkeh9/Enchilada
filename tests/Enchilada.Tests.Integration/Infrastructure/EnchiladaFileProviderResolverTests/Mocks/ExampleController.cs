namespace Enchilada.Tests.Integration.Infrastructure.EnchiladaFileProviderResolverTests.Mocks
{
   using System.IO;
   using System.Threading.Tasks;
   using Enchilada.Infrastructure.Interface;

   public class ExampleController
   {
      private readonly IEnchiladaFilesystemResolver enchilada;

      public ExampleController( IEnchiladaFilesystemResolver enchilada )
      {
         this.enchilada = enchilada;
      }

      public async Task<string> Index( string bucket, string imageName )
      {
         using( var downloadFileReference = enchilada.OpenFileReference( $"enchilada://{bucket}/{imageName}" ) )
         {
            using( var downloadFileStream = await downloadFileReference.OpenReadAsync() )
            {
               long size = downloadFileReference.Size;

               var reader = new StreamReader( downloadFileStream );
               return await reader.ReadToEndAsync();
            }
         }
      }
   }
}