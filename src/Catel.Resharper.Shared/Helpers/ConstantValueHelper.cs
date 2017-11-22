namespace Catel.ReSharper.Helpers
{
    using JetBrains.Metadata.Reader.API;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.Psi.Modules;

    internal static class ConstantValueHelper
    {
        internal static ConstantValue CreateStringValue(string p, IPsiModule psiModule, UniversalModuleReferenceContext universalModuleReferenceContext)
        {
#if R10X || R2017X
            return ClrConstantValueFactory.CreateStringValue(p, psiModule);
#else
            return ClrConstantValueFactory.CreateStringValue(p, psiModule, universalModuleReferenceContext);
#endif
        }
    }
}