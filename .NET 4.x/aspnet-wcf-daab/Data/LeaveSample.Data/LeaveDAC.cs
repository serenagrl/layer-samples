//====================================================================================================
// Base code generated with Momentum: DAC Gen (Build 2.5.4877.28464)
// Layered Architecture Solution Guidance (http://layerguidance.codeplex.com)
//
// Generated by Serena Yeoh at ALIENWARE on 05/22/2013 10:32:03 
//====================================================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
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
            const string SQL_STATEMENT =
                "INSERT INTO dbo.Leaves ([CorrelationID], [Category], [Employee], [StartDate], [EndDate], [Description], [Duration], [Status], [IsCompleted], [Remarks], [DateSubmitted]) " +
                "VALUES(@CorrelationID, @Category, @Employee, @StartDate, @EndDate, @Description, @Duration, @Status, @IsCompleted, @Remarks, @DateSubmitted); SELECT SCOPE_IDENTITY();";

            // Connect to database.
            Database db = DatabaseFactory.CreateDatabase(CONNECTION_NAME);
            using (DbCommand cmd = db.GetSqlStringCommand(SQL_STATEMENT))
            {
                // Set parameter values.
                db.AddInParameter(cmd, "@CorrelationID", DbType.Guid, leave.CorrelationID);
                db.AddInParameter(cmd, "@Category", DbType.Byte, leave.Category);
                db.AddInParameter(cmd, "@Employee", DbType.AnsiString, leave.Employee);
                db.AddInParameter(cmd, "@StartDate", DbType.DateTime, leave.StartDate);
                db.AddInParameter(cmd, "@EndDate", DbType.DateTime, leave.EndDate);
                db.AddInParameter(cmd, "@Description", DbType.AnsiString, leave.Description);
                db.AddInParameter(cmd, "@Duration", DbType.Byte, leave.Duration);
                db.AddInParameter(cmd, "@Status", DbType.Byte, leave.Status);
                db.AddInParameter(cmd, "@IsCompleted", DbType.Boolean, leave.IsCompleted);
                db.AddInParameter(cmd, "@Remarks", DbType.AnsiString, leave.Remarks);
                db.AddInParameter(cmd, "@DateSubmitted", DbType.DateTime, leave.DateSubmitted);

                // Get the primary key value.
                leave.LeaveID = Convert.ToInt64(db.ExecuteScalar(cmd));
            }

            return leave;
        }

        /// <summary>
        /// Updates an existing row in the Leaves table.
        /// </summary>
        /// <param name="leave">A Leave entity object.</param>
        public void UpdateStatus(Leave leave)
        {
            const string SQL_STATEMENT =
                "UPDATE dbo.Leaves " +
                "SET " +
                    "[Status]=@Status, " +
                    "[IsCompleted]=@IsCompleted, " +
                    "[Remarks]=@Remarks " +
                "WHERE [LeaveID]=@LeaveID ";

            // Connect to database.
            Database db = DatabaseFactory.CreateDatabase(CONNECTION_NAME);
            using (DbCommand cmd = db.GetSqlStringCommand(SQL_STATEMENT))
            {
                // Set parameter values.
                db.AddInParameter(cmd, "@Status", DbType.Byte, leave.Status);
                db.AddInParameter(cmd, "@IsCompleted", DbType.Boolean, leave.IsCompleted);
                db.AddInParameter(cmd, "@Remarks", DbType.AnsiString, leave.Remarks);
                db.AddInParameter(cmd, "@LeaveID", DbType.Int64, leave.LeaveID);

                db.ExecuteNonQuery(cmd);
            }
        }

        /// <summary>
        /// Returns a row from the Leaves table.
        /// </summary>
        /// <param name="leaveID">A LeaveID value.</param>
        /// <returns>A Leave object with data populated from the database.</returns>
        public Leave SelectById(long leaveID)
        {
            const string SQL_STATEMENT =
                "SELECT [LeaveID], [CorrelationID], [Category], [Employee], [StartDate], [EndDate], [Description]" +
                        ", [Duration], [Status], [IsCompleted], [Remarks], [DateSubmitted] " +
                "FROM dbo.Leaves  " +
                "WHERE [LeaveID]=@LeaveID ";

            Leave leave = null;

            // Connect to database.
            Database db = DatabaseFactory.CreateDatabase(CONNECTION_NAME);
            using (DbCommand cmd = db.GetSqlStringCommand(SQL_STATEMENT))
            {
                db.AddInParameter(cmd, "@LeaveID", DbType.Int64, leaveID);

                using (IDataReader dr = db.ExecuteReader(cmd))
                {
                    if (dr.Read())
                    {
                        // Create a new Leave
                        leave = LoadLeave(dr);
                    }
                }
            }

            return leave;
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
            const string SQL_STATEMENT =
                "WITH SortedLeaves AS " +
                "(SELECT ROW_NUMBER() OVER (ORDER BY {1}) AS RowNumber, " +
                    "[LeaveID], [CorrelationID], [Category], [Employee], [StartDate], [EndDate], [Description]" +
                        ", [Duration], [Status], [IsCompleted], [Remarks], [DateSubmitted] " +
                    "FROM dbo.Leaves " +
                    "{0}" +
                ") SELECT * FROM SortedLeaves " +
                "WHERE RowNumber BETWEEN @StartRowIndex AND @EndRowIndex";

            startRowIndex++;
            long endRowIndex = startRowIndex + maximumRows;

            List<Leave> result = new List<Leave>();

            // Connect to database.
            Database db = DatabaseFactory.CreateDatabase(CONNECTION_NAME);
            using (DbCommand cmd = db.GetSqlStringCommand(SQL_STATEMENT))
            {
                // Append filters.
                string filter = AppendFilters(db, cmd, employee, category, status);

                // Construct final WHERE statement. 
                if (!string.IsNullOrWhiteSpace(filter))
                    filter = "WHERE " + base.FormatFilterStatement(filter);

                cmd.CommandText = string.Format(SQL_STATEMENT, filter, sortExpression);

                // Paging Parameters.
                db.AddInParameter(cmd, "@StartRowIndex", DbType.Int64, startRowIndex);
                db.AddInParameter(cmd, "@EndRowIndex", DbType.Int64, endRowIndex);

                using (IDataReader dr = db.ExecuteReader(cmd))
                {
                    while (dr.Read())
                    {
                        // Create a new Leave
                        Leave leave = LoadLeave(dr);

                        // Add to List.
                        result.Add(leave);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Conditionally appends filters to the query statememt.
        /// </summary>
        /// <param name="db">A Database object.</param>
        /// <param name="cmd">A DbCommand object.</param>
        /// <param name="employee">The employee to filter by.</param>
        /// <param name="category">The category to filter by.</param>
        /// <param name="status">The status to filter by.</param>
        /// <returns>A condition statement.</returns>
        private static string AppendFilters(Database db, DbCommand cmd, 
            string employee, LeaveCategories? category, LeaveStatuses? status)
        {
            string filter = string.Empty;

            // Employee filter. 
            if (!string.IsNullOrWhiteSpace(employee))
            {
                db.AddInParameter(cmd, "@Employee", DbType.AnsiString, employee);
                filter += "AND [Employee]=@Employee ";
            }

            // Category filter. 
            if (category != null)
            {
                db.AddInParameter(cmd, "@Category", DbType.Byte, category);
                filter += "AND [Category]=@Category ";
            }

            // Status filter. 
            if (status != null)
            {
                db.AddInParameter(cmd, "@Status", DbType.Byte, status);
                filter += "AND [Status]=@Status ";
            }
            return filter;
        }

        /// <summary>
        /// Returns a count based on the condition.
        /// </summary>
        /// <param name="employee">A employee value.</param>
        /// <param name="category">A category value.</param>
        /// <param name="status">A status value.</param>
        /// <returns>The record count.</returns>		
        public int Count(string employee, LeaveCategories? category, LeaveStatuses? status)
        {
            const string SQL_STATEMENT =
                "SELECT COUNT(1) " +
                "FROM dbo.Leaves " +
                "{0}";

            int result = 0;

            // Connect to database.
            Database db = DatabaseFactory.CreateDatabase(CONNECTION_NAME);
            using (DbCommand cmd = db.GetSqlStringCommand(SQL_STATEMENT))
            {
                // Append filters.
                string filter = AppendFilters(db, cmd, employee, category, status);

                // Construct final WHERE statement. 
                if (!string.IsNullOrWhiteSpace(filter))
                    filter = "WHERE " + base.FormatFilterStatement(filter);

                cmd.CommandText = string.Format(SQL_STATEMENT, filter); 
                
                result = Convert.ToInt32(db.ExecuteScalar(cmd));
            }

            return result;
        }

        /// <summary>
        /// Indicates whether there are any overlapping dates from other leaves.
        /// </summary>
        /// <param name="leave">A Leave object.</param>
        /// <returns>Returns true if there is an overlap, false if there is not.</returns>
        public bool IsOverlap(Leave leave)
        {
            const string SQL_STATEMENT =
                "SELECT LeaveID " +
                  "FROM Leaves " +
                 "WHERE Employee=@Employee " +
                   "AND StartDate <= @EndDate AND EndDate >= @StartDate " +
                   "AND ([Status]=0 OR [Status]=2) ";

            // Connect to database.
            Database db = DatabaseFactory.CreateDatabase(CONNECTION_NAME);
            using (DbCommand cmd = db.GetSqlStringCommand(SQL_STATEMENT))
            {
                db.AddInParameter(cmd, "@Employee", DbType.AnsiString, leave.Employee);
                db.AddInParameter(cmd, "@StartDate", DbType.DateTime, leave.StartDate);
                db.AddInParameter(cmd, "@EndDate", DbType.DateTime, leave.EndDate);

                using (IDataReader dr = db.ExecuteReader(cmd))
                {
                    return dr.Read();
                }
            }
        }

        /// <summary>
        /// Creates a new Leave from a Datareader.
        /// </summary>
        /// <param name="dr">A DataReader object.</param>
        /// <returns>Returns a Leave.</returns>		
        private Leave LoadLeave(IDataReader dr)
        {
            // Create a new Leave
            Leave leave = new Leave();

            // Read values.
            leave.LeaveID = base.GetDataValue<long>(dr, "LeaveID");
            leave.CorrelationID = base.GetDataValue<Guid>(dr, "CorrelationID");
            leave.Category = base.GetDataValue<LeaveCategories>(dr, "Category");
            leave.Employee = base.GetDataValue<string>(dr, "Employee");
            leave.StartDate = base.GetDataValue<DateTime>(dr, "StartDate");
            leave.EndDate = base.GetDataValue<DateTime>(dr, "EndDate");
            leave.Description = base.GetDataValue<string>(dr, "Description");
            leave.Duration = base.GetDataValue<byte>(dr, "Duration");
            leave.Status = base.GetDataValue<LeaveStatuses>(dr, "Status");
            leave.IsCompleted = base.GetDataValue<bool>(dr, "IsCompleted");
            leave.Remarks = base.GetDataValue<string>(dr, "Remarks");
            leave.DateSubmitted = base.GetDataValue<DateTime>(dr, "DateSubmitted");

            return leave;
        }


    }
}
