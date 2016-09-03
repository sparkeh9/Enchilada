namespace Enchilada.Azure.Properties.Configuration
{
    using System;

    public interface IEnchiladaAdapterConfiguration
    {
        string AdapterName { get; set; }
        Type FileProvider { get; }
    }
}