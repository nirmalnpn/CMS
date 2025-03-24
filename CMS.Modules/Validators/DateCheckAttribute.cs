using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Modules.Validators
{
    internal class DateCheckAttribute:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value,ValidationContext validation)
        {
            var date = (DateTime?)value;
            if (date < DateTime.MinValue)
            {
                return new ValidationResult("The date must be grater than or equal to todays date");
            }
            return ValidationResult.Success;
        }
    }
}
