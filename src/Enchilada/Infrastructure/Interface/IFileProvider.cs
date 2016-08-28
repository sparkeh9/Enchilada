namespace Enchilada.Infrastructure.Interface
{
    public interface IFileProvider
    {
        IDirectory RootDirectory { get; }
    }
}