using Gradution_Project_G5.BLL.Interfaces;
using Gradution_Project_G5.BLL.ViewModels;
using Gradution_Project_G5.BLL.ViewModels.SessionsVM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gradution_Project_G5.UI.Controllers
{
    public class SessionsController : Controller
    {
        private readonly ISessionServices _sessionService;
        private readonly ICourseServices _courseService;

        public SessionsController(ISessionServices sessionService, ICourseServices courseService)
        {
            _sessionService = sessionService;
            _courseService = courseService;
        }

        // GET: Sessions
        public async Task<IActionResult> Index(string searchTerm, int page = 1, int pageSize = 10)
        {
            ViewBag.CurrentFilter = searchTerm;

            var result = string.IsNullOrEmpty(searchTerm)
               ? await _sessionService.GetAllSessionsAsync(page, pageSize)
               : await _sessionService.SearchSessionsAsync(searchTerm, page, pageSize);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return View(new PagedResult<SessionVM>());
            }

            return View(result.Data);
        }

        // GET: Sessions/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = await _sessionService.GetSessionByIdAsync(id);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        // GET: Sessions/Create
        public async Task<IActionResult> Create()
        {
            await LoadCourses();
            return View();
        }

        // POST: Sessions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SessionCreateVM sessionVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _sessionService.CreateSessionAsync(sessionVM);
                if (result.Success)
                {
                    TempData["Success"] = "Session created successfully";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = result.Message;
            }

            await LoadCourses();
            return View(sessionVM);
        }

        // GET: Sessions/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _sessionService.GetSessionByIdAsync(id);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            var editVM = new SessionEditVM
            {
                Id = result.Data.Id,
                Title = result.Data.Title,
                StartDate = result.Data.StartDate,
                EndDate = result.Data.EndDate,
                CourseId = result.Data.CourseId
            };

            await LoadCourses();
            return View(editVM);
        }

        // POST: Sessions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SessionEditVM sessionVM)
        {
            if (id != sessionVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = await _sessionService.UpdateSessionAsync(id, sessionVM);
                if (result.Success)
                {
                    TempData["Success"] = "Session updated successfully";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = result.Message;
            }

            await LoadCourses();
            return View(sessionVM);
        }

        // POST: Sessions/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _sessionService.DeleteSessionAsync(id);
            if (result.Success)
            {
                TempData["Success"] = "Session deleted successfully";
            }
            else
            {
                TempData["Error"] = result.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCourses()
        {
            var coursesResult = await _courseService.GetAllCoursesAsync(1, 1000);
            ViewBag.Courses = coursesResult.Success
                ? new SelectList(coursesResult.Data.Items.Where(c => c.IsActive), "Id", "Name")
                : new SelectList(new List<string>());
        }
    }
}