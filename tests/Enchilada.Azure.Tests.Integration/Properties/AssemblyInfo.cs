using System.Reflection;
using System.Runtime.InteropServices;
using Xunit;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyConfiguration( "" )]
[assembly: AssemblyCompany( "" )]
[assembly: AssemblyProduct( "Enchilada.Azure.Tests.Unit" )]
[assembly: AssemblyTrademark( "" )]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible( false )]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid( "ae7a1fcc-5590-4c9e-af41-698965c6938a" )]

// Match the FTP integration tests and run Azure tests sequentially within this assembly.
[assembly: CollectionBehavior( DisableTestParallelization = true )]
