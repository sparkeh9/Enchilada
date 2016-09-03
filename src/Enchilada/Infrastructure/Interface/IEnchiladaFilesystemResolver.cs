namespace Enchilada.Infrastructure.Interface
{
    public interface IEnchiladaFilesystemResolver
    {
        IDirectory OpenDirectory( string uri );
        IFile OpenFile( string uri );
    }
}