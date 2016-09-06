namespace Enchilada.AspNetCore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Configuration;
    using Infrastructure;
    using Infrastructure.Attributes;
    using Infrastructure.Interface;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyModel;

    public static class EnchiladaServiceCollectionExtensions
    {
        public static IServiceCollection AddEnchilada( this IServiceCollection serviceCollection, EnchiladaBuilderOptions builderOptions )
        {
            var configurations = ( from configuredAdapter in builderOptions.Adapters.GetChildren()
                                   let adapterTypes = GetAvailableEnchiladaAdapters( GetAssembliesToCheck( builderOptions ) )
                                   let adapterProperties = configuredAdapter.GetChildren().ToList()
                                   let matchingFilesystemAdapter = GetEnchiladaAdapterType( configuredAdapter, adapterTypes )
                                   where ( matchingFilesystemAdapter != null ) && adapterProperties.Any( section => section.Key.ToLower() == "adapter" )
                                   select CreateConfigurationInstance( matchingFilesystemAdapter, configuredAdapter, adapterProperties ) ).ToList();

            var enchiladaFileProviderResolver = new EnchiladaFileProviderResolver( new EnchiladaConfiguration
                                                                                   {
                                                                                       Adapters = configurations
                                                                                   } );
            serviceCollection.AddSingleton<IEnchiladaFilesystemResolver>( enchiladaFileProviderResolver );

            return serviceCollection;
        }

        private static Type GetEnchiladaAdapterType( IConfiguration configuredAdapters, IEnumerable<Type> availableAdapters )
        {
            string adapterMoniker = configuredAdapters.GetSection( "adapter" ).Value;

            var matchingFilesystemAdapter = ( from adapter in availableAdapters
                                              let moniker = adapter.GetTypeInfo().GetCustomAttribute<FilesystemAdapterMonikerAttribute>()
                                              where ( moniker?.Moniker != null ) && string.Equals( moniker.Moniker, adapterMoniker, StringComparison.CurrentCultureIgnoreCase )
                                              select adapter ).SingleOrDefault();
            return matchingFilesystemAdapter;
        }

        private static IEnchiladaAdapterConfiguration CreateConfigurationInstance( Type matchingFilesystemAdapter, IConfigurationSection configuredAdapters, List<IConfigurationSection> adapterProperties )
        {
            var instance = (IEnchiladaAdapterConfiguration) Activator.CreateInstance( matchingFilesystemAdapter );
            instance.AdapterName = configuredAdapters.Key;

            foreach ( var adapterProperty in adapterProperties )
                try
                {
                    var property = matchingFilesystemAdapter.GetProperty( adapterProperty.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance );

                    if ( property == null )
                        continue;

                    if ( property.PropertyType == typeof( bool ) )
                    {
                        property.SetValue( instance, Convert.ToBoolean( adapterProperty.Value ) );
                        continue;
                    }

                    if ( property.PropertyType == typeof( int ) )
                    {
                        property.SetValue( instance, Convert.ToInt32( adapterProperty.Value ) );
                        continue;
                    }

                    if ( property.PropertyType == typeof( long ) )
                    {
                        property.SetValue( instance, Convert.ToInt64( adapterProperty.Value ) );
                        continue;
                    }

                    property.SetValue( instance, adapterProperty.Value );
                }
                catch ( ArgumentException ) {}
            return instance;
        }

        private static List<Type> GetAvailableEnchiladaAdapters( IEnumerable<Assembly> assemblies )
        {
            var availableAdapters = assemblies.SelectMany( assembly => assembly.GetTypes() )
                                              .Where( type => typeof( IEnchiladaAdapterConfiguration ).IsAssignableFrom( type ) && type.GetTypeInfo().IsClass )
                                              .ToList();
            return availableAdapters;
        }

        private static IEnumerable<Assembly> GetAssembliesToCheck( EnchiladaBuilderOptions builderOptions )
        {
            var assembliesToCheck = new List<string> { Assembly.GetEntryAssembly().GetName().Name };
            assembliesToCheck.AddRange( builderOptions.Namespaces );

            var assemblies = DependencyContext.Default
                                              .RuntimeLibraries
                                              .Where( x => assembliesToCheck.Contains( x.Name ) || x.Name.StartsWith( "Enchilada" ) )
                                              .Select( x => Assembly.Load( new AssemblyName( x.Name ) ) );
            return assemblies;
        }
    }
}