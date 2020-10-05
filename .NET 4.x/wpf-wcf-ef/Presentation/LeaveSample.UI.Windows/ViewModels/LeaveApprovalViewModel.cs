using LeaveSample.Entities;
using LeaveSample.UI.Process;
using LeaveSample.UI.Windows.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LeaveSample.UI.Windows.ViewModels
{
    /// <summary>
    /// ViewModel for Approving Leaves.
    /// </summary>
    public class LeaveApprovalViewModel : LeaveViewModelBase
    {
        #region " Commands "
        private ICommand _approveCommand;
        private ICommand _rejectCommand;

        public ICommand ApproveCommand
        {
            get { return _approveCommand; }
        }

        public ICommand RejectCommand
        {
            get { return _rejectCommand; }
        }
        #endregion

        /// <summary>
        /// Initializes the ViewModel.
        /// </summary>
        public LeaveApprovalViewModel()
        {
            // Initialize commands.
            _approveCommand = new RelayCommand(p => this.ApproveLeave(), null);
            _rejectCommand = new RelayCommand(p => this.RejectLeave(), null);

            FilterLeaves();

        }

        /// <summary>
        /// Filters the Leaves.
        /// </summary>
        protected override void FilterLeaves()
        {
            this.Leaves = FilterLeaves(this.SortExpression, this.CategoryFilter, this.StatusFilter);
        }

        /// <summary>
        /// Approve a Leave.
        /// </summary>
        private void ApproveLeave()
        {
            if (this.SelectedLeave == null || this.SelectedLeave.Status != LeaveStatuses.Pending)
                return;

            try
            {
                int i = _cacheLeaves.IndexOf(this.SelectedLeave);

                var leave = _upc.Approve(this.SelectedLeave);

                _cacheLeaves[i] = leave;
                FilterLeaves();
                this.SelectedLeave = null;

            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Error Approving Leave", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "An Unexpected Error Occurred", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Rejects a Leave.
        /// </summary>
        private void RejectLeave()
        {
            if (this.SelectedLeave == null || this.SelectedLeave.Status != LeaveStatuses.Pending)
                return;

            try
            {
                int i = _cacheLeaves.IndexOf(this.SelectedLeave);

                var leave = _upc.Reject(this.SelectedLeave);

                _cacheLeaves[i] = leave;
                FilterLeaves();
                this.SelectedLeave = null;

            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Error Rejecting Leave", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "An Unexpected Error Occurred", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
