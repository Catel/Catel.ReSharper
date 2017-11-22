// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataObjectBasePropertyDataBuilder.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.ReSharper.CatelProperties.CSharp.Builders
{
    using System.Collections.Generic;
    using System.Linq;

    using Catel.Logging;

    using JetBrains.ReSharper.Feature.Services.CSharp.Generate;
    using JetBrains.ReSharper.Feature.Services.Generate;
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.Psi.CSharp;
    using JetBrains.ReSharper.Psi.CSharp.Tree;

    [GeneratorBuilder(WellKnownGenerationActionKinds.GenerateCatelDataProperties, typeof(CSharpLanguage))]
    internal sealed class DataObjectBaseOrModelBasePropertyDataBuilder : PropertyDataBuilderBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        #region Public Properties

        public override double Priority
        {
            get { return 0; }
        }
        #endregion

        #region Public Methods and Operators

        /// <exception cref="System.ArgumentNullException">The <paramref name="context"/> is <c>null</c>.</exception>
        protected override void Process(CSharpGeneratorContext context)
        {
            Argument.IsNotNull(() => context);

            var factory = CSharpElementFactory.GetInstance(context.Root.GetPsiModule());
            var typeOwners = context.InputElements.OfType<GeneratorDeclaredElement<ITypeOwner>>().ToList();
#if R2017X
            var includeInSerialization = bool.Parse(context.GetOption(OptionIds.IncludePropertyInSerialization));
            var notificationMethod = bool.Parse(context.GetOption(OptionIds.ImplementPropertyChangedNotificationMethod));
            var forwardEventArgument = bool.Parse(context.GetOption(OptionIds.ForwardEventArgumentToImplementedPropertyChangedNotificationMethod));
#else
            var includeInSerialization = bool.Parse(context.GetGlobalOptionValue(OptionIds.IncludePropertyInSerialization));
            var notificationMethod = bool.Parse(context.GetGlobalOptionValue(OptionIds.ImplementPropertyChangedNotificationMethod));
            var forwardEventArgument = bool.Parse(context.GetGlobalOptionValue(OptionIds.ForwardEventArgumentToImplementedPropertyChangedNotificationMethod));
#endif

            var propertyConverter = new PropertyConverter(factory, context.PsiModule, (IClassDeclaration)context.ClassDeclaration);
            foreach (var typeOwner in typeOwners)
            {
                var propertyDeclaredElement = typeOwner.DeclaredElement;
                var propertyDeclaration = (IPropertyDeclaration)propertyDeclaredElement.GetDeclarations().FirstOrDefault();
                if (propertyDeclaration != null)
                {
                    propertyConverter.Convert(propertyDeclaration, includeInSerialization, notificationMethod, forwardEventArgument);
                }
            }
        }

#endregion
    }
}