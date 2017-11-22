// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExposeModelPropertyDataAction.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.ReSharper.CatelProperties.Actions
{
    using Catel.Logging;
    using Catel.ReSharper.CatelProperties.Providers;
    
#if !R2017X
    using JetBrains.ActionManagement;
#endif

    using JetBrains.ReSharper.Feature.Services.Generate.Actions;
    using JetBrains.UI.RichText;

#if R2017X
    using JbActionAttribute = JetBrains.Application.UI.ActionsRevised.Menu.ActionAttribute;
#elif R8X
    using JbActionAttribute = JetBrains.ActionManagement.ActionHandlerAttribute;
#else
    using JbActionAttribute = JetBrains.UI.ActionsRevised.ActionAttribute;
#endif

    [JbActionAttribute(Id)]
    public class ExposeModelPropertyDataAction : GenerateActionBase<ExposeModelPropertyDataItemProvider>
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public const string Id = "Expose.ModelPropertyData";

        protected override RichText Caption
        {
            get { return "Expose model properties"; }
        }

        protected override bool ShowMenuWithOneItem
        {
            get { return true; }
        }
    }
}