// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelBaseModelPropertyDataBuilder.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.ReSharper.CatelProperties.CSharp.Builders
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Catel.Logging;
    using Catel.ReSharper.Helpers;
    using Catel.ReSharper.CatelProperties.CSharp.Patterns;
    using Catel.ReSharper.Identifiers;
    using JetBrains.Metadata.Reader.API;
    using JetBrains.ReSharper.Feature.Services.CSharp.Generate;
    using JetBrains.ReSharper.Feature.Services.Generate;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.Psi.CSharp;
    using JetBrains.ReSharper.Psi.CSharp.Tree;
    using JetBrains.ReSharper.Psi.ExtensionsAPI.Tree;
    using JetBrains.ReSharper.Psi.Util;
    using JetBrains.Util;

#if R80 || R81 || R82
    using JetBrains.Metadata.Reader.API;
    using JetBrains.ReSharper.Psi.Modules;
    using JetBrains.ReSharper.Psi.Tree;
#endif

#if R90
    using JetBrains.Metadata.Reader.API;
#endif

    [GeneratorBuilder(WellKnownGenerationActionKinds.ExposeModelPropertiesAsCatelDataProperties, typeof(CSharpLanguage))]
    internal sealed class ViewModelBaseModelPropertyDataBuilder : PropertyDataBuilderBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public override double Priority
        {
            get { return 0; }
        }

        protected override void Process(CSharpGeneratorContext context)
        {
            Argument.IsNotNull(() => context);

            var factory = CSharpElementFactory.GetInstance(context.Root.GetPsiModule());
            var viewModelToModelAttributeClrType = TypeHelper.CreateTypeByCLRName(CatelMVVM.ViewModelToModelAttribute, context.PsiModule, UniversalModuleReferenceContext.Instance);

            var declaredElements = context.InputElements.OfType<GeneratorDeclaredElement>().ToList();
            var classLikeDeclaration = context.ClassDeclaration;
            if (classLikeDeclaration != null)
            {
#if R2017X
                var includeInSerialization = bool.Parse(context.GetOption(OptionIds.IncludePropertyInSerialization));
                var notificationMethod = bool.Parse(context.GetOption(OptionIds.ImplementPropertyChangedNotificationMethod));
                var forwardEventArgument = bool.Parse(context.GetOption(OptionIds.ForwardEventArgumentToImplementedPropertyChangedNotificationMethod));
#else
                var includeInSerialization = bool.Parse(context.GetGlobalOptionValue(OptionIds.IncludePropertyInSerialization));
                var notificationMethod = bool.Parse(context.GetGlobalOptionValue(OptionIds.ImplementPropertyChangedNotificationMethod));
                var forwardEventArgument = bool.Parse(context.GetGlobalOptionValue(OptionIds.ForwardEventArgumentToImplementedPropertyChangedNotificationMethod));
#endif
                var propertyConverter = new PropertyConverter(factory, context.PsiModule, (IClassDeclaration)classLikeDeclaration);
                foreach (var declaredElement in declaredElements)
                {
                    var model = (IProperty)declaredElement.GetGroupingObject();
                    var modelProperty = (IProperty)declaredElement.DeclaredElement;
                    if (model != null)
                    {
                        Log.Debug("Computing property name");
                        string propertyName = string.Empty;

                        var cSharpTypeMemberDeclarations = new List<ICSharpTypeMemberDeclaration>();

                        IClassLikeDeclaration currentClassDeclaration = classLikeDeclaration;

                        do
                        {
                            cSharpTypeMemberDeclarations.AddRange(currentClassDeclaration.MemberDeclarations);
                            var superType = currentClassDeclaration.SuperTypes.FirstOrDefault(type => type.IsClassType());
                            if (superType != null)
                            {
                                var superTypeTypeElement = superType.GetTypeElement();
                                if (superTypeTypeElement != null)
                                {
                                    currentClassDeclaration = (IClassLikeDeclaration)superTypeTypeElement.GetDeclarations().FirstOrDefault();
                                }
                            }
                        }
                        while (currentClassDeclaration != null);

                        if (!cSharpTypeMemberDeclarations.Exists(declaration => declaration.DeclaredName == modelProperty.ShortName))
                        {
                            propertyName = modelProperty.ShortName;
                        }

                        if (string.IsNullOrEmpty(propertyName) && !cSharpTypeMemberDeclarations.Exists(declaration => declaration.DeclaredName == model.ShortName + modelProperty.ShortName))
                        {
                            propertyName = model.ShortName + modelProperty.ShortName;
                        }

                        int idx = 0;
                        while (string.IsNullOrEmpty(propertyName))
                        {
                            if (!cSharpTypeMemberDeclarations.Exists(declaration => declaration.DeclaredName == model.ShortName + modelProperty.ShortName + idx.ToString(CultureInfo.InvariantCulture)))
                            {
                                propertyName = model.ShortName + modelProperty.ShortName + idx.ToString(CultureInfo.InvariantCulture);
                            }

                            idx++;
                        }

                        Log.Debug("Adding property '{0}'", propertyName);
                        var propertyDeclaration = (IPropertyDeclaration)factory.CreateTypeMemberDeclaration(ImplementationPatterns.AutoProperty, modelProperty.Type, propertyName);

                        var modelMemberDeclaration = model.GetDeclarations().FirstOrDefault();
                        if (modelMemberDeclaration != null && modelMemberDeclaration.Parent != null)
                        {
                            var modelClassDeclaration = modelMemberDeclaration.Parent.Parent;
                            if (modelClassDeclaration == classLikeDeclaration)
                            {
                                propertyDeclaration = ModificationUtil.AddChildAfter(modelClassDeclaration, modelMemberDeclaration, propertyDeclaration);
                            }
                            else if (classLikeDeclaration.Body != null && classLikeDeclaration.Body.FirstChild != null)
                            {

                                propertyDeclaration = ModificationUtil.AddChildAfter(classLikeDeclaration.Body.FirstChild, propertyDeclaration);
                            }
                        }

                        var fixedArguments = new List<AttributeValue> { new AttributeValue(ConstantValueHelper.CreateStringValue(model.ShortName, context.PsiModule, UniversalModuleReferenceContext.Instance)) };

                        if (propertyName != modelProperty.ShortName)
                        {
                            fixedArguments.Add(new AttributeValue(ConstantValueHelper.CreateStringValue(modelProperty.ShortName, context.PsiModule, UniversalModuleReferenceContext.Instance)));
                        }

                        Log.Debug("Adding attribute ViewModelToModel to property '{0}'", propertyName);
                        IAttribute attribute = factory.CreateAttribute(viewModelToModelAttributeClrType.GetTypeElement(), fixedArguments.ToArray(), new Pair<string, AttributeValue>[] { });
                        propertyDeclaration.AddAttributeAfter(attribute, null);
                        propertyConverter.Convert(propertyDeclaration, includeInSerialization, notificationMethod, forwardEventArgument);
                    }
                }
            }
        }
    }
}