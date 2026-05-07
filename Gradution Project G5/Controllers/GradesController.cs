using Gradution_Project_G5.BLL.Interfaces;
using Gradution_Project_G5.BLL.Services;
using Gradution_Project_G5.BLL.ViewModels;
using Gradution_Project_G5.BLL.ViewModels.GradesVM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gradution_Project_G5.UI.Controllers
{
    public class GradesController : Controller
    {
        private readonly IGradeServices _gradeService;
        private readonly ISessionServices _sessionService;
        private readonly IUserServices _userService;

        public GradesController(IGradeServices gradeService, ISessionServices sessionService, IUserServices userService)
        {
            _gradeService = gradeService;
            _sessionService = sessionService;
            _userService = userService;
        }

        // GET: Grades
        public async Task<IActionResult> Index(int? sessionId, int? traineeId, int page = 1, int pageSize = 5)
        {
            ViewBag.SessionId = sessionId;
            ViewBag.TraineeId = traineeId;

            Result<PagedResult<GradeVM>> result;

            if (sessionId.HasValue && traineeId.HasValue)
            {
   
                var gradesResult = await _gradeService.GetGradesBySessionAsync(sessionId.Value, 1, int.MaxValue);

                if (gradesResult.Success)
                {
                    var filtered = gradesResult.Data.Items
                        .Where(g => g.TraineeId == traineeId.Value)
                        .ToList();

                    result = new Result<PagedResult<GradeVM>>
                    {
                        Success = true,
                        Data = new PagedResult<GradeVM>
                        {
                            Items = filtered.Skip((page - 1) * pageSize).Take(pageSize), 
                            TotalCount = filtered.Count,
                            PageNumber = page,
                            PageSize = pageSize
                        }
                    };
                }
                else
                {
                    result = new Result<PagedResult<GradeVM>> { Success = false, Message = gradesResult.Message };
                }
            }
            else if (sessionId.HasValue)
            {
                result = await _gradeService.GetGradesBySessionAsync(sessionId.Value, page, pageSize);
            }
            else if (traineeId.HasValue)
            {
                result = await _gradeService.GetGradesByTraineeAsync(traineeId.Value, page, pageSize);
            }
            else
            {
                result = await _gradeService.GetAllGradesAsync(page, pageSize);
            }

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(new PagedResult<GradeVM>());
            }

            await LoadViewData(sessionId, traineeId); 
            return View(result.Data);
        }

        // GET: Grades/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = await _gradeService.GetGradeByIdAsync(id);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        // GET: Grades/Create
        public async Task<IActionResult> Create()
        {
            await LoadViewData();
            return View();
        }

        // POST: Grades/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GradeCreateVM gradeVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _gradeService.CreateGradeAsync(gradeVM);
                if (result.Success)
                {
                    TempData["Success"] = "Grade created successfully";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = result.Message;
            }

            await LoadViewData();
            return View(gradeVM);
        }

        // GET: Grades/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _gradeService.GetGradeByIdAsync(id);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            var editVM = new GradeEditVM
            {
                Id = result.Data.Id,
                Value = result.Data.Value
            };

            return View(editVM);
        }

        // POST: Grades/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GradeEditVM gradeVM)
        {
            if (id != gradeVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = await _gradeService.UpdateGradeAsync(id, gradeVM);
                if (result.Success)
                {
                    TempData["Success"] = "Grade updated successfully";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = result.Message;
            }

            return View(gradeVM);
        }

        // POST: Grades/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _gradeService.DeleteGradeAsync(id);
            if (result.Success)
            {
                TempData["Success"] = "Grade deleted successfully";
            }
            else
            {
                TempData["Error"] = result.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadViewData(int? selectedSessionId = null, int? selectedTraineeId = null)
        {
            var sessionsResult = await _sessionService.GetAllSessionsAsync(1, 1000);
            var traineesResult = await _userService.GetUsersByRoleAsync("Trainee", 1, 1000);

            ViewBag.Sessions = sessionsResult.Success
                ? new SelectList(sessionsResult.Data.Items, "Id", "Title", selectedSessionId) 
                : new SelectList(new List<string>());

            ViewBag.Trainees = traineesResult.Success
                ? new SelectList(traineesResult.Data.Items, "Id", "Name", selectedTraineeId)   
                : new SelectList(new List<string>());
        }
    }
}