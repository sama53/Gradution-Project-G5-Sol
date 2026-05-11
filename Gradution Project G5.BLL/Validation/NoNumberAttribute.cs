using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Gradution_Project_G5.BLL.Validation
{
    public class NoNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string stringValue)
            {
                if (Regex.IsMatch(stringValue, @"\d"))
                {
                    return new ValidationResult(ErrorMessage ?? "Field cannot contain numbers");
                }
            }

            return ValidationResult.Success;
        }
    }

    public class NoSpecialCharactersAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string stringValue)
            {
                if (Regex.IsMatch(stringValue, @"[^a-zA-Z0-9\s\-_]"))
                {
                    return new ValidationResult(ErrorMessage ?? "Field cannot contain special characters");
                }
            }

            return ValidationResult.Success;
        }
    }
    public class NotOnlyNumbersAndSpecialCharsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string stringValue && !string.IsNullOrEmpty(stringValue))
            {
                if (!Regex.IsMatch(stringValue, @"[a-zA-Z]"))
                    return new ValidationResult(ErrorMessage ?? "Field must contain at least one letter");
            }
            return ValidationResult.Success;
        }
    }
}