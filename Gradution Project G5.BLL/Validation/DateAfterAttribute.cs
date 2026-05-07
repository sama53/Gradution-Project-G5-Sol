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

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
            if (property == null)
                return new ValidationResult($"Property {_comparisonProperty} not found");

            var comparisonValue = (DateTime)property.GetValue(validationContext.ObjectInstance);

            if (value is DateTime dateValue && dateValue <= comparisonValue)
                return new ValidationResult(ErrorMessage ?? "End date must be after start date");

            return ValidationResult.Success;
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