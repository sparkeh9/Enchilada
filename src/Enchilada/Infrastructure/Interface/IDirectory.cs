namespace Enchilada.Infrastructure.Interface
{
    using System.Collections.Generic;

    public interface IDirectory : INode, IEnumerable<INode>
    {

        IEnumerable<IFile> Files { get; }
        IEnumerable<IDirectory> Directories { get; }
        IEnumerable<IDirectory> GetDirectories( string path );
        IDirectory GetDirectory( string path );
        void CreateDirectory();
        IFile GetFile( string fileName );
    }
}