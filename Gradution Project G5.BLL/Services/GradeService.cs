using Gradution_Project_G5.BLL.Interfaces;
using Gradution_Project_G5.BLL.ViewModels;
using Gradution_Project_G5.BLL.ViewModels.CoursesVM;
using Gradution_Project_G5.BLL.ViewModels.GradesVM;
using Gradution_Project_G5.DAL.Entities;
using Gradution_Project_G5.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Gradution_Project_G5.BLL.Services
{
    public class GradeServices : IGradeServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GradeServices> _logger;

        public GradeServices(IUnitOfWork unitOfWork, ILogger<GradeServices> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<GradeVM>> GetGradeByIdAsync(int id)
        {
            try
            {
                var grade = (await _unitOfWork.Grades.FindAsync(g => g.Id == id)).FirstOrDefault();
                if (grade == null)
                    return ResultHelper.Failure<GradeVM>("Grade not found"); 

                var gradeVM = await MapToVM(grade);
                return ResultHelper.Success(gradeVM); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting grade by ID: {Id}", id);
                return ResultHelper.Failure<GradeVM>("An error occurred while retrieving the grade"); 
            }
        }

        public async Task<Result<PagedResult<GradeVM>>> GetGradesBySessionAsync(int sessionId, int page = 1, int pageSize = 10)
        {
            try
            {
                var grades = await _unitOfWork.Grades.GetBySessionIdAsync(sessionId);
                var totalCount = grades.Count();

                var pagedGrades = grades
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);

                var gradeVMs = new List<GradeVM>();
                foreach (var grade in pagedGrades)
                {
                    gradeVMs.Add(await MapToVM(grade));
                }

                var pagedResult = new PagedResult<GradeVM>
                {
                    Items = gradeVMs,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize
                };

                return ResultHelper.Success(pagedResult); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting grades for session: {SessionId}", sessionId);
                return ResultHelper.Failure<PagedResult<GradeVM>>("An error occurred while retrieving grades"); 
            }
        }

        public async Task<Result<PagedResult<GradeVM>>> GetGradesByTraineeAsync(int traineeId, int page = 1, int pageSize = 10)
        {
            try
            {
                var grades = await _unitOfWork.Grades.GetByTraineeIdAsync(traineeId);
                var totalCount = grades.Count();

                var pagedGrades = grades
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);

                var gradeVMs = new List<GradeVM>();
                foreach (var grade in pagedGrades)
                {
                    gradeVMs.Add(await MapToVM(grade));
                }

                var pagedResult = new PagedResult<GradeVM>
                {
                    Items = gradeVMs,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize
                };

                return ResultHelper.Success(pagedResult); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting grades for trainee: {TraineeId}", traineeId);
                return ResultHelper.Failure<PagedResult<GradeVM>>("An error occurred while retrieving grades"); 
            }
        }

        public async Task<Result<GradeVM>> CreateGradeAsync(GradeCreateVM createVM)
        {
            try
            {
            
                var validationResult = await ValidateGradeValueAsync(createVM.Value);
                if (!validationResult.Success)
                    return ResultHelper.Failure<GradeVM>(validationResult.Message); 

                var grade = new Grade
                {
                    SessionId = createVM.SessionId,
                    TraineeId = createVM.TraineeId,
                    Value = createVM.Value
                };

                await _unitOfWork.Grades.AddAsync(grade);
                await _unitOfWork.SaveChangesAsync();

                var gradeVM = await MapToVM(grade);
                return ResultHelper.Success(gradeVM); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating grade");
                return ResultHelper.Failure<GradeVM>("An error occurred while creating the grade"); 
            }
        }

        public async Task<Result<GradeVM>> UpdateGradeAsync(int id, GradeEditVM editVM)
        {
            try
            {
                var grade = (await _unitOfWork.Grades.FindAsync(g => g.Id == id)).FirstOrDefault();
                if (grade == null)
                    return ResultHelper.Failure<GradeVM>("Grade not found"); 

                
                var validationResult = await ValidateGradeValueAsync(editVM.Value);
                if (!validationResult.Success)
                    return ResultHelper.Failure<GradeVM>(validationResult.Message); 

                grade.Value = editVM.Value;
                _unitOfWork.Grades.Update(grade);
                await _unitOfWork.SaveChangesAsync();

                var gradeVM = await MapToVM(grade);
                return ResultHelper.Success(gradeVM); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating grade with ID: {Id}", id);
                return ResultHelper.Failure<GradeVM>("An error occurred while updating the grade"); 
            }
        }

        public async Task<Result<bool>> DeleteGradeAsync(int id)
        {
            try
            {
                var grade = (await _unitOfWork.Grades.FindAsync(g => g.Id == id)).FirstOrDefault();
                if (grade == null)
                    return ResultHelper.Failure<bool>("Grade not found"); 

                _unitOfWork.Grades.Delete(grade);
                await _unitOfWork.SaveChangesAsync();

                return ResultHelper.Success(true); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting grade with ID: {Id}", id);
                return ResultHelper.Failure<bool>("An error occurred while deleting the grade"); 
            }
        }

        public async Task<Result<bool>> ValidateGradeValueAsync(int value)
        {
            if (value < 0 || value > 100)
                return ResultHelper.Failure<bool>("Grade value must be between 0 and 100"); 

            return ResultHelper.Success(true); 
        }

        private async Task<GradeVM> MapToVM(Grade grade)
        {
            var session = await _unitOfWork.Sessions.GetByIdAsync(grade.SessionId);
            var trainee = await _unitOfWork.Users.GetByIdAsync(grade.TraineeId);
            var course = session != null ? await _unitOfWork.Courses.GetByIdAsync(session.CourseId) : null;

            return new GradeVM
            {
                Id = grade.Id,
                Value = grade.Value,
                SessionId = grade.SessionId,
                SessionTitle = session?.Title ?? "Unknown Session",
                TraineeId = grade.TraineeId,
                TraineeName = trainee?.Name ?? "Unknown Trainee",
                CourseName = course?.Name ?? "Unknown Course",
                CourseId = course?.Id ?? 0
            };
        }

        public async Task<Result<PagedResult<GradeVM>>> GetAllGradesAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var grades = await _unitOfWork.Grades.GetAllAsync();
                var totalCount = grades.Count();

                var pagedGrades = grades
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(MapToVM)
                    .Select(x => x.Result);

                var pagedResult = new PagedResult<GradeVM>
                {
                    Items = pagedGrades,
                    TotalCount = totalCount,
                    PageNumber = page,
                    PageSize = pageSize
                };

                return ResultHelper.Success(pagedResult); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all grades");
                return ResultHelper.Failure<PagedResult<GradeVM>>("An error occurred while retrieving grades"); 
            }
        }
    }
}