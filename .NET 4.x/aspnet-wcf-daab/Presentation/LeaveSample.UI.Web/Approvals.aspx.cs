using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.ModelBinding;
using System.Web.UI.WebControls;
using LeaveSample.Entities;
using LeaveSample.UI.Process;

namespace LeaveSample.UI.Web
{
    public partial class Approvals : System.Web.UI.Page
    {
        private bool _isApproved = false;

        public List<ListItem> category_GetData()
        {
            return UIHelper.LoadEnumFilters(typeof(LeaveCategories));
        }

        public List<ListItem> status_GetData()
        {
            return UIHelper.LoadEnumFilters(typeof(LeaveStatuses));;
        }

        public List<Leave> leaveGrid_GetData(
                   [Control]LeaveCategories? category,
                   [Control]LeaveStatuses? status,
                   int maximumRows,
                   int startRowIndex, out int totalRowCount, string sortByExpression
                   )
        {
            if (!Page.IsPostBack && status == null)
            {
                status = LeaveStatuses.Pending;
            }

            var upc = new LeaveController();

            return upc.ListLeavesByEmployee(maximumRows, startRowIndex, sortByExpression,
                null, category, status, out totalRowCount);
        }

        protected void leaveGrid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            _isApproved = (e.CommandArgument.ToString() == "Approve");
        }

        // The id parameter name should match the DataKeyNames value set on the control
        public void leaveGrid_UpdateItem(int leaveID)
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
                if (_isApproved)
                    upc.Approve(leave);
                else
                    upc.Reject(leave);
            }
        }

        protected void status_DataBound(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                status.SelectedIndex = 1;
            }
        }
    }
}