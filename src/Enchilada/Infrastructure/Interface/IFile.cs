namespace Enchilada.Infrastructure.Interface
{
   using System;
   using System.IO;
   using System.Threading.Tasks;
   using FileMode = FileMode;

   public interface IFile: INode, IDisposable
   {
      /// <summary>
      /// Gets the file's size
      /// </summary>
      long Size { get; }

      /// <summary>
      /// Provides a Stream in read-only mode
      /// </summary>
      /// <returns>Stream</returns>
      Task<Stream> OpenReadAsync();

      /// <summary>
      /// Asynchronously reads the file, returning a byte array
      /// </summary>
      /// <returns></returns>
      Task<byte[]> ReadToEndAsync();

      /// <summary>
      /// Provides a stream in write mode.
      /// </summary>
      /// <param name="mode">The mode in which to amend the file</param>
      /// <returns>Stream</returns>
      Task<Stream> OpenWriteAsync( FileMode mode = FileMode.Overwrite );

      /// <summary>
      /// Provides a hash to check for file integrity
      /// </summary>
      /// <returns></returns>
      Task<string> GetHashAsync();

      /// <summary>
      /// Copies a file from source to target
      /// </summary>
      /// <param name="sourceFile"></param>
      /// <returns></returns>
      Task CopyFromAsync( IFile sourceFile );

      /// <summary>
      /// Copies data from a stream to the file
      /// </summary>
      /// <param name="sourceStream"></param>
      /// <returns></returns>
      Task CopyFromAsync( Stream sourceStream );
   }
}
