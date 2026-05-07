using Gradution_Project_G5.BLL.ViewModels.GradesVM;
using Gradution_Project_G5.BLL.ViewModels;

namespace Gradution_Project_G5.BLL.Interfaces
{
    public interface IGradeServices
    {
        Task<Result<GradeVM>> GetGradeByIdAsync(int id);
        Task<Result<PagedResult<GradeVM>>> GetAllGradesAsync(int page = 1, int pageSize = 5);
        Task<Result<PagedResult<GradeVM>>> GetGradesBySessionAsync(int sessionId, int page = 1, int pageSize = 5);
        Task<Result<PagedResult<GradeVM>>> GetGradesByTraineeAsync(int traineeId, int page = 1, int pageSize = 5);
        Task<Result<GradeVM>> CreateGradeAsync(GradeCreateVM createVM);
        Task<Result<GradeVM>> UpdateGradeAsync(int id, GradeEditVM editVM);
        Task<Result<bool>> DeleteGradeAsync(int id);
        Task<Result<bool>> ValidateGradeValueAsync(int value);
    }
}