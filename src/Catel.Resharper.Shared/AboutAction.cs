// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AboutAction.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.ReSharper
{
    using System.Windows.Forms;

    using JetBrains.ActionManagement;
    using JetBrains.Application.DataContext;

#if R9X || R10X
    using JetBrains.UI.ActionsRevised;
#endif

    /// <summary>
    /// The about action
    /// </summary>
#if R9X || R10X
    [Action("Catel.ReSharper.About")]
    public class AboutAction : IExecutableAction
#else
    [ActionHandler("Catel.ReSharper.About")]
    public class AboutAction : IActionHandler
#endif
    {
        #region IActionHandler Members
        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            return true;
        }

        public void Execute(IDataContext context, DelegateExecute nextExecute)
        {
            MessageBox.Show("Catel.ReSharper\nCatel development team\n\nReSharper plugin for Catel", "About Catel.ReSharper", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion
    }
}