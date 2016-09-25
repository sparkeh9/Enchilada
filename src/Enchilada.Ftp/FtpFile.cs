namespace Enchilada.Ftp
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using CoreFtp;
    using CoreFtp.Infrastructure;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Infrastructure.Interface;
    using Microsoft.Extensions.Logging;
    using FileMode = Infrastructure.FileMode;

    public class FtpFile : IFile
    {
        private readonly FtpClient ftpClient;
        private readonly string path;
        private readonly string fileName;
        private long size;
        private DateTime? lastModified;

        private readonly AsyncLazy<bool> HasMetadata;

        public string Name => fileName;

        public string Extension => System.IO.Path.GetExtension( fileName );
        public string Path => $"/{path}".StripDoubleSlash();
        public string RealPath => $"/{path}/{fileName}".StripDoubleSlash();

        public bool IsDirectory => false;

        public long Size
        {
            get
            {
                HasMetadata.GetAwaiter().GetResult();
                return size;
            }
            private set { size = value; }
        }

        public DateTime? LastModified
        {
            get
            {
                HasMetadata.GetAwaiter().GetResult();
                return lastModified;
            }
            private set { lastModified = value; }
        }

        private bool? exists;

        public bool Exists
        {
            get
            {
                if ( exists.HasValue )
                    return exists.Value;
                HasMetadata.GetAwaiter().GetResult();
                exists = size > 0;

                return exists.Value;
            }
        }

        public FtpFile( FtpClient ftpClient, string fileName, string path = null )
        {
            this.ftpClient = ftpClient;
            this.path = path;
            this.fileName = fileName;

            EnsureConnectedAsync().Wait();

            HasMetadata = new AsyncLazy<bool>( async () =>
                                               {
                                                   await GetFileMetadataAsync();
                                                   return true;
                                               } );
        }

        private async Task EnsureConnectedAsync()
        {
            if ( ftpClient.IsConnected )
                return;

            await ftpClient.LoginAsync();
        }

        private async Task GetFileMetadataAsync()
        {
            await EnsureConnectedAsync();

            try
            {
                Size = await ftpClient.GetFileSizeAsync( fileName );

                var fileNode = ( await ftpClient.ListFilesAsync() ).FirstOrDefault( x => x.Name == fileName );
                LastModified = fileNode?.DateModified;
            }
            catch ( FtpException )
            {
                Size = 0;
                LastModified = default( DateTime? );
            }
        }

        public async Task<Stream> OpenReadAsync()
        {
            try
            {
                await EnsureConnectedAsync();
                await ftpClient.ChangeWorkingDirectoryAsync( Path );
                var stream = await ftpClient.OpenFileReadStreamAsync( fileName ).ConfigureAwait( false );
                return stream;
            }
            catch ( Exception e )
            {
                throw;
            }
        }

        public async Task<string> GetHashAsync()
        {
            using ( var stream = await OpenReadAsync() )
            {
                return await stream.ToMd5HashAsync();
            }
        }

        public async Task CopyFromAsync( IFile sourceFile )
        {
            using ( var sourceStream = await sourceFile.OpenReadAsync() )
            using ( var targetStream = await OpenWriteAsync() )
            {
                await sourceStream.CopyToAsync( targetStream );
            }
        }

        public async Task<byte[]> ReadToEndAsync()
        {
            using ( var fileStream = await OpenReadAsync() )
            {
                return await fileStream.ReadStreamToEndAsync();
            }
        }

        public async Task<Stream> OpenWriteAsync( FileMode mode = FileMode.Overwrite )
        {
            await EnsureConnectedAsync();
            exists = null;
            await ftpClient.CreateDirectoryAsync( Path );
            await ftpClient.ChangeWorkingDirectoryAsync( Path );
            return await ftpClient.OpenFileWriteStreamAsync( fileName );
        }

        public async Task DeleteAsync( bool recursive = true )
        {
            await EnsureConnectedAsync();
            await ftpClient.ChangeWorkingDirectoryAsync( Path );
            await Task.WhenAny( ftpClient.DeleteFileAsync( fileName ), Task.Delay( 100 ) );
            exists = false;
        }

        public void Dispose()
        {
            ftpClient.Dispose();
        }
    }
}