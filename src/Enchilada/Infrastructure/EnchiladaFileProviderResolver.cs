namespace Enchilada.Infrastructure
{
    using System;
    using System.Linq;
    using Configuration;
    using Exceptions;
    using Interface;

    public class EnchiladaFileProviderResolver : IEnchiladaFilesystemResolver
    {
        private readonly EnchiladaConfiguration enchiladaConfiguration;

        public EnchiladaFileProviderResolver( EnchiladaConfiguration enchiladaConfiguration = null )
        {
            this.enchiladaConfiguration = enchiladaConfiguration;
        }

        protected IFileProvider OpenProvider( string uri )
        {
            if ( enchiladaConfiguration == null || !enchiladaConfiguration.Adapters.Any() )
                throw new ConfigurationMissingException();

            var providerUri = new Uri( uri );

            var matchingProvider = enchiladaConfiguration.Adapters
                                                         .FirstOrDefault( x => string.Equals(x.AdapterName, providerUri.Host, StringComparison.OrdinalIgnoreCase) );

            return (IFileProvider) Activator.CreateInstance( matchingProvider.FileProvider, matchingProvider, providerUri.PathAndQuery );
        }

        public IDirectory OpenDirectoryReference( string uri )
        {
            var provider = OpenProvider( uri );

            return provider.RootDirectory;
        }

        public IFile OpenFileReference( string uri )
        {
            var provider = OpenProvider( uri );

            return provider.File;
        }
    }
}
