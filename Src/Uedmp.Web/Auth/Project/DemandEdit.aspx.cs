using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Uedmp.Business;
using Uedmp.Entity;
using Uedmp.Web.Common;
using Useease.GeneralDataAccess;

namespace Uedmp.Web.Auth.Project
{
    public partial class DemandEdit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                EtyProject project = QueryStringKeys.GetProject(Page);

                if (project == null)
                {
                    PageForwarding.GoToProjectManage();

                    return;
                }
            }

            DemandSideTextBox.Focus();
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            EtyProject p = QueryStringKeys.GetProject(Page);
            EtyDemand demand = EntityFactories.Default.CreateNew<EtyDemand>();

            demand.Project = p;
            demand.TimeCreated = DateTime.Now;

            DemandUtility.AddDemand(demand);

            EtyDemandVersion version = EntityFactories.Default.CreateNew<EtyDemandVersion>();

            version.Demand = demand;

            version.Creator = Globals.CurrentUser;
            version.TimeCreated = demand.TimeCreated;

            version.DemandSide = DemandSideTextBox.Text;
            version.DemandTitle = TitleTextBox.Text;
            version.DemandContent = ContentTextBox.Text;

            version.DemandPriority = CommonUtility.GetDemandPriorityById(Int32.Parse(DemandPriorityDropDownList.SelectedValue));
            version.DemandImportance = CommonUtility.GetDemandImportanceById(Int32.Parse(DemandImportanceDropDownList.SelectedValue));

            DemandUtility.AddDemandVersion(version);

            PageForwarding.GoToDemandManage(QueryStringKeys.GetProjectId(Page));
        }

        protected int ProjectId
        {
            get { return Int32.Parse(Request.QueryString["ProjectID"]); }
        }

        protected void CancelLinkButton_Click(object sender, EventArgs e)
        {
            PageForwarding.GoToDemandManage(QueryStringKeys.GetProjectId(Page));
        }
    }
}