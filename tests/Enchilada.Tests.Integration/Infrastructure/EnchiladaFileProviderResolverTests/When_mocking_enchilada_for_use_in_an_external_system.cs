namespace Enchilada.Tests.Integration.Infrastructure.EnchiladaFileProviderResolverTests
{
   using System.IO;
   using System.Text;
   using System.Threading.Tasks;
   using Enchilada.Infrastructure.Interface;
   using Mocks;
   using NSubstitute;
   using Xunit;

   public class When_mocking_enchilada_for_use_in_an_external_system
   {
      [ Fact ]
      public async Task Should_provide_mockable_usage()
      {
         var file = Substitute.For<IFile>();
         file.Name.Returns( "test.jpg" );
         file.OpenReadAsync().Returns( new MemoryStream( Encoding.UTF8.GetBytes( "whatever" ) ) );

         var enchilada = Substitute.For<IEnchiladaFilesystemResolver>();
         enchilada.OpenFileReference( Arg.Any<string>() )
                  .Returns( file );

         var controllerForTest = new ExampleController( enchilada );
         await controllerForTest.Index( "a-bucket", "jibble.jpg" );

         enchilada.Received( 1 ).OpenFileReference( "enchilada://a-bucket/jibble.jpg" );
      }
   }
}
