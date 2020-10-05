using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeaveSample.Entities;

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
    /// LeaveStatusLogs data access component. Manages CRUD operations for the LeaveStatusLogs table.
    /// </summary>
    public partial class LeaveStatusLogDAC : DataAccessComponent
    {
        /// <summary>
        /// Inserts a new row in the LeaveStatusLogs table.
        /// </summary>
        /// <param name="leaveStatusLog">A LeaveStatusLog object.</param>
        /// <returns>An updated LeaveStatusLog object.</returns>
        public LeaveStatusLog Create(LeaveStatusLog leaveStatusLog)
        {
            using (var db = new DbContext(CONNECTION_NAME))
            {
                db.Set<LeaveStatusLog>().Add(leaveStatusLog);
                db.SaveChanges();

                return leaveStatusLog;
            }
        }

        /// <summary>
        /// Conditionally retrieves one or more rows from the LeaveStatusLogs table.
        /// </summary>
        /// <param name="leaveID">A leaveID value.</param>
        /// <returns>A collection of LeaveStatusLog objects.</returns>	
        public List<LeaveStatusLog> SelectByLeave(long leaveID)
        {
            using (var db = new DbContext(CONNECTION_NAME))
            {
                // Store the query.
                IQueryable<LeaveStatusLog> query = db.Set<LeaveStatusLog>();

                // Filter by LeaveID
                query = query.Where(l => l.LeaveID == leaveID)
                    .OrderByDescending(l => l.Date);

                // Return result.
                return query.ToList();
            }
        }
    }
}
