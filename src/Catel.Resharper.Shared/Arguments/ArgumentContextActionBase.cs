// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArgumentContextActionBase.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.ReSharper.Arguments
{
    using System;
    using System.Collections.Generic;
    using System.Xml;

    using Catel.Logging;
    using Catel.ReSharper.CSharp;
    using Catel.ReSharper.Helpers;
    using Catel.ReSharper.Identifiers;

    using JetBrains.Application;
    using JetBrains.Application.Progress;
    using JetBrains.DocumentModel;
#if !R2017X
    using JetBrains.ReSharper.Feature.Services.CSharp.Bulbs;
#endif
    using JetBrains.ReSharper.Feature.Services.LiveTemplates.Hotspots;
    using JetBrains.ReSharper.Feature.Services.LiveTemplates.LiveTemplates;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.Psi.CSharp.Tree;
    using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
    using JetBrains.ReSharper.Psi.Tree;
    using JetBrains.TextControl;
    using JetBrains.Util;
    using JetBrains.ProjectModel;

#if R8X
    using JetBrains.DocumentModel;
#else
    using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
    using JetBrains.ReSharper.Resources.Shell;
#endif

    public abstract class ArgumentContextActionBase : ContextActionBase
    {
#region Static Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

#endregion

#region Fields
        private ICSharpFunctionDeclaration _methodDeclaration;

        private IRegularParameterDeclaration _parameterDeclaration;

#endregion

#region Constructors and Destructors
        protected ArgumentContextActionBase(ICSharpContextActionDataProvider provider)
            : base(provider)
        {
        }

#endregion

#region Public Methods and Operators

        public override sealed bool IsAvailable(IUserDataHolder cache)
        {
            using (ReadLockCookie.Create())
            {
                if (Provider.SelectedElement != null)
                {
                    var catelArgumentType = TypeHelper.CreateTypeByCLRName(CatelCore.Argument, Provider.PsiModule, Provider.SelectedElement.GetResolveContext());
                    _parameterDeclaration = null;
                    _methodDeclaration = null;

                    if (catelArgumentType.GetTypeElement() != null)
                    {
                        if (Provider.SelectedElement != null && Provider.SelectedElement.Parent is IRegularParameterDeclaration)
                        {
                            _parameterDeclaration = Provider.SelectedElement.Parent as IRegularParameterDeclaration;
                            if (_parameterDeclaration.Parent != null && _parameterDeclaration.Parent != null
                                && _parameterDeclaration.Parent.Parent is ICSharpFunctionDeclaration)
                            {
                                _methodDeclaration =
                                    _parameterDeclaration.Parent.Parent as ICSharpFunctionDeclaration;
                            }
                        }
                    }
                    
                }
            }

            return _parameterDeclaration != null && IsArgumentTypeTheExpected(_parameterDeclaration.Type)
                   && _methodDeclaration != null
                   && !IsArgumentChecked(_methodDeclaration, _parameterDeclaration);
        }

#endregion

#region Methods
        protected abstract ICSharpStatement CreateArgumentCheckStatement(IRegularParameterDeclaration parameterDeclaration);

        protected abstract string CreateExceptionXmlDoc(IRegularParameterDeclaration parameterDeclaration);

        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            Argument.IsNotNull(() => solution);
            Argument.IsNotNull(() => progress);
#if R8X
            IDocCommentBlockNode exceptionCommentBlock = null;
#else
            IDocCommentBlock exceptionCommentBlock = null;
#endif
            var xmlDoc = _methodDeclaration.GetXMLDoc(false);
            if (xmlDoc == null || !IsArgumentCheckDocumented(xmlDoc, _parameterDeclaration))
            {
                // TODO: Move out the initialization of the exception block down (CreateExceptionXmlDoc).
                exceptionCommentBlock =
                    Provider.ElementFactory.CreateDocCommentBlock(
                        CreateExceptionXmlDoc(_parameterDeclaration));

                // TODO: Detect the right position to insert the document node.
#if R8X
                if (_methodDeclaration.FirstChild is IDocCommentBlockNode)
#else
                if (_methodDeclaration.FirstChild is IDocCommentBlock)
#endif
                {
                    var lastChild = _methodDeclaration.FirstChild.LastChild;
                    if (lastChild != null)
                    {
                        exceptionCommentBlock = ModificationUtil.AddChildAfter(lastChild, exceptionCommentBlock);
                    }
                }
                else if (_methodDeclaration.Parent != null)
                {
                    exceptionCommentBlock = ModificationUtil.AddChildBefore(_methodDeclaration.Parent, _methodDeclaration.FirstChild, exceptionCommentBlock);
                }
            }

            // TODO: Detect the right position to insert the code.
            var methodBodyFirstChild = _methodDeclaration.Body.FirstChild;
            Dictionary<string, List<DocumentRange>> fields = null;
            if (methodBodyFirstChild != null)
            {
                var checkStatement = ModificationUtil.AddChildAfter(methodBodyFirstChild, CreateArgumentCheckStatement(_parameterDeclaration));
                fields = checkStatement.GetFields();
                if (exceptionCommentBlock != null)
                {
                    fields.Merge(exceptionCommentBlock.GetFields());
                }
            }
            
            var hotspotInfos = fields != null ? fields.AsHotspotInfos() : new HotspotInfo[] { };
            return hotspotInfos.Length == 0
                       ? (Action<ITextControl>)null
                       : textControl =>
                       {
#if R2017X
                           var hotspotSession =
                               Shell.Instance.GetComponent<LiveTemplatesManager>()
                                   .CreateHotspotSessionAtopExistingText(
                                       solution,
                                       DocumentRange.InvalidRange,
                                       textControl,
                                       LiveTemplatesManager.EscapeAction.LeaveTextAndCaret,
                                       hotspotInfos);
#else
                           var hotspotSession =
                               Shell.Instance.GetComponent<LiveTemplatesManager>()
                                    .CreateHotspotSessionAtopExistingText(
                                        solution,
                                        TextRange.InvalidRange,
                                        textControl,
                                        LiveTemplatesManager.EscapeAction.LeaveTextAndCaret,
                                        hotspotInfos);
#endif

                           hotspotSession.Execute();
                       };
        }

        protected abstract bool IsArgumentCheckDocumented(XmlNode xmlDocOfTheMethod, IRegularParameterDeclaration parameterDeclaration);

        protected abstract bool IsArgumentChecked(ICSharpFunctionDeclaration methodDeclaration, IRegularParameterDeclaration parameterDeclaration);

        protected abstract bool IsArgumentTypeTheExpected(IType type);

#endregion
    }
}