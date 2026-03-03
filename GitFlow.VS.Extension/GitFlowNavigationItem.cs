using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Forms;
using Microsoft.ApplicationInsights;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Controls;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamFoundation.Git.Extensibility;
using TeamExplorer.Common;

namespace GitFlowVS.Extension
{
    [TeamExplorerNavigationItem("59168500-14BC-4EE9-BB1F-7B2B970A4AF6",1500, TargetPageId = "1F9974CD-16C3-4AEF-AED2-0CE37988E2F1")]
    public class GitFlowNavigationItem : TeamExplorerBaseNavigationItem
    {
        private readonly ITeamExplorer teamExplorer;
        private readonly IGitExt gitService;

        [ImportingConstructor]
        public GitFlowNavigationItem([Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            try
            {
                Text = "GitFlow";
                Image = Resources.LinkIcon;
                teamExplorer = GetService<ITeamExplorer>();
                gitService = serviceProvider.GetService(typeof(IGitExt)) as IGitExt;

                if (teamExplorer != null)
                {
                    teamExplorer.PropertyChanged += TeamExplorerOnPropertyChanged;
                }

                UpdateVisible();
            }
            catch (Exception ex)
            {
	            HandleException(ex);
            }
        }

	    private void HandleException(Exception ex)
	    {
		    Logger.Exception(ex);
		    ShowNotification(ex.Message, NotificationType.Error);
	    }

	    protected override void ContextChanged(object sender, ContextChangedEventArgs e)
        {
            UpdateVisible();
            base.ContextChanged(sender, e);
        }

        private void TeamExplorerOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            UpdateVisible();
        }

        private void UpdateVisible()
        {
            // Keep the entry visible even if Git services are not initialized yet.
            // VS can compose Team Explorer parts before IGitExt is available.
            IsVisible = true;
        }

        public override void Execute()
        {
	        try
	        {
				Logger.PageView("Navigate");
	        }
	        catch (Exception ex)
	        {
		       Logger.Exception(ex);
		       ShowNotification(ex.Message, NotificationType.Error);
	        }
            if (teamExplorer != null)
            {
                teamExplorer.NavigateToPage(new Guid(GuidList.GitFlowPage), null);
            }
        }
    }
}
