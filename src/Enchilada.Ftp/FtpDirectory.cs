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
        private readonly string directoryPath;
        private readonly string endOfPath;
        private IReadOnlyCollection<INode> allNodes;

        public string Name => directoryPath;


        public DateTime? LastModified => allNodes.OrderBy( x => x.LastModified )
                                                 .FirstOrDefault()?.LastModified;

        public bool IsDirectory => true;

        public string RealPath => $"/{directoryPath}";

        public bool Exists => true;

        public FtpDirectory( FtpClient ftpClient, string directoryPath )
        {
            endOfPath = directoryPath.Split( '/' ).LastOrDefault();

            this.ftpClient = ftpClient;
            this.directoryPath = directoryPath;


            EnsureConnectedAsync().ContinueWith( async task =>
            {
                allNodes = task.IsFaulted
                    ? new List<INode>().AsReadOnly()
                    : await GetAllNodesAsync();
            } );
        }

        private async Task EnsureConnectedAsync()
        {
            if ( ftpClient.IsConnected )
                return;

            await ftpClient.LoginAsync();

            if ( !directoryPath.IsNullOrWhiteSpace() )
                await ftpClient.ChangeWorkingDirectoryAsync( directoryPath );
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

            allNodes = new List<INode>().AsReadOnly();
        }

        public IEnumerator<INode> GetEnumerator()
        {
            return allNodes
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public async Task<IReadOnlyCollection<IDirectory>> GetDirectoriesAsync()
        {
            var ftpNodes = await ftpClient.ListDirectoriesAsync();

            return ftpNodes.Select( ftpNode => new FtpDirectory( ftpClient, $"{directoryPath}/{ftpNode.Name}" ) )
                           .ToList()
                           .AsReadOnly();
        }

        public async Task<IReadOnlyCollection<IFile>> GetFilesAsync()
        {
            var ftpNodes = await ftpClient.ListFilesAsync();

            return ftpNodes.Select( ftpNode => new FtpFile( ftpClient, ftpNode.Name, directoryPath ) )
                           .ToList()
                           .AsReadOnly();
        }

        public IDirectory GetDirectory( string path )
        {
            return new FtpDirectory( ftpClient, $"{directoryPath}/{path}" );
        }

        public IFile GetFile( string fileName )
        {
            return new FtpFile( ftpClient, fileName.RemovePath(), directoryPath );
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
            await ftpClient.CreateDirectoryAsync( directoryPath );
            allNodes = await GetAllNodesAsync();
        }
    }
}