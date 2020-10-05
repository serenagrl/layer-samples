using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaveSample.Entities.Validators
{
    /// <summary>
    /// Validates whether Start date is greater than End date.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Class)]
    public class DateRangeConstraintAttribute : ValidationAttribute
    {
        public DateRangeConstraintAttribute() : base () { }

        public DateRangeConstraintAttribute(string errorMessage)
            : base(errorMessage)
        {
        }

        public override bool IsValid(object value)
        {
            if (value == null || value.GetType() != typeof(Leave))
                return true;

            var leave = value as Leave;

            return !(leave.StartDate > leave.EndDate);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || value.GetType() != typeof(Leave))
                return ValidationResult.Success;

            if (!IsValid(value))
            {
                var result = new ValidationResult("Start date cannot be greater than End date.", 
                    new List<string>() { "StartDate", "EndDate" } );

                return result;
            }

            return base.IsValid(value, validationContext);
        }
    }
}
