// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExcludePropertyFromSerializationContextAction.cs" company="Catel development team">
//   Copyright (c) 2008 - 2013 Catel development team. All rights reserved.
// </copyright>
// <summary>
//   The remove property context action.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.ReSharper.CatelProperties.CSharp.Actions
{
    using System;

    using JetBrains.Application.Progress;
    using JetBrains.ProjectModel;
#if R90
    using JetBrains.ReSharper.Feature.Services.CSharp.Analyses.Bulbs;
    using JetBrains.ReSharper.Feature.Services.ContextActions;
#else
    using JetBrains.ReSharper.Feature.Services.Bulbs;
    using JetBrains.ReSharper.Feature.Services.CSharp.Bulbs;
#endif
    using JetBrains.ReSharper.Psi;
    using JetBrains.ReSharper.Psi.CSharp.Parsing;
    using JetBrains.ReSharper.Psi.CSharp.Tree;
    using JetBrains.TextControl;

    /// <summary>
    ///     The remove property context action.
    /// </summary>
    [ContextAction(Name = Name, Group = "C#", Description = Description, Priority = -21)]
    public sealed class ExcludePropertyFromSerializationContextAction : FieldContextActionBase
    {
        #region Constants

        /// <summary>
        /// The description.
        /// </summary>
        private const string Description = "ExcludePropertyFromSerializationContextActionDescription";

        /// <summary>
        /// The name.
        /// </summary>
        private const string Name = "ExcludePropertyFromSerializationContextAction";

        #endregion

        #region Fields

        /// <summary>
        ///     The invocation expression.
        /// </summary>
        private IInvocationExpression invocationExpression;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcludePropertyFromSerializationContextAction"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        public ExcludePropertyFromSerializationContextAction(ICSharpContextActionDataProvider provider)
            : base(provider)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the text.
        /// </summary>
        public override string Text
        {
            get { return "Exclude from serialization"; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The execute psi transaction.
        /// </summary>
        /// <param name="solution">
        /// The solution.
        /// </param>
        /// <param name="progress">
        /// The progress.
        /// </param>
        /// <returns>
        /// The System.Action`1[T -&gt; JetBrains.TextControl.ITextControl].
        /// </returns>
        protected override Action<ITextControl> ExecutePsiTransaction(ISolution solution, IProgressIndicator progress)
        {
            if (this.invocationExpression.ArgumentList.Arguments.Count == 4)
            {
                this.invocationExpression.RemoveArgument(this.invocationExpression.ArgumentList.Arguments[3]);
                ICSharpArgument argument = this.Provider.ElementFactory.CreateArgument(ParameterKind.VALUE, this.Provider.ElementFactory.CreateExpression("false"));
                this.invocationExpression.AddArgumentAfter(argument, this.invocationExpression.ArgumentList.Arguments[2]);
            }
            else
            {
                if (this.invocationExpression.ArgumentList.Arguments.Count == 1)
                {
                    ICSharpArgument argument = this.Provider.ElementFactory.CreateArgument(ParameterKind.VALUE, this.Provider.ElementFactory.CreateExpression("default($0)", this.PropertyDeclaration.Type));
                    this.invocationExpression.AddArgumentAfter(argument, this.invocationExpression.ArgumentList.Arguments[0]);
                }

                if (this.invocationExpression.ArgumentList.Arguments.Count == 2)
                {
                    ICSharpArgument argument = this.Provider.ElementFactory.CreateArgument(ParameterKind.VALUE, this.Provider.ElementFactory.CreateExpression("null"));
                    this.invocationExpression.AddArgumentAfter(argument, this.invocationExpression.ArgumentList.Arguments[1]);
                }

                if (this.invocationExpression.ArgumentList.Arguments.Count == 3)
                {
                    ICSharpArgument argument = this.Provider.ElementFactory.CreateArgument(ParameterKind.VALUE, this.Provider.ElementFactory.CreateExpression("false"));
                    this.invocationExpression.AddArgumentAfter(argument, this.invocationExpression.ArgumentList.Arguments[2]);
                }
            }

            return null;
        }

        /// <summary>
        ///     Indicates whether the the action is available.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if is available, otherwise <c>false</c>.
        /// </returns>
        protected override bool IsAvailable()
        {
            var expressionInitializer = this.FieldDeclaration.Initial as IExpressionInitializer;
            this.invocationExpression = null;
            if (expressionInitializer != null)
            {
                this.invocationExpression = expressionInitializer.Value as IInvocationExpression;
            }

            return this.invocationExpression != null && (this.invocationExpression.ArgumentList.Arguments.Count < 4 || ((this.invocationExpression.ArgumentList.Arguments[3].Value is ICSharpLiteralExpression) && (this.invocationExpression.ArgumentList.Arguments[3].Value as ICSharpLiteralExpression).Literal.GetTokenType() == CSharpTokenType.TRUE_KEYWORD));
        }

        #endregion
    }
}