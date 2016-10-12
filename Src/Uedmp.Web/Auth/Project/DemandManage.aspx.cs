using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Uedmp.Business;
using Uedmp.Entity;
using Uedmp.Web.Common;

namespace Uedmp.Web.Auth.Project
{
    public partial class DemandManage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                AddDemandHyperLink.NavigateUrl = String.Format(
                        "~/Auth/Project/DemandEdit.aspx?{0}={1}",
                        QueryStringKeys.PROJECT_ID,
                        QueryStringKeys.GetProjectId(Page)
                    );
            }
        }
    }
}