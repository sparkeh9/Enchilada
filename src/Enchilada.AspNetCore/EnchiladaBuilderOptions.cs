namespace Enchilada.AspNetCore
{
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;

    public class EnchiladaBuilderOptions
    {
        public IConfigurationSection Adapters { get; set; }
        public IEnumerable<string> Namespaces { get; set; }

        public EnchiladaBuilderOptions()
        {
            Namespaces = [];
        }
    }
}