﻿// <auto-generated />
using System.Reflection;
using System.Runtime.InteropServices;
using Android.App;
using Android.Content.PM;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Xalapa Limpia.Droid")]
[assembly: AssemblyDescription("Xalapa Limpia Android Application")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("H. Ayuntamiento de Xalapa")]
[assembly: AssemblyProduct("Xalapa Limpia")]
[assembly: AssemblyCopyright("Copyright © 2016 H. Ayuntamiento de Xalapa")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

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
[assembly: AssemblyVersion("1.0.8.8")]
[assembly: AssemblyFileVersion("1.0.8.8")]

// Add some common permissions, these can be removed if not needed
[assembly: UsesPermission(Android.Manifest.Permission.Internet)]
[assembly: UsesPermission(Android.Manifest.Permission.WriteExternalStorage)]
[assembly: UsesPermission(Android.Manifest.Permission.AccessCoarseLocation)]
[assembly: UsesPermission(Android.Manifest.Permission.AccessFineLocation)]
[assembly: UsesPermission(Android.Manifest.Permission.AccessLocationExtraCommands)]
[assembly: UsesPermission(Android.Manifest.Permission.AccessMockLocation)]
[assembly: UsesPermission(Android.Manifest.Permission.AccessNetworkState)]
[assembly: UsesPermission(Android.Manifest.Permission.AccessWifiState)]
[assembly: UsesPermission(Android.Manifest.Permission.WakeLock)]
[assembly: UsesPermission("com.google.android.c2dm.permission.RECEIVE")]
[assembly: UsesPermission("mx.gob.xalapa.limpia.permission.C2D_MESSAGE")]
[assembly: Permission(Name = "mx.gob.xalapa.limpia.permission.C2D_MESSAGE", ProtectionLevel = Protection.Signature)]

#if DEBUG
[assembly: MetaData("com.google.android.maps.v2.API_KEY", Value = "AIzaSyD_cUK3_yZMr_qn0jOCT9zA-ow6AG7NK_k")]
#else
[assembly: MetaData("com.google.android.maps.v2.API_KEY", Value = "AIzaSyDsC2GxVpceiE8OqLlRcDOpsbJbaeZDWBw")]
#endif
