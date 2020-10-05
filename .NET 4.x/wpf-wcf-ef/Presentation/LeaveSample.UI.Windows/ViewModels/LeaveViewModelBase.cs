using LeaveSample.Entities;
using LeaveSample.Framework;
using LeaveSample.UI.Process;
using LeaveSample.UI.Windows.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace LeaveSample.UI.Windows.ViewModels
{
    /// <summary>
    /// Base class for all Leave ViewModels.
    /// </summary>
    public abstract class LeaveViewModelBase : ViewModelBase
    {
        // Shared cache copy for improving back-end performance.
        protected static ObservableCollection<Leave> _cacheLeaves;
        
        private Object lockObj = new Object();
        protected LeaveController _upc;

        private ICommand _filterCommand;
        private ICommand _reloadCommand;
        
        private bool _isActivating;
        private bool _canProcess;
        private Leave _selectedLeave;
        private ObservableCollection<Leave> _leaves;

        private List<string> _categories;
        private List<string> _statuses;

        /// <summary>
        /// Gets or sets the selected category to filter.
        /// </summary>
        public string CategoryFilter { get; set; }

        /// <summary>
        /// Gets or sets the selected status to filter.
        /// </summary>
        public string StatusFilter { get; set; }

        /// <summary>
        /// Gets or sets the sort expression to use when filtering.
        /// </summary>
        public string SortExpression { get; set; }

        /// <summary>
        /// Indicates whether the View is activating.
        /// </summary>
        public bool IsActivating
        {
            get { return _isActivating; }
            set
            {
                _isActivating = value;

                if (_isActivating)
                    FilterLeaves();
            }
        }

        /// <summary>
        /// Command to Reload cache and reapply filters.
        /// </summary>
        public ICommand ReloadCommand
        {
            get { return _reloadCommand; }
        }

        /// <summary>
        /// Command to Filter leaves.
        /// </summary>
        public ICommand FilterCommand
        {
            get { return _filterCommand; }
        }

        /// <summary>
        /// Indicates whether the selected Leave can be processed.
        /// </summary>
        public bool CanProcess
        {
            get { return _canProcess; }
            protected set 
            {
                _canProcess = value;
                NotifyPropertyChanged("CanProcess");
            }
        }

        /// <summary>
        /// Gets or sets the selected Leave.
        /// </summary>
        public Leave SelectedLeave
        {
            get { return _selectedLeave; }
            set
            {
                _selectedLeave = value;

                // Determine whether it can be processed.
                this.CanProcess = _selectedLeave == null ? false : _selectedLeave.Status == LeaveStatuses.Pending;
            }
        }

        /// <summary>
        /// Gets or sets the Leaves collection that is to be displayed.
        /// </summary>
        public ObservableCollection<Leave> Leaves
        {
            get { return _leaves; }
            set
            {
                _leaves = value;
                NotifyPropertyChanged("Leaves");
            }
        }

        /// <summary>
        /// Returns a list of Leave categories.
        /// </summary>
        public List<string> Categories
        {
            get { return _categories; }
        }

        /// <summary>
        /// Returns a list of Leave statuses.
        /// </summary>
        public List<string> Statuses
        {
            get { return _statuses; }
        }

        /// <summary>
        /// Initializes the class.
        /// </summary>
        public LeaveViewModelBase()
        {
            _upc = new LeaveController();

            // Load enums values that can be used by filter controls.
            _categories = LoadEnumFilter(typeof(LeaveCategories));
            _statuses = LoadEnumFilter(typeof(LeaveStatuses));

            // Set default selection.
            this.CategoryFilter = "- All -";
            this.StatusFilter = "- All -";

            // Initialize commands.
            _filterCommand = new RelayCommand(p => this.FilterLeaves(), null);
            _reloadCommand = new RelayCommand(p => this.ReloadLeaves(), null);

            // Load the cache.
            this.SortExpression = "DateSubmitted DESC";
            LoadLeaves();
        }

        /// <summary>
        /// Returns an array of Enum names that can be used for dropdownlists or comboboxes.
        /// </summary>
        /// <param name="type">The Enum type.</param>
        /// <returns>A string array of Enum names.</returns>
        private List<string> LoadEnumFilter(Type type)
        {
            var items = Enum.GetNames(type).ToList();
            items.Insert(0, "- All -");
            return items;
        }

        /// <summary>
        /// Loads a cache copy of the Leaves from back-end.
        /// </summary>
        protected void LoadLeaves()
        {
            int totalRowCount = 0;

            lock (lockObj)
            {
                _cacheLeaves = _upc.ListLeavesByEmployee(1000, 0, null, null, null, null, out totalRowCount);
            }
        }

        /// <summary>
        /// Reloads the cache-copy from back-end and apply filters to it.
        /// </summary>
        protected void ReloadLeaves()
        {
            LoadLeaves();
            FilterLeaves();
        }

        /// <summary>
        /// Filters the Leaves.
        /// </summary>
        protected abstract void FilterLeaves();

        /// <summary>
        /// Returns a filtered list of Leaves.
        /// </summary>
        /// <param name="sort">The sort expression.</param>
        /// <param name="categoryFilter">The category to filter by.</param>
        /// <param name="statusFilter">The status to filter by.</param>
        /// <param name="employee">The employee to filter by.</param>
        /// <returns></returns>
        protected static ObservableCollection<Leave> FilterLeaves(string sort, string categoryFilter, string statusFilter, string employee = null)
        {
            LeaveCategories? category = null;
            LeaveStatuses? status = null;

            if (!string.IsNullOrWhiteSpace(categoryFilter) && categoryFilter != "- All -")
                category = (LeaveCategories?)Enum.Parse(typeof(LeaveCategories), categoryFilter);

            if (!string.IsNullOrWhiteSpace(statusFilter) && statusFilter != "- All -")
                status = (LeaveStatuses?)Enum.Parse(typeof(LeaveStatuses), statusFilter);

            var query = _cacheLeaves.AsQueryable();

            // Filter by Category.
            if (category != null)
                query = query.Where(l => l.Category == category);

            // Filter by Status.
            if (status != null)
                query = query.Where(l => l.Status == status);

            // Fiilter by Employee.
            if (!string.IsNullOrWhiteSpace(employee))
                query = query.Where(l => l.Employee == employee);

            // Set sorting column.
            query = query.OrderBy(sort);

            return new ObservableCollection<Leave>(query.ToList());
        }
    }
}
