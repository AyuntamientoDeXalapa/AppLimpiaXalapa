﻿using System.Reflection;
using System.Runtime.InteropServices;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.WinRT;
using Xamarin.Forms.Platform.WinRT;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle("Xamarin.Forms.Maps.WinRT.Phone")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]

[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: ComVisible(false)]
[assembly: ExportRenderer(typeof (Map), typeof (MapRenderer))]