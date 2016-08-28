namespace Enchilada.Infrastructure.Interface
{
    public interface IEnchiladaFilesystemResolver
    {
        IFileProvider OpenProvider( string uri );
    }
}