// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FieldContextActionBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.ReSharper.CatelProperties.CSharp.Actions
{
    using Catel.ReSharper.CatelProperties.CSharp.Helpers;
    using Catel.ReSharper.CSharp;
    using Catel.ReSharper.Identifiers;

    using JetBrains.Application;
#if !R2017X
    using JetBrains.ReSharper.Feature.Services.CSharp.Bulbs;
#endif
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.Psi.CSharp.Tree;
    using JetBrains.ReSharper.Psi.Tree;
    using JetBrains.Util;

#if R9X || R10X || R2017X
    using JetBrains.ReSharper.Resources.Shell;
    using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
#endif

    public abstract class FieldContextActionBase : ContextActionBase
    {
        #region Constructors and Destructors
        protected FieldContextActionBase(ICSharpContextActionDataProvider provider)
            : base(provider)
        {
        }

        #endregion

        #region Properties

        protected IClassDeclaration ClassDeclaration { get; private set; }

        protected IFieldDeclaration FieldDeclaration { get; private set; }

        protected IPropertyDeclaration PropertyDeclaration { get; private set; }

        #endregion

        #region Public Methods and Operators
        public override bool IsAvailable(IUserDataHolder cache)
        {
            ClassDeclaration = null;
            PropertyDeclaration = null;
            FieldDeclaration = null;

            using (ReadLockCookie.Create())
            {
                ITreeNode selectedElement = Provider.SelectedElement;
                if (selectedElement != null && selectedElement.Parent != null
                    && selectedElement.Parent is IFieldDeclaration)
                {
                    FieldDeclaration = selectedElement.Parent as IFieldDeclaration;
                    if (FieldDeclaration.Parent != null && FieldDeclaration.Parent.Parent != null
                        && FieldDeclaration.Parent.Parent.Parent is IClassDeclaration)
                    {
                        ClassDeclaration = FieldDeclaration.Parent.Parent.Parent as IClassDeclaration;
                        var classDeclaredElement = ClassDeclaration.DeclaredElement;
                        if (classDeclaredElement != null && (classDeclaredElement.IsDescendantOf(CatelCore.GetDataObjectBaseTypeElement(Provider.PsiModule, selectedElement.GetResolveContext())) || classDeclaredElement.IsDescendantOf(CatelCore.GetModelBaseTypeElement(Provider.PsiModule, selectedElement.GetResolveContext()))) && (FieldDeclaration.IsStatic && FieldDeclaration.Initial is IExpressionInitializer))
                        {
                            var expressionInitializer = FieldDeclaration.Initial as IExpressionInitializer;
                            if (expressionInitializer.Value is IInvocationExpression)
                            {
                                var invocationExpression = expressionInitializer.Value as IInvocationExpression;
                                if (invocationExpression.InvokedExpression is IReferenceExpression)
                                {
                                    var referenceExpression = invocationExpression.InvokedExpression as IReferenceExpression;
                                    if (referenceExpression.NameIdentifier != null && referenceExpression.NameIdentifier.GetText() == RegisterPropertyExpressionHelper.RegisterPropertyMethodName)
                                    {
                                        PropertyDeclaration = RegisterPropertyExpressionHelper.GetPropertyDeclaration(ClassDeclaration, invocationExpression);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return ClassDeclaration != null && FieldDeclaration != null && PropertyDeclaration != null
                   && IsAvailable();
        }

        #endregion

        #region Methods
        protected abstract bool IsAvailable();

        #endregion
    }
}