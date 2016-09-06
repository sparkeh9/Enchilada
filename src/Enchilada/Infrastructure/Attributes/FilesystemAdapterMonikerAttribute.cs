namespace Enchilada.Infrastructure.Attributes
{
    using System;

    [ AttributeUsage( AttributeTargets.Class ) ]
    public class FilesystemAdapterMonikerAttribute : Attribute
    {
        public string Moniker { get; set; }

        public FilesystemAdapterMonikerAttribute( string moniker )
        {
            Moniker = moniker;
        }
    }
}