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

namespace Uedmp.Web.Auth.Task
{
    public partial class TaskEdit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ProjectDropDownList.Focus();
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            if (!IsValid)
            {
                return;
            }

            EtyTask task = EntityFactories.Default.CreateNew<EtyTask>();

            task.Demand = DemandUtility.GetDemandById(Convert.ToInt32(DemandDropDownList.SelectedValue));

            task.TaskAssigner = Globals.CurrentUser;
            task.TaskWorker = UserUtility.GetUserById(Convert.ToInt32(TaskWorkerDropDownList.SelectedValue));

            task.TaskName = TaskNameTextBox.Text;
            task.TaskContent = ContentTextBox.Text;

            task.StartTime = GetStartTime();
            task.EndTime = GetEndTime();

            task.WorkTime = Convert.ToDouble(WorkTimeTextBox.Text);

            task.TaskState = CommonUtility.GetTaskStateById(Convert.ToInt32(TaskStateDropDownList.SelectedValue));

            TaskUtility.AddTask(task);

            PageForwarding.GoToTaskManage();
        }

        protected void CancelLinkButton_Click(object sender, EventArgs e)
        {
            PageForwarding.GoToTaskManage();
        }

        protected DateTime GetStartTime()
        {
            DateTime date = DateTime.Parse(StartTimeDateTextBox.Text);
            int hours = Int32.Parse(StartTimeHourDropDownList.SelectedValue);
            int minutes = Int32.Parse(StartTimeMinuteDropDownList.SelectedValue);

            DateTime result = date.AddHours(hours).AddMinutes(minutes);

            return result;
        }

        protected DateTime GetEndTime()
        {
            DateTime date = DateTime.Parse(EndTimeDateTextBox.Text);
            int hours = Int32.Parse(EndTimeHourDropDownList.SelectedValue);
            int minutes = Int32.Parse(EndTimeMinuteDropDownList.SelectedValue);

            DateTime result = date.AddHours(hours).AddMinutes(minutes);

            return result;
        }
    }
}