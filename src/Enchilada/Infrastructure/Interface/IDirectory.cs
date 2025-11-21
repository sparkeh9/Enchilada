namespace Enchilada.Infrastructure.Interface
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IDirectory : INode, IEnumerable<INode>
    {
        Task<IReadOnlyCollection<IDirectory>> GetDirectoriesAsync();
        Task<IReadOnlyCollection<IFile>> GetFilesAsync();
        IDirectory GetDirectory( string path );
        Task CreateDirectoryAsync();
        IFile GetFile( string fileName );
        Task<IReadOnlyCollection<INode>> GetAllNodesAsync();
        Task DeleteAsync();
    }
}