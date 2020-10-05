using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace LeaveSample.UI.Windows.Behaviors
{
    /// <summary>
    /// Behavior to get the Sort Column from a DataGrid.
    /// </summary>
    public class DataGridSortColumnBehavior : Behavior<DataGrid>
    {
        public static readonly DependencyProperty SortColumnProperty =
            DependencyProperty.Register("SortColumn", typeof(string), typeof(DataGridSortColumnBehavior),
                                    new FrameworkPropertyMetadata(string.Empty));

        public string SortColumn
        {
            get { return (string)base.GetValue(SortColumnProperty); }
            set { base.SetValue(SortColumnProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.Sorting += AssociatedObject_Sorting;
            base.OnAttached();
        }

        void AssociatedObject_Sorting(object sender, DataGridSortingEventArgs e)
        {
            var grid = sender as DataGrid;

            // Store the sort column.
            if (grid != null)
            {
                if (e.Column.SortDirection != null && e.Column.SortMemberPath == SortColumn)
                    SortColumn += " DESC";
                else
                    SortColumn = e.Column.SortMemberPath;
            }
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Sorting -= AssociatedObject_Sorting;
            base.OnDetaching();
        }
    }
}
