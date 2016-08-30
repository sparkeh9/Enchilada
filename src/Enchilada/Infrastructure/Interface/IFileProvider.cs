namespace Enchilada.Infrastructure.Interface
{
    public interface IFileProvider
    {
        IDirectory RootDirectory { get; }
        IFile File { get; }
        bool IsDirectory { get; }
        bool IsFile { get; }
    }
}