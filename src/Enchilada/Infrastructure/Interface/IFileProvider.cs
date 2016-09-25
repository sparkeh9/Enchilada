namespace Enchilada.Infrastructure.Interface
{
    using System;

    public interface IFileProvider : IDisposable
    {
        IDirectory RootDirectory { get; }
        IFile File { get; }
        bool IsDirectory { get; }
        bool IsFile { get; }
    }
}