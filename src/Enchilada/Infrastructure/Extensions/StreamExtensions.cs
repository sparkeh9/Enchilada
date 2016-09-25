namespace Enchilada.Infrastructure.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Threading.Tasks;

    public static class StreamExtensions
    {
        public static Stream Reset( this Stream stream )
        {
            stream.Seek( 0, SeekOrigin.Begin );
            return stream;
        }

        public static async Task<byte[]> ReadStreamToEndAsync( this Stream stream )
        {
            var bytes = new List<byte>();
            var buffer = new byte[4096]; // 4kb buffer size
            int readCount = 0;
            while ( ( readCount = await stream.ReadAsync( buffer, 0, buffer.Length ) ) != 0 )
            {
                bytes.AddRange( buffer.Take( readCount ) );
            }
            return bytes.ToArray();
        }

        public static async Task<byte[]> ReadStreamToEndResetAsync( this Stream stream )
        {
            stream.Reset();
            var bytes = new List<byte>();
            var buffer = new byte[4096]; // 4kb buffer size
            int readCount = 0;
            while ( ( readCount = await stream.ReadAsync( buffer, 0, buffer.Length ) ) != 0 )
            {
                bytes.AddRange( buffer.Take( readCount ) );
            }
            return bytes.ToArray();
        }

        public static async Task<string> ToMd5HashAsync( this Stream stream )
        {
            var task = new Task<string>( stream.ToMd5Hash );
            task.Start();
            return await task;
        }

        public static string ToMd5Hash( this Stream stream )
        {
            using ( var md5 = MD5.Create() )
            {
                var hashBytes = md5.ComputeHash( stream );

                return BitConverter.ToString( hashBytes )
                                   .Replace( "-", "" )
                                   .ToLower();
            }
        }
    }
}