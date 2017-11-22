// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyContextActionBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.ReSharper.CatelProperties.CSharp.Actions
{
    using System;

    using Catel.Logging;
    using Catel.ReSharper.CSharp;
    using Catel.ReSharper.Identifiers;

    using JetBrains.Application;
    using JetBrains.Application.Progress;
    using JetBrains.ProjectModel;
#if !R2017X
    using JetBrains.ReSharper.Feature.Services.CSharp.Bulbs;
#endif
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.Psi.CSharp.Tree;
    using JetBrains.ReSharper.Psi.Tree;
    using JetBrains.TextControl;
    using JetBrains.Util;

#if R9X || R10X || R2017X
    using JetBrains.ReSharper.Resources.Shell;
    using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
#endif

    public abstract class PropertyContextActionBase : ContextActionBase
    {
        #region Static Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Fields
        private IPropertyDeclaration _propertyDeclaration;

        private IClassDeclaration _classDeclaration;

        #endregion

        #region Constructors and Destructors
        protected PropertyContextActionBase(ICSharpContextActionDataProvider provider)
            : base(provider)
        {
        }

        #endregion

        #region Public Methods and Operators
        public override sealed bool IsAvailable(IUserDataHolder cache)
        {
            using (ReadLockCookie.Create())
            {
                var selectedElement = Provider.SelectedElement;
                var moduleReferenceResolveContext = selectedElement.GetResolveContext();
                if (selectedElement != null && selectedElement.Parent is IPropertyDeclaration)
                {
                    _propertyDeclaration = selectedElement.Parent as IPropertyDeclaration;
                    if (_propertyDeclaration.IsAuto && _propertyDeclaration.Parent != null
                        && _propertyDeclaration.Parent.Parent is IClassDeclaration)
                    {
                        _classDeclaration = _propertyDeclaration.Parent.Parent as IClassDeclaration;
                    }
                }
            }

            return _classDeclaration != null && _classDeclaration.DeclaredElement != null
                   && (_classDeclaration.DeclaredElement.IsDescendantOf(CatelCore.GetDataObjectBaseTypeElement(Provider.PsiModule, _classDeclaration.GetResolveContext()))
                       || _classDeclaration.DeclaredElement.IsDescendantOf(CatelCore.GetModelBaseTypeElement(Provider.PsiModule, _classDeclaration.GetResolveContext())));
        }

        #endregion

        #region Methods

        protected abstract void ConvertProperty(
            PropertyConverter propertyConverter, IPropertyDeclaration propertyDeclaration);

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            using (WriteLockCookie.Create())
            {
                ConvertProperty(new PropertyConverter(Provider.ElementFactory, Provider.PsiModule, _classDeclaration), _propertyDeclaration);
            }

            return null;
        }

        #endregion
    }
}