using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
namespace Gradution_Project_G5.DAL.Entities
   
{
    public class MinDurationAttribute : ValidationAttribute, IClientModelValidator
    {
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var instance = context.ObjectInstance;
            var startProp = instance.GetType().GetProperty("StartDate");
            var endProp = instance.GetType().GetProperty("EndDate");

            if (startProp == null || endProp == null) return ValidationResult.Success;

            var start = (DateTime?)startProp.GetValue(instance);
            var end = (DateTime?)endProp.GetValue(instance);

            if (start.HasValue && end.HasValue && (end.Value - start.Value).TotalHours < 1)
                return new ValidationResult(ErrorMessage ?? "Session duration must be at least 1 hour.");

            return ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            context.Attributes["data-val"] = "true";
            context.Attributes["data-val-minduration"] = ErrorMessage ?? "Session duration must be at least 1 hour.";
        }
    }


public class Session
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int CourseId { get; set; }
        public Course? Course { get; set; }

        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
    }
}
