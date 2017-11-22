// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;
#if !R2017X
using JetBrains.ActionManagement;
#endif

#if R81 || R82
using JetBrains.Application.PluginSupport;
#endif

[assembly: AssemblyTitle("Catel.ReSharper")]
[assembly: AssemblyProduct("Catel.ReSharper")]
[assembly: AssemblyDescription("Catel.ReSharper")]



#if R81 || R82
// The following information is displayed by ReSharper in the Plugins dialog
[assembly: ActionsXml("Catel.ReSharper.Actions.xml")]
[assembly: PluginTitle("Catel.ReSharper")]
[assembly: PluginDescription("ReSharper plugin for Catel")]
[assembly: PluginVendor("Catel development team")]
#endif