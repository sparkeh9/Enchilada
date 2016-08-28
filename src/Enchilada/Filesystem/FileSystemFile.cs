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
        public DateTime LastModified => BackingFile.LastWriteTime;
        public bool IsDirectory => false;
        public bool Exists => BackingFile.Exists;

        public FilesystemFile( FileInfo fileInfo )
        {
            BackingFile = fileInfo;
        }

        public Stream OpenRead()
        {
            return BackingFile.OpenRead();
        }

        public async Task<string> GetHashAsync()
        {
            using ( var stream = OpenRead() )
            {
                return await stream.ToMd5HashAsync();
            }
        }

        public async Task<byte[]> ReadToEndAsync()
        {
            using ( var fileStream = OpenRead() )
            {
                return await fileStream.ReadStreamToEndAsync();
            }
        }

        public Stream OpenWrite( FileMode mode = FileMode.Overwrite )
        {
            Directory.CreateDirectory( Path.GetDirectoryName( RealPath ) );

            switch ( mode )
            {
                case FileMode.Overwrite:
                    return BackingFile.OpenWrite();
                case FileMode.Append:
                    return BackingFile.Open( System.IO.FileMode.Append, FileAccess.Write );
                case FileMode.Truncate:
                    return BackingFile.Open( System.IO.FileMode.Truncate, FileAccess.Write );
                default:
                    throw new NotImplementedException();
            }
        }

        public void Delete( bool recursive = true )
        {
            BackingFile.Delete();
        }
    }
}