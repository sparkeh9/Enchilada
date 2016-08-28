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

        public IFileProvider OpenProvider( string uri )
        {
            if ( enchiladaConfiguration == null || !enchiladaConfiguration.Adapters.Any() )
                throw new ConfigurationMissingException();

            var providerUri = new Uri( uri );

            var matchingProvider = enchiladaConfiguration.Adapters
                                                         .FirstOrDefault( x => x.AdapterName == providerUri.Host );

            return (IFileProvider) Activator.CreateInstance( matchingProvider.FileProvider, matchingProvider, providerUri.PathAndQuery );
        }
    }
}