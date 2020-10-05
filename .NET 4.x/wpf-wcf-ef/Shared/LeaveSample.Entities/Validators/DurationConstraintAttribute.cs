using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveSample.Entities.Validators
{
    /// <summary>
    /// Validates whether duration exceeds the number of days between Start and End Date.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Class)]
    public class DurationConstraintAttribute : ValidationAttribute
    {
        public DurationConstraintAttribute() : base () { }

        public DurationConstraintAttribute(string errorMessage)
            : base(errorMessage)
        {
        }

        public override bool IsValid(object value)
        {
            if (value == null || value.GetType() != typeof(Leave))
                return true;

            var leave = value as Leave;

            var totalDays = leave.EndDate.Subtract(leave.StartDate).Days + 1;
            return !(leave.Duration > totalDays);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || value.GetType() != typeof(Leave))
                return ValidationResult.Success;

            if (!IsValid(value))
            {
                var result = new ValidationResult("Duration exceeded the number of days between Start and End dates.", 
                    new List<string>() { "Duration" } );

                return result;
            }

            return base.IsValid(value, validationContext);
        }
    }
}
