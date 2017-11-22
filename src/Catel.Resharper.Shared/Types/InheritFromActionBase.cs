// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InheritFromActionBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.ReSharper.Types
{
    using System;

    using Catel.ReSharper.CSharp;
    using Catel.ReSharper.Helpers;

    using CatelProperties.CSharp;
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

#if !R8X
    using JetBrains.ReSharper.Resources.Shell;
    using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
#endif

    public abstract class InheritFromActionBase : ContextActionBase
    {
        private const string InheritFromTextPattern = "Inherit from '{0}'";

        private IClassDeclaration _classDeclaration;

        private IDeclaredType _superType;

        protected InheritFromActionBase(ICSharpContextActionDataProvider provider)
            : base(provider)
        {
        }

        public override string Text
        {
            get
            {
                return string.Format(InheritFromTextPattern, SuperTypeName);
            }
        }

        protected abstract string SuperTypeName { get; }

        public override bool IsAvailable(IUserDataHolder cache)
        {
            using (ReadLockCookie.Create())
            {
                if (Provider.SelectedElement != null)
                {
                    _superType = TypeHelper.CreateTypeByCLRName(SuperTypeName, Provider.PsiModule, Provider.SelectedElement.GetResolveContext());
                    if (_superType.GetTypeElement() != null)
                    {
                        _classDeclaration = Provider.SelectedElement.Parent as IClassDeclaration;
                    }
                }
            }

            // !_classDeclaration.IsStatic doesn't work, IsStatic is returns true
            return _classDeclaration != null && !_classDeclaration.IsStaticEx() && _classDeclaration.SuperTypes.IsEmpty();
        }

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            _classDeclaration.SetSuperClass(_superType);
            return null;
        }
    }
}