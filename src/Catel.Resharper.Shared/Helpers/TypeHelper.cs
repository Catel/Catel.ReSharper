// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeHelper.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Catel.ReSharper.Helpers
{
    using JetBrains.Metadata.Reader.API;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.Psi.Modules;

    /// <summary>
    /// The type helper.
    /// </summary>
    internal static class TypeHelper
    {
        public static bool TryGetTypeElement(string typeName, IPsiModule psiModule, IModuleReferenceResolveContext moduleReferenceResolveContext, out ITypeElement typeElement)
        {
            typeElement = null;

            var typeByClrName = CreateTypeByCLRName(typeName, psiModule, moduleReferenceResolveContext);
            if (typeByClrName != null)
            {
                typeElement = typeByClrName.GetTypeElement();
            }

            return typeElement != null;
        }

        public static IDeclaredType CreateTypeByCLRName(string typeName, IPsiModule psiModule, IModuleReferenceResolveContext moduleReferenceResolveContext)
        {
#if R10X || R2017X
            return TypeFactory.CreateTypeByCLRName(typeName, psiModule);
#else
            return TypeFactory.CreateTypeByCLRName(typeName, psiModule, moduleReferenceResolveContext);
#endif
        }
    }
}