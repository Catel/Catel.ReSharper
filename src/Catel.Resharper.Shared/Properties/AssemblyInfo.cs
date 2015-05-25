﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyInfo.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Reflection;

using JetBrains.ActionManagement;

#if R81 || R82
using JetBrains.Application.PluginSupport;
#endif

[assembly: AssemblyTitle("Catel.ReSharper")]
[assembly: AssemblyProduct("Catel.ReSharper")]
[assembly: AssemblyDescription("Catel.ReSharper")]

[assembly: ActionsXml("Catel.ReSharper.Actions.xml")]

#if R81 || R82
// The following information is displayed by ReSharper in the Plugins dialog
[assembly: PluginTitle("Catel.ReSharper")]
[assembly: PluginDescription("ReSharper plugin for Catel")]
[assembly: PluginVendor("Catel development team")]
#endif