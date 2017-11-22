// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextActionBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.ReSharper.CSharp
{
#if R9X || R10X || R2017X
    using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
    using JetBrainsContextActionBase = JetBrains.ReSharper.Feature.Services.ContextActions.ContextActionBase;
#else
    using JetBrains.ReSharper.Feature.Services.CSharp.Bulbs;
    using JetBrainsContextActionBase = JetBrains.ReSharper.Intentions.Extensibility.ContextActionBase;
#endif

    public abstract class ContextActionBase : JetBrainsContextActionBase
    {
        protected ContextActionBase(ICSharpContextActionDataProvider provider)
        {
            Argument.IsNotNull(() => provider);

            Provider = provider;
        }

        protected ICSharpContextActionDataProvider Provider { get; private set; }
    }
}