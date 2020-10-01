using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeaveSample.Entities;

namespace LeaveSample.Framework
{
    public static class Validations
    {
        public static void ValidateLeaveDates(Leave leave)
        {
            // Validate start and end date.
            if (leave.StartDate > leave.EndDate)
            {
                throw new ApplicationException("Start date cannot be greater than End date.");
            }

            // Validate duration.
            var totalDays = leave.EndDate.Subtract(leave.StartDate).Days + 1;
            if (leave.Duration > totalDays)
                throw new ApplicationException("Duration cannot be greater than the number of days in selected date range.");
        }
    }
}
