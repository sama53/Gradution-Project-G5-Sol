using System.ComponentModel.DataAnnotations;

namespace Gradution_Project_G5.BLL.Validation
{
    public class DateAfterAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateAfterAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

    }

    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dateValue)
            {
                if (dateValue < DateTime.Now)
                {
                    return new ValidationResult(ErrorMessage ?? "Date cannot be in the past");
                }
            }
            return ValidationResult.Success;
        }
    }
}