using System;
using System.Data;
using System.Configuration;
using System.Text.RegularExpressions;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace LeaveSample.Data
{
    /// <summary>
    /// Base data access component class.
    /// </summary>
    public abstract class DataAccessComponent
    {
        protected const string CONNECTION_NAME = "default";

        static DataAccessComponent()
        {
            DatabaseFactory.SetDatabaseProviderFactory(new DatabaseProviderFactory());
        }

        protected int PageSize
        {
            get
            {
                // TODO: Define PageSize in config file.
                return Convert.ToInt32(ConfigurationManager.AppSettings["PageSize"]);
            }
        }

        protected T GetDataValue<T>(IDataReader dr, string columnName)
        {
            // NOTE: GetOrdinal() is used to automatically determine where the column
            //       is physically located in the database table. This allows the
            //       schema to be changed without affecting this piece of code.
            //       This of course sacrifices a little performance for maintainability.
            int i = dr.GetOrdinal(columnName);

            if (!dr.IsDBNull(i))
                return (T)dr.GetValue(i);
            else
                return default(T);
        }

        protected string FormatFilterStatement(string filter)
        {
            return Regex.Replace(filter, "^(AND|OR)", string.Empty);
        }
    }
}