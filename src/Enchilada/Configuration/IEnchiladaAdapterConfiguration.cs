namespace Enchilada.Configuration
{
    using System;

    public interface IEnchiladaAdapterConfiguration
    {
        string AdapterName { get; set; }
        Type FileProvider { get; }
    }
}