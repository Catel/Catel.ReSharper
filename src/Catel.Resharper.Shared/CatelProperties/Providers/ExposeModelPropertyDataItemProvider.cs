// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExposeModelPropertyDataItemProvider.cs" company="Catel development team">
//   Copyright (c) 2008 - 2012 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Catel.ReSharper.CatelProperties.Providers
{
    using System.Collections.Generic;

    using Catel.Logging;
    using Catel.ReSharper.CatelProperties.Workflows;

    using JetBrains.Application.DataContext;
    using JetBrains.ProjectModel;
    using JetBrains.ReSharper.Feature.Services.Generate.Actions;
    using JetBrains.ReSharper.Psi;
    using JetBrains.UI.Icons;
#if !R2017X
    using DataConstants = JetBrains.ProjectModel.DataContext.DataConstants;
#endif

    [GenerateProvider]
    public class ExposeModelPropertyDataItemProvider : IGenerateActionProvider
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public IEnumerable<IGenerateActionWorkflow> CreateWorkflow(IDataContext dataContext)
        {
            Argument.IsNotNull(() => dataContext);
#if R2017X
            var solution = dataContext.GetData(JetBrains.ProjectModel.DataContext.ProjectModelDataConstants.SOLUTION);
#else
            var solution = dataContext.GetData(DataConstants.SOLUTION);
#endif
            var iconManager = solution.GetComponent<PsiIconManager>();
            var icon = iconManager.GetImage(CLRDeclaredElementType.PROPERTY);

            yield return new ExposeModelPropertyDataWorkflow(icon);
        }
    }
}