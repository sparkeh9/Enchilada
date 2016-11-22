namespace Enchilada.Ftp
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using CoreFtp;
    using CoreFtp.Infrastructure;
    using Infrastructure.Extensions;
    using Infrastructure.Interface;
    using FileMode = Infrastructure.FileMode;

    public class FtpFile : IFile
    {
        private readonly FtpClient ftpClient;
        private readonly string path;
        private readonly string fileName;

        public string Name => fileName;

        public string Extension => System.IO.Path.GetExtension( fileName );
        public string Path => $"/{path}".StripDoubleSlash();
        public string RealPath => $"/{path}/{fileName}".StripDoubleSlash();

        public bool IsDirectory => false;

        public long Size { get; protected set; }

        public DateTime? LastModified { get; protected set; }

        private bool? exists;

        public bool Exists
        {
            get
            {
                if ( exists.HasValue )
                    return exists.Value;

                exists = Size > 0;

                return exists.Value;
            }
        }

        public FtpFile( FtpClient ftpClient, string fileName, string path = null )
        {
            this.ftpClient = ftpClient;
            this.path = path;
            this.fileName = fileName;


            Task.WaitAll( Task.Run( async () =>
            {
                await EnsureConnectedAsync();
                await GetFileMetadataAsync();
            } ) );
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
            await EnsureConnectedAsync();
            await ftpClient.ChangeWorkingDirectoryAsync( Path );
            var stream = await ftpClient.OpenFileReadStreamAsync( fileName );
            return stream;
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
            await CopyFromAsync( await sourceFile.OpenReadAsync() );
        }

        public async Task CopyFromAsync( Stream sourceStream )
        {
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

        public async Task DeleteAsync()
        {
            await EnsureConnectedAsync();
            await ftpClient.ChangeWorkingDirectoryAsync( Path );

            try
            {
                await ftpClient.DeleteFileAsync( fileName );
                exists = false;
            }
            catch ( FtpException ) {}
        }

        public void Dispose()
        {
            ftpClient.Dispose();
        }
    }
}