using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LeaveSample.Entities;
using LeaveSample.UI.Process;
using LeaveSample.Framework;
using System.Diagnostics;

namespace LeaveSample.UI.Windows
{
    public partial class Main : Form
    {
        private List<Leave> _leaves;
        private bool _isLoading;

        private string _sortColumn;
        private string _apSortColumn;

        public Main()
        {
            InitializeComponent();
            LoadLeaves();
            InitializeApply();
            InitializeApprovals();
        }

        private void mainTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mainTab.SelectedIndex == 0)
                LoadLeaveRecords();
         
            else if (mainTab.SelectedIndex == 1)
                LoadApprovals();
        }

        private void LoadEnumFilter(ComboBox comboBox, Type type)
        {
            var list = Enum.GetNames(type).ToList();
            list.Insert(0, "- All -");

            comboBox.DataSource = list;
        }

        private void BindGrid(DataGridView grid, List<Leave> leaves)
        {
            Leave selected = null;

            // Get previous selected row.
            if (grid.SelectedRows.Count > 0)
                selected = grid.SelectedRows[0].DataBoundItem as Leave;

            var source = new BindingSource();
            source.DataSource = leaves;
            grid.DataSource = source;

            // Re-select previous row.
            if (selected != null)
            {
                int i = leaves.FindIndex(l => l.LeaveID == selected.LeaveID);
                if (i >= 0)
                    grid.Rows[i].Selected = true;
            }
        }

        private void LoadLeaves()
        {
            // NOTE: Since this is WinForms, we can actually keep a cache of the 
            // Leave records in memory and do not need to keep querying the back-end.

            int totalRowCount = 0;
            var upc = new LeaveController();

            // For simplicity, we are only querying back 1000 records and paging is 
            // not implemented because it is not necessary in Windows Forms.
            _leaves = upc.ListLeavesByEmployee(1000, 0, null,
                null, null, null, out totalRowCount);
        }

        #region " Apply Tab Methods "
        private void InitializeApply()
        {
            employeeLabel.Text = Environment.UserName;
            categoryBox.DataSource = LeaveController.GetCategories();
            durationBox.Value = 1;

            startDateBox.MinDate = DateTime.Today.AddDays(-7);
            endDateBox.MinDate = startDateBox.Value;

            _sortColumn = "DateSubmitted DESC";

            // Load all the filters with enum values.
            _isLoading = true;
            LoadEnumFilter(categoryFilter, typeof(LeaveCategories));
            LoadEnumFilter(statusFilter, typeof(LeaveStatuses));
            _isLoading = false;

            leaveRecordsGrid.AutoGenerateColumns = false;

            // Default to sort DateSubmitted descending.
            _sortColumn = submitDateColumn.DataPropertyName + " DESC";

            LoadLeaveRecords();
        }

        private void LoadLeaveRecords()
        {
            // NOTE: Since this is WinForms, we can actually keep a cache of the 
            // Leave records in memory and do not need to keep querying the back-end.

            LeaveCategories? category = LeaveController.GetEnumValue<LeaveCategories>(categoryFilter.Text);
            LeaveStatuses? status = LeaveController.GetEnumValue<LeaveStatuses>(statusFilter.Text);

            BindGrid(leaveRecordsGrid, _leaves);
            RefreshGrid(leaveRecordsGrid, _leaves, _sortColumn, categoryFilter.Text, 
                statusFilter.Text, Environment.UserName);

        }

        private void RefreshGrid(DataGridView grid, List<Leave> leaves, string sort, 
            string categoryFilter, string statusFilter, string employee = null)
        {
            if (leaves == null) return;
            // Perform all filtering and sorting in memory only. Do not need to call back-end.

            LeaveCategories? category = LeaveController.GetEnumValue<LeaveCategories>(categoryFilter);
            LeaveStatuses? status = LeaveController.GetEnumValue<LeaveStatuses>(statusFilter);

            var query = leaves.AsQueryable();

            // Filter by Category.
            if (category != null)
                query = query.Where(l => l.Category == category);

            // Filter by Status.
            if (status != null)
                query = query.Where(l => l.Status == status);

            if (!string.IsNullOrWhiteSpace(employee))
                query = query.Where(l => l.Employee == employee);

            // Set sorting column.
            query = query.OrderBy(sort);

            BindGrid(grid, query.ToList());
        }


        private void applyButton_Click(object sender, EventArgs e)
        {
            var upc = new LeaveController();

            // Create new Leave.
            var leave = new Leave()
            {
                Employee = Environment.UserName,
                Category = (LeaveCategories)categoryBox.SelectedIndex,
                StartDate = startDateBox.Value.Date,
                EndDate = endDateBox.Value.Date,
                Duration = Convert.ToByte(durationBox.Value),
                Description = descriptionBox.Text
            };

            try
            {
                leave = upc.Apply(leave);

                // Add new leave to list of records.
                _leaves.Add(leave);

                RefreshGrid(leaveRecordsGrid, _leaves, _sortColumn, categoryFilter.Text,
                    statusFilter.Text, Environment.UserName);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(this, ex.Message, "Error Applying New Leave", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "An Unexpected Error Occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void dateBox_ValueChanged(object sender, EventArgs e)
        {
            int days = endDateBox.Value.Subtract(startDateBox.Value).Days + 1;
            endDateBox.MinDate = startDateBox.Value;
            durationBox.Maximum = days;
            durationBox.Value = days;
        }

        private void filter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_isLoading)
                RefreshGrid(leaveRecordsGrid, _leaves, _sortColumn, categoryFilter.Text,
                    statusFilter.Text, Environment.UserName);
        }

        private void reloadButton_Click(object sender, EventArgs e)
        {
            LoadLeaves();
            LoadLeaveRecords();
        }

        private void leaveRecordsGrid_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            // Disables the cancel button if leave is not pending.
            cancelButton.Enabled = (leaveRecordsGrid.Rows[e.RowIndex].Cells["statusColumn"].Value.ToString() == "Pending");
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            // Exit when no row is selected.
            if (leaveRecordsGrid.SelectedRows.Count == 0) return;

            var upc = new LeaveController();

            try
            {
                // Get the Leave object.
                Leave leave = leaveRecordsGrid.SelectedRows[0].DataBoundItem as Leave;

                // Get the Leave index position in the list.
                int i = _leaves.IndexOf(leave);

                leave = upc.Cancel(leave);

                // Update the Leave in the list.
                _leaves[i] = leave;

                RefreshGrid(leaveRecordsGrid, _leaves, _sortColumn, categoryFilter.Text,
                    statusFilter.Text, Environment.UserName);
               
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(this, ex.Message, "Error Applying New Leave", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "An Unexpected Error Occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void leaveRecordsGrid_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string sortColumn = leaveRecordsGrid.Columns[e.ColumnIndex].DataPropertyName;

            // Determine sort order.
            if (sortColumn == _sortColumn)
                _sortColumn += " DESC";
            else
                _sortColumn = sortColumn;

            RefreshGrid(leaveRecordsGrid, _leaves, _sortColumn, categoryFilter.Text,
                statusFilter.Text, Environment.UserName);

        }
        #endregion

        #region " Approvals Tab Methods "
        private void InitializeApprovals()
        {
            _apSortColumn = "DateSubmitted DESC, Status";

            // Load all the filters with enum values.
            _isLoading = true;
            LoadEnumFilter(apCategoryFilter, typeof(LeaveCategories));
            LoadEnumFilter(apStatusFilter, typeof(LeaveStatuses));
            _isLoading = false;
            approvalsGrid.AutoGenerateColumns = false;

            // Default to sort DateSubmitted descending.
            _apSortColumn = apSubmitDateColumn.DataPropertyName + " DESC";

        }

        private void LoadApprovals()
        {
            // NOTE: Since this is WinForms, we can actually keep a cache of the 
            // Leave records in memory and do not need to keep querying the back-end.
            BindGrid(approvalsGrid, _leaves);
            RefreshGrid(approvalsGrid, _leaves, _apSortColumn, apCategoryFilter.Text, apStatusFilter.Text);
        }

        private void apfilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_isLoading)
                RefreshGrid(approvalsGrid, _leaves, _apSortColumn,
                    apCategoryFilter.Text, apStatusFilter.Text);
        }

        private void approvalsGrid_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string sortColumn = approvalsGrid.Columns[e.ColumnIndex].DataPropertyName;

            // Determine sort order.
            if (sortColumn == _apSortColumn)
                _apSortColumn += " DESC";
            else
                _apSortColumn = sortColumn;

            RefreshGrid(approvalsGrid, _leaves, _apSortColumn,
               apCategoryFilter.Text, apStatusFilter.Text);
        }

        private void approvalsGrid_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            // Disables the cancel button if leave is not pending.
            approveButton.Enabled = (approvalsGrid.Rows[e.RowIndex].Cells["apStatusColumn"].Value.ToString() == "Pending");
            rejectButton.Enabled = approveButton.Enabled;
        }

        private void approvalsButton_Click(object sender, EventArgs e)
        {
            // Exit when no row is selected.
            if (approvalsGrid.SelectedRows.Count == 0) return;

            var upc = new LeaveController();

            try
            {
                // Get the Leave object.
                Leave leave = approvalsGrid.SelectedRows[0].DataBoundItem as Leave;

                // Get the Leave index position in the list.
                int i = _leaves.IndexOf(leave);

                if (sender == approveButton)
                    leave = upc.Approve(leave);
                else
                    leave = upc.Reject(leave);

                // Update the Leave in the approval list.
                _leaves[i] = leave;
                RefreshGrid(approvalsGrid, _leaves, _apSortColumn, apCategoryFilter.Text, apStatusFilter.Text);
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "An Unexpected Error Occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void approvalsGrid_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (approvalsGrid.Columns[e.ColumnIndex] == apRemarksColumn)
            {
                var leave = approvalsGrid.Rows[e.RowIndex].DataBoundItem as Leave;

                // Disable edit on remarks when Leave is not pending.
                e.Cancel = (leave.Status != LeaveStatuses.Pending);
            }
        }

        private void apReloadButton_Click(object sender, EventArgs e)
        {
            LoadLeaves();
            LoadApprovals();
        }

        #endregion

        #region " Footer "
        private void link1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://layersample.codeplex.com");
        }

        private void link2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://visualstudiogallery.msdn.microsoft.com/caa8f808-d4a5-4f48-a88e-da572834f37e");
        }

        private void link3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://serena-yeoh.blogspot.com");
        }
        #endregion
    }
}
