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
using System.Windows.Controls;
using System.Windows.Input;

namespace LeaveSample.UI.Windows.ViewModels
{
    /// <summary>
    /// ViewModel for applying leaves.
    /// </summary>
    public class ApplyLeaveViewModel : LeaveViewModelBase
    {
        private List<string> _inputCategories;
        private Leave _leave;
        private int _maximumDuration;

        /// <summary>
        /// Stores the new Leave.
        /// </summary>
        public Leave Leave
        {
            get{ return _leave; }
            set
            {
                _leave = value;
                NotifyPropertyChanged("Leave");
            }
        }

        /// <summary>
        /// Provides a list of Leave Categories.
        /// </summary>
        public List<string> InputCategories
        {
            get { return _inputCategories; }
        }

        /// <summary>
        /// Gets the maximum acceptable duration.
        /// </summary>
        public int MaximumDuration
        {
            get { return _maximumDuration; }
            private set
            {
                _maximumDuration = value;
                NotifyPropertyChanged("MaximumDuration");
            }
        }

        #region " Commands "
        private ICommand _applyCommand;
        private ICommand _cancelCommand;
        private ICommand _onDateChangedCommand;

        /// <summary>
        /// Command to Apply Leave.
        /// </summary>
        public ICommand ApplyCommand
        {
            get { return _applyCommand; }
        }

        /// <summary>
        /// Command to Cancel a Leave.
        /// </summary>
        public ICommand CancelCommand
        {
            get { return _cancelCommand; }
        }

        /// <summary>
        /// Command to recompute date related changes and duration.
        /// </summary>
        public ICommand OnDateChangedCommand
        {
            get { return _onDateChangedCommand; }
        }

        #endregion

        /// <summary>
        /// Initialized the ViewModel.
        /// </summary>
        public ApplyLeaveViewModel()
        {
            InitializeNewLeave();

            _applyCommand = new RelayCommand(p => this.ApplyLeave(), null);
            _cancelCommand = new RelayCommand(p => this.CancelLeave(), null);
            _onDateChangedCommand = new RelayCommand(p => this.CalculateDuration(), null);

            LoadInputCategories();
            FilterLeaves();
        }

        /// <summary>
        /// Initializes a new Leave for data entry;
        /// </summary>
        private void InitializeNewLeave()
        {
            this.Leave = new Leave()
            {
                Employee = Environment.UserName,
                Category = LeaveCategories.Annual,
                StartDate = DateTime.Now.Date,
                EndDate = DateTime.Now.Date,
                Duration = 1
            };
        }

        /// <summary>
        /// Loads the available Leave Categories.
        /// </summary>
        private void LoadInputCategories()
        {
            _inputCategories = Enum.GetNames(typeof(LeaveCategories)).ToList();
        }

        /// <summary>
        /// Calculate the duration.
        /// </summary>
        private void CalculateDuration()
        {
            if (_leave == null) return;

            if (_leave.EndDate < _leave.StartDate)
                _leave.EndDate = _leave.StartDate;

            int days = _leave.EndDate.Subtract(_leave.StartDate).Days + 1;

            this.MaximumDuration = days;
            _leave.Duration = Convert.ToByte(days);

            // Have to call this since Leave does not implement INotifyPropertyChanged.
            NotifyPropertyChanged("Leave");
        }

        /// <summary>
        /// Filter the leaves.
        /// </summary>
        protected override void FilterLeaves()
        {
            this.Leaves = FilterLeaves(this.SortExpression, this.CategoryFilter, this.StatusFilter, Environment.UserName);
        }

        /// <summary>
        /// Apply new Leave.
        /// </summary>
        private void ApplyLeave()
        {
            try
            {
                var leave = _upc.Apply(this.Leave);

                _cacheLeaves.Insert(0, leave);
                FilterLeaves();
                InitializeNewLeave();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Error Applying New Leave", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "An Unexpected Error Occurred", MessageBoxButton.OK, MessageBoxImage.Error);
            }   
        }

        /// <summary>
        /// Cancel a Leave.
        /// </summary>
        private void CancelLeave()
        {
            if (this.SelectedLeave == null || this.SelectedLeave.Status != LeaveStatuses.Pending)
                return;

            try
            {
                int i = _cacheLeaves.IndexOf(this.SelectedLeave);

                var leave = _upc.Cancel(this.SelectedLeave);

                _cacheLeaves[i] = leave;
                FilterLeaves();
                this.SelectedLeave = null;
            }
            catch(ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "Error Cancelling Leave", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "An Unexpected Error Occurred", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
    }
}
