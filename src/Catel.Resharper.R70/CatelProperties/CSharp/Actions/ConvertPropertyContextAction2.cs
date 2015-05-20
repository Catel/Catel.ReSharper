// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConvertPropertyContextAction2.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.ReSharper.CatelProperties.CSharp.Actions
{
    using Catel.Logging;

#if R90
    using JetBrains.ReSharper.Feature.Services.ContextActions;
    using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
#else
    using JetBrains.ReSharper.Feature.Services.Bulbs;
    using JetBrains.ReSharper.Feature.Services.CSharp.Bulbs;
#endif

    using JetBrains.ReSharper.Psi.CSharp.Tree;

    /// <summary>
    ///     The convert property context action 2.
    /// </summary>
    [ContextAction(Name = Name, Group = "C#", Description = Description, Priority = -22)]
    public sealed class ConvertPropertyContextAction2 : PropertyContextActionBase
    {
        #region Constants

        /// <summary>
        /// The description.
        /// </summary>
        private const string Description = "ConvertPropertyContextAction2Description";

        /// <summary>
        /// The name.
        /// </summary>
        private const string Name = "ConvertPropertyContextAction2";

        #endregion

        #region Static Fields

        /// <summary>
        ///     The log.
        /// </summary>
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertPropertyContextAction2"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        public ConvertPropertyContextAction2(ICSharpContextActionDataProvider provider)
            : base(provider)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets Text.
        /// </summary>
        public override string Text
        {
            get
            {
                return "To Catel property with property changed notification method and event argument forwarded";
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The convert property.
        /// </summary>
        /// <param name="propertyConverter">
        /// The property converter.
        /// </param>
        /// <param name="propertyDeclaration">
        /// The property declaration.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// The <paramref name="propertyConverter"/> is <c>null</c>.
        /// </exception>
        protected override void ConvertProperty(
            PropertyConverter propertyConverter, IPropertyDeclaration propertyDeclaration)
        {
            Argument.IsNotNull(() => propertyConverter);

            propertyConverter.Convert(propertyDeclaration, true, true, true);
        }

        #endregion
    }
}