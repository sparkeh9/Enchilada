namespace Enchilada.Filesystem
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Infrastructure;
    using Infrastructure.Extensions;
    using Infrastructure.Interface;
    using FileMode = Infrastructure.FileMode;

    public class FilesystemFile : IFile
    {
        protected FileInfo BackingFile;

        public string Name => BackingFile.Name;
        public string Extension => BackingFile.Extension;
        public string RealPath => BackingFile.FullName;
        public long Size => BackingFile.Length;
        public DateTime? LastModified => BackingFile.LastWriteTime;
        public bool IsDirectory => false;
        public bool Exists => BackingFile.Exists;

        public FilesystemFile( FileInfo fileInfo )
        {
            BackingFile = fileInfo;
        }

        public async Task<Stream> OpenReadAsync()
        {
            var task = new Task<Stream>( BackingFile.OpenRead );
            task.Start();
            return await task;
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
                return await fileStream.ReadStreamToEndResetAsync();
            }
        }

        public async Task<Stream> OpenWriteAsync( FileMode mode = FileMode.Overwrite )
        {
            Directory.CreateDirectory( Path.GetDirectoryName( RealPath ) );

            Task<Stream> task;
            switch ( mode )
            {
                case FileMode.Overwrite:
                    task = new Task<Stream>( BackingFile.OpenWrite );
                    break;
                case FileMode.Append:
                    task = new Task<Stream>( () => BackingFile.Open( System.IO.FileMode.Append, FileAccess.Write ) );
                    break;
                case FileMode.Truncate:
                    task = new Task<Stream>( () => BackingFile.Open( System.IO.FileMode.Truncate, FileAccess.Write ) );
                    break;
                default:
                    throw new NotImplementedException();
            }
            task.Start();
            return await task;
        }

        public async Task DeleteAsync( bool recursive = true )
        {
            var task = new Task( BackingFile.Delete );
            task.Start();
            await task;
        }

        public void Dispose() {}
    }
}