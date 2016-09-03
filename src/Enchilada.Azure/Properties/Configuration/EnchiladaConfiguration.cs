namespace Enchilada.Azure.Properties.Configuration
{
    using System.Collections.Generic;

    public class EnchiladaConfiguration
    {
        public IEnumerable<IEnchiladaAdapterConfiguration> Adapters { get; set; }
    }
}