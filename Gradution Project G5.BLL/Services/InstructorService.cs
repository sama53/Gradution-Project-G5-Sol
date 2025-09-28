using Gradution_Project_G5.BLL.Interfaces;
using Gradution_Project_G5.BLL.ViewModels;
using Gradution_Project_G5.DAL.Models;
using Gradution_Project_G5.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace Gradution_Project_G5.BLL.Services
{
    public class InstructorService : IInstructorServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InstructorService> _logger;

        public InstructorService(IUnitOfWork unitOfWork, ILogger<InstructorService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<SelectListItem>>> GetInstructorsSelectListAsync()
        {
            try
            {
                var selectList = await _unitOfWork.Instructors.GetInstructorSelectListAsync();
                return ResultHelper.Success<IEnumerable<SelectListItem>>(selectList); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting instructors select list");
                return ResultHelper.Failure<IEnumerable<SelectListItem>>("An error occurred while retrieving instructors"); 
            }
        }

        public async Task<Result<Instructor>> GetInstructorById(int id)
        {
            var instructor = await _unitOfWork.Instructors.GetByIdAsync(id);
            return ResultHelper.Success<Instructor>(instructor);
        }

       
    }
}