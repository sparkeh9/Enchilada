namespace Enchilada.Ftp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CoreFtp;
    using Infrastructure.Interface;
    using Infrastructure.Extensions;

    public class FtpDirectory : IDirectory
    {
        private readonly FtpClient ftpClient;
        private readonly string path;
        private readonly string endOfPath;

        public string Name => string.Empty;

        public DateTime? LastModified => GetAllNodesAsync().GetAwaiter()
                                                           .GetResult()
                                                           .OrderBy( x => x.LastModified )
                                                           .FirstOrDefault()?.LastModified;

        public bool IsDirectory => true;

        public string RealPath => string.Empty;

        public bool Exists => true;

        public FtpDirectory( FtpClient ftpClient, string path )
        {
            endOfPath = path.Split( '/' ).LastOrDefault();

            this.ftpClient = ftpClient;
            this.path = path;
            EnsureConnectedAsync().Wait();
        }

        private async Task EnsureConnectedAsync()
        {
            if ( ftpClient.IsConnected )
                return;

            await ftpClient.LoginAsync();

            if ( !path.IsNullOrWhiteSpace() )
                await ftpClient.ChangeWorkingDirectoryAsync( path );
        }

        public async Task DeleteAsync()
        {
            await EnsureConnectedAsync();

            if ( endOfPath == null )
                return;

            if ( ftpClient.WorkingDirectory.EndsWith( endOfPath ) )
            {
                await ftpClient.ChangeWorkingDirectoryAsync( ".." );
            }

            await ftpClient.DeleteDirectoryAsync( endOfPath );
        }

        public IEnumerator<INode> GetEnumerator()
        {
            return GetAllNodesAsync().GetAwaiter()
                                     .GetResult()
                                     .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public async Task<IReadOnlyCollection<IDirectory>> GetDirectoriesAsync()
        {
            var ftpNodes = await ftpClient.ListDirectoriesAsync();

            return ftpNodes.Select( ftpNode => new FtpDirectory( ftpClient, $"{path}/{ftpNode.Name}" ) )
                           .ToList()
                           .AsReadOnly();
        }

        public async Task<IReadOnlyCollection<IFile>> GetFilesAsync()
        {
            var ftpNodes = await ftpClient.ListFilesAsync();

            return ftpNodes.Select( ftpNode => new FtpFile( ftpClient, ftpNode.Name, path ) )
                           .ToList()
                           .AsReadOnly();
        }

        public IDirectory GetDirectory( string path )
        {
            return new FtpDirectory( ftpClient, $"{this.path}/{path}" );
        }

        public IFile GetFile( string fileName )
        {
            return new FtpFile( ftpClient, fileName, path );
        }

        public async Task<IReadOnlyCollection<INode>> GetAllNodesAsync()
        {
            var directories = await GetDirectoriesAsync();
            var files = await GetFilesAsync();

            return directories.Union( files.Cast<INode>() )
                              .ToList()
                              .AsReadOnly();
        }

        public async Task CreateDirectoryAsync()
        {
            await ftpClient.CreateDirectoryAsync( path );
        }
    }
}