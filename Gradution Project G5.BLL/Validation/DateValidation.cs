using Gradution_Project_G5.BLL.ViewModels;

namespace Gradution_Project_G5.BLL.Validation
{
    public class DateValidation
    {
        public async Task<Result<bool>> ValidateSessionDatesAsync(int courseId, DateTime startDate, DateTime endDate, int? excludeSessionId = null)
        {
            if (startDate < DateTime.Now.Date)
                return ResultHelper.Failure<bool>("Start date cannot be in the past");

            if (endDate <= startDate)
                return ResultHelper.Failure<bool>("End date must be after start date");

            if ((endDate - startDate).TotalHours > 24)
                return ResultHelper.Failure<bool>("Session duration cannot exceed 24 hours");

            return ResultHelper.Success(true);
        }


    }
}