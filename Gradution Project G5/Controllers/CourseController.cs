using Gradution_Project_G5.BLL.Interfaces;
using Gradution_Project_G5.BLL.ViewModels;
using Gradution_Project_G5.BLL.ViewModels.CoursesVM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gradution_Project_G5.UI.Controllers
{
    public class CoursesController : Controller
    {
        private readonly ICourseServices _courseService;
        private readonly IInstructorServices _instructorService;

        public CoursesController(ICourseServices courseService, IInstructorServices instructorService)
        {
            _courseService = courseService;
            _instructorService = instructorService;
        }

        // GET: Courses
        public async Task<IActionResult> Index(string searchTerm, int page = 1, int pageSize = 5)
        {
            ViewBag.CurrentFilter = searchTerm;

            var result = string.IsNullOrEmpty(searchTerm)
                ? await _courseService.GetAllCoursesAsync(page, pageSize)
                : await _courseService.SearchCoursesAsync(searchTerm, page, pageSize);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(new PagedResult<CourseVM>());
            }

            return View(result.Data);
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = await _courseService.GetCourseByIdAsync(id);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);

        }

        // GET: Courses/Create
        public async Task<IActionResult> Create()
        {
            await LoadInstructors();
            return View();
        }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseCreateVM courseVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _courseService.CreateCourseAsync(courseVM);
                if (result.Success)
                {
                    TempData["Success"] = "Course created successfully";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = result.Message;
            }

            await LoadInstructors();
            return View(courseVM);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _courseService.GetCourseByIdAsync(id);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            var editVM = new CourseEditVM
            {
                Id = result.Data.Id,
                Name = result.Data.Name,
                Description = result.Data.Description,
                Category = result.Data.Category,
                StartDate = result.Data.StartDate,
                EndDate = result.Data.EndDate,
                InstructorId = result.Data.InstructorId
            };

            await LoadInstructors();
            return View(editVM);
        }

        // POST: Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CourseEditVM courseVM)
        {
            if (id != courseVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = await _courseService.UpdateCourseAsync(id, courseVM);
                if (result.Success)
                {
                    TempData["Success"] = "Course updated successfully";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = result.Message;
            }

            await LoadInstructors();
            return View(courseVM);
        }

        // POST: Courses/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _courseService.DeleteCourseAsync(id);
            if (result.Success)
            {
                TempData["Success"] = "Course deleted successfully";
            }
            else
            {
                TempData["Error"] = result.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // Remote Validation for Course Name
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> IsCourseNameUnique(string name, int id = 0)
        {
            var result = await _courseService.IsCourseNameUniqueAsync(name, id == 0 ? null : id);
            return Json(result.Success ? true : $"Course name '{name}' is already in use.");
        }

        private async Task LoadInstructors()
        {
            var instructorsResult = await _instructorService.GetInstructorsSelectListAsync();
            ViewBag.Instructors = instructorsResult.Success
                ? instructorsResult.Data
                : new List<SelectListItem>();
        }
    }
}