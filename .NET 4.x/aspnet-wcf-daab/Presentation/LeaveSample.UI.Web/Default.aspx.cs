using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;
using System.Web.UI;
using System.Web.UI.WebControls;
using LeaveSample.Entities;
using LeaveSample.UI.Process;

namespace LeaveSample.UI.Web
{
    public partial class Default : System.Web.UI.Page
    {
        /// <summary>
        /// Inserts a new leave.
        /// </summary>
        public void leaveForm_InsertItem()
        {
            var leave = new Leave();
            TryUpdateModel(leave);

            if (ModelState.IsValid)
            {
                var upc = new LeaveController();

                try
                {
                    upc.Apply(leave);
                }
                catch(ApplicationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return;
                }

                leaveGrid.DataBind();
            }
        }

        public string[] GetCategories()
        {
            return LeaveController.GetCategories();
        }

        public List<ListItem> category_GetData()
        {
            return UIHelper.LoadEnumFilters(typeof(LeaveCategories));
        }

        public List<ListItem> status_GetData()
        {
            return UIHelper.LoadEnumFilters(typeof(LeaveStatuses));
        }

        public List<Leave> leaveGrid_GetData(
            [Control]LeaveCategories? category,
            [Control]LeaveStatuses? status,
            int maximumRows,
            int startRowIndex, out int totalRowCount, string sortByExpression
            )
        {
            var upc = new LeaveController();

            return upc.ListLeavesByEmployee(maximumRows, startRowIndex, sortByExpression,
                Environment.UserName, category, status, out totalRowCount);
        }
      
        public void leaveGrid_UpdateItem(long leaveID)
        {
            var upc = new LeaveController();
            var leave = upc.GetLeaveById(leaveID);

            if (leave == null)
            {
                // The item wasn't found
                ModelState.AddModelError("", String.Format("Item with id {0} was not found", leaveID));
                return;
            }

            TryUpdateModel(leave);
            if (ModelState.IsValid)
            {
                // Save changes here, e.g. MyDataLayer.SaveChanges();
                upc.Cancel(leave);
            }
        }

        protected void leaveForm_DataBound(object sender, EventArgs e)
        {
            // Set default values.
            InitializeForm();
        }

        private void InitializeForm()
        {
            // Set Employee to current login user.
            Label employeeLabel = leaveForm.FindControl("employeeLabel") as Label;
            employeeLabel.Text = Environment.UserName;

            var radioList = leaveForm.FindControl("categoryRadio") as RadioButtonList;
            radioList.SelectedIndex = 0;

            // Default to today.
            var startDate = leaveForm.FindControl("startDateBox") as TextBox;
            startDate.Text = DateTime.Now.ToShortDateString();

            // Default to today.
            var endDate = leaveForm.FindControl("endDateBox") as TextBox;
            endDate.Text = DateTime.Now.ToShortDateString();

            var duration = leaveForm.FindControl("durationBox") as TextBox;
            duration.Text = "1";
        }
        
    }
}