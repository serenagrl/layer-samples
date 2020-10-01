using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeaveSample.Entities;
using LeaveSample.Framework;

// NOTE:
//
// Data access components are responsible for querying and persisting data for
// the application. All database related processing should be done here.
//
// All the CRUD activities are done here in this component to isolate them from 
// higher level components in the layer. This should allow upgrades or change of
// data access technologies as required.
//
// It is not necessary that each Data Access Component to be mapped directly to an
// individual table. In larger systems, one DAC may manage the CRUD activities for
// one or more tables/Entities.

namespace LeaveSample.Data
{
    /// <summary>
    /// Leaves data access component. Manages CRUD operations for the Leaves table.
    /// </summary>
    public partial class LeaveDAC : DataAccessComponent
    {
        /// <summary>
        /// Inserts a new row in the Leaves table.
        /// </summary>
        /// <param name="leave">A Leave object.</param>
        /// <returns>An updated Leave object.</returns>
        public Leave Create(Leave leave)
        {
            using (var db = new DbContext(CONNECTION_NAME))
            {
                db.Set<Leave>().Add(leave);
                db.SaveChanges();

                return leave;
            }
        }

        /// <summary>
        /// Updates an existing row in the Leaves table.
        /// </summary>
        /// <param name="leave">A Leave entity object.</param>
        public void UpdateStatus(Leave leave)
        {
            using (var db = new DbContext(CONNECTION_NAME))
            {
                var entry = db.Entry<Leave>(leave);

                // Re-attach the entity.
                entry.State = EntityState.Unchanged;

                // Only allow relevant fields to be updated.
                entry.Property("Status").IsModified = true;
                entry.Property("Remarks").IsModified = true;
                entry.Property("IsCompleted").IsModified = true;

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Returns a row from the Leaves table.
        /// </summary>
        /// <param name="leaveID">A LeaveID value.</param>
        /// <returns>A Leave object with data populated from the database.</returns>
        public Leave SelectById(long leaveID)
        {
            using (var db = new DbContext(CONNECTION_NAME))
            {
                return db.Set<Leave>().Find(leaveID);
            }
        }

        /// <summary>
        /// Conditionally retrieves one or more rows from the Leaves table with paging and a sort expression.
        /// </summary>
        /// <param name="maximumRows">The maximum number of rows to return.</param>
        /// <param name="startRowIndex">The starting row index.</param>
        /// <param name="sortExpression">The sort expression.</param>
        /// <param name="employee">A employee value.</param>
        /// <param name="category">A category value.</param>
        /// <param name="status">A status value.</param>
        /// <returns>A collection of Leave objects.</returns>		
        public List<Leave> Select(int maximumRows, int startRowIndex, string sortExpression,
            string employee, LeaveCategories? category, LeaveStatuses? status)
        {
            using (var db = new DbContext(CONNECTION_NAME))
            {
                // Store the query.
                IQueryable<Leave> query = db.Set<Leave>();

                // Append filters.
                query = AppendFilters(query, employee, category, status);

                // Sort and page.
                query = query.OrderBy(sortExpression)
                        .Skip(startRowIndex).Take(maximumRows);

                // Return result.
                return query.ToList();
            }
        }

        /// <summary>
        /// Returns a count based on the condition.
        /// </summary>
        /// <param name="employee">A employee value.</param>
        /// <param name="category">A category value.</param>
        /// <param name="status">A status value.</param>
        /// <returns>The record count.</returns>		
        public int Count(string employee, LeaveCategories? category, 
            LeaveStatuses? status)
        {
            using (var db = new DbContext(CONNECTION_NAME))
            {
                // Store the query.
                IQueryable<Leave> query = db.Set<Leave>();

                // Append filters.
                query = AppendFilters(query, employee, category, status);
              
                // Return result.
                return query.Count();
            }
        }

        /// <summary>
        /// Conditionally appends filters to the query.
        /// </summary>
        /// <param name="query">The query object.</param>
        /// <param name="employee">The employee to filter by.</param>
        /// <param name="category">The category to filter by.</param>
        /// <param name="status">The status to filter by.</param>
        /// <returns>A query object.</returns>
        private static IQueryable<Leave> AppendFilters(IQueryable<Leave> query,
            string employee, LeaveCategories? category, LeaveStatuses? status)
        {
            // Filter employee.
            if (!string.IsNullOrWhiteSpace(employee))
                query = query.Where(l => l.Employee == employee);

            // Filter category.
            if (category != null)
                query = query.Where(l => l.Category == category);

            // Filter status.
            if (status != null)
                query = query.Where(l => l.Status == status);
            return query;
        }

        /// <summary>
        /// Indicates whether there are any overlapping dates from other leaves.
        /// </summary>
        /// <param name="leave">A Leave object.</param>
        /// <returns>Returns true if there is an overlap, false if there is not.</returns>
        public bool IsOverlap(Leave leave)
        {
            using (var db = new DbContext(CONNECTION_NAME))
            {
                // Store the query.
                IQueryable<Leave> query = db.Set<Leave>();

                // Check for overlapping dates.
                query = query.Where(l => l.Employee == leave.Employee &&
                    l.StartDate <= leave.EndDate && 
                    l.EndDate >= leave.StartDate &&
                    (l.Status == LeaveStatuses.Pending ||
                    l.Status == LeaveStatuses.Approved));

                // Return result.
                return query.Any();
            }
        }
    }


}
