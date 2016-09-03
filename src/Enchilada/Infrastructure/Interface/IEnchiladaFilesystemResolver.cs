namespace Enchilada.Infrastructure.Interface
{
    public interface IEnchiladaFilesystemResolver
    {
        IDirectory OpenDirectoryReference( string uri );
        IFile OpenFileReference( string uri );
    }
}