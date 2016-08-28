namespace Enchilada.Infrastructure.Interface
{
    using System;

    public interface INode
    {
        /// <summary>
        /// Provides the name and extension of the given file
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Provides the direct path of the given file
        /// </summary>
        string RealPath { get; }

        /// <summary>
        /// Provides a DateTime as to when the given file was last modified
        /// </summary>
        DateTime LastModified { get; }

        /// <summary>
        /// Determines whether the node is a directory or not
        /// </summary>
        bool IsDirectory { get; }

        bool Exists { get; }

        /// <summary>
        /// Deletes the given file
        /// </summary>
        /// <param name="recursive"></param>
        void Delete( bool recursive = true);
    }
}