// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HideModelPropertyContextAction.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.ReSharper.CatelProperties.CSharp.Actions
{
    using System;
    using System.Linq;

    using Catel.ReSharper.Identifiers;

    using JetBrains.Application.Progress;
    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.Psi.Tree;
    using JetBrains.ReSharper.Psi.CSharp.Tree;
    using JetBrains.TextControl;

#if R8X
    using JetBrains.ReSharper.Feature.Services.Bulbs;
    using JetBrains.ReSharper.Feature.Services.CSharp.Bulbs;
#else
    using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
    using JetBrains.ReSharper.Feature.Services.ContextActions;
#endif

    [ContextAction(Name = Name, Group = "C#", Description = Description, Priority = -23)]
    public sealed class HideModelPropertyContextAction : FieldContextActionBase
    {
        #region Constants
        private const string Description = "HideModelPropertyContextActionDescription";

        private const string Name = "HideModelPropertyContextAction";

        #endregion

        #region Constructors and Destructors
        public HideModelPropertyContextAction(ICSharpContextActionDataProvider provider)
            : base(provider)
        {
        }

        #endregion

        #region Public Properties
        public override string Text
        {
            get
            {
                return "Hide model property";
            }
        }

        #endregion

        #region Methods
        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            ClassDeclaration.RemoveClassMemberDeclaration(FieldDeclaration);
            ClassDeclaration.RemoveClassMemberDeclaration(PropertyDeclaration);

            return null;
        }

        protected override bool IsAvailable()
        {
            ITypeElement typeElement;
            IAttribute viewModelToModelAttribute = null;
            if (CatelMVVM.TryGetViewModelToModelAttributeTypeElement(Provider.PsiModule, Provider.SelectedElement.GetResolveContext(), out typeElement))
            {
#if R2017X
                viewModelToModelAttribute = (from attribute in PropertyDeclaration.Attributes
                                             where
                                                 attribute.TypeReference != null
                                                 && typeElement.Equals(attribute.TypeReference.Resolve().DeclaredElement)
                                             select attribute).FirstOrDefault();
#else
                viewModelToModelAttribute = (from attribute in PropertyDeclaration.Attributes
                                             where
                                                 attribute.TypeReference != null
                                                 && attribute.TypeReference.CurrentResolveResult != null
                                                 && typeElement.Equals(
                                                     attribute.TypeReference.CurrentResolveResult.DeclaredElement)
                                             select attribute).FirstOrDefault();
#endif
            }

            return viewModelToModelAttribute != null && PropertyDeclaration.HasDefaultCatelImplementation();
        }

#endregion
    }
}