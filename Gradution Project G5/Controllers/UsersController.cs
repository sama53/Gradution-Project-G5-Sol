using Gradution_Project_G5.BLL.Interfaces;
using Gradution_Project_G5.BLL.ViewModels;
using Gradution_Project_G5.BLL.ViewModels.UsersVM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Gradution_Project_G5.UI.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserServices _userService;

        public UsersController(IUserServices userService)
        {
            _userService = userService;
        }

        // GET: Users
        public async Task<IActionResult> Index(string searchTerm, string role, int page = 1, int pageSize = 5)
        {
            ViewBag.CurrentFilter = searchTerm;
            ViewBag.CurrentRole = role;

            var result = string.IsNullOrEmpty(searchTerm) && string.IsNullOrEmpty(role)
                ? await _userService.GetAllUsersAsync(page, pageSize)
                : await _userService.SearchUsersAsync(searchTerm, page, pageSize);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
    
                var emptyModel = new PagedResult<UserVM>
                {
                    Items = new List<UserVM>(),
                    TotalCount = 0,
                    PageNumber = page,
                    PageSize = pageSize
                };
                return View(emptyModel);
            }

            ViewBag.Roles = Enum.GetValues(typeof(Gradution_Project_G5.DAL.Models.UserRole))
                               .Cast<Gradution_Project_G5.DAL.Models.UserRole>()
                               .Select(r => new SelectListItem
                               {
                                   Value = r.ToString(),
                                   Text = r.ToString()
                               });

            return View(result.Data);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }

            return View(result.Data);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewBag.Roles = GetRoleSelectList();
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateVM userVM)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.CreateUserAsync(userVM);
                if (result.Success)
                {
                    TempData["Success"] = "User created successfully";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = result.Message;
            }

            ViewBag.Roles = GetRoleSelectList();
            return View(userVM);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int id, bool isInstructor = false)
        {
            UserEditVM editVM;

            if (isInstructor)
            {
                var result = await _userService.GetInstructorAsUserAsync(id);
                if (!result.Success)
                {
                    TempData["Error"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
                editVM = new UserEditVM
                {
                    Id = result.Data.Id,
                    Name = result.Data.Name,
                    Email = result.Data.Email,
                    Role = result.Data.Role,
                    IsInstructor = true
                };
            }
            else
            {
                var result = await _userService.GetUserByIdAsync(id);
                if (!result.Success)
                {
                    TempData["Error"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
                editVM = new UserEditVM
                {
                    Id = result.Data.Id,
                    Name = result.Data.Name,
                    Email = result.Data.Email,
                    Role = result.Data.Role,
                    IsInstructor = false
                };
            }

            ViewBag.Roles = GetRoleSelectList();
            return View(editVM);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserEditVM userVM)
        {
            if (id != userVM.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = await _userService.UpdateUserAsync(id, userVM);
                if (result.Success)
                {
                    TempData["Success"] = "User updated successfully";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = result.Message;
            }

            ViewBag.Roles = GetRoleSelectList();
            return View(userVM);
        }

        // POST: Users/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, bool isInstructor = false)
        {
            var result = isInstructor
                ? await _userService.DeleteInstructorAsync(id)
                : await _userService.DeleteUserAsync(id);

            if (result.Success)
                TempData["Success"] = "User deleted successfully";
            else
                TempData["Error"] = result.Message;

            return RedirectToAction(nameof(Index));
        }

        // Remote Validation for Email
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> IsEmailUnique(string email, int id = 0)
        {
            var result = await _userService.IsEmailUniqueAsync(email, id == 0 ? null : id);

            if (!result.Success)
                return Json($"An error occurred while validating email.");

            return result.Data
                ? Json(true)
                : Json($"Email '{email}' is already in use.");
        }

        private List<SelectListItem> GetRoleSelectList()
        {
            return Enum.GetValues(typeof(Gradution_Project_G5.DAL.Models.UserRole))
                      .Cast<Gradution_Project_G5.DAL.Models.UserRole>()
                      .Select(r => new SelectListItem
                      {
                          Value = r.ToString(),
                          Text = r.ToString()
                      })
                      .ToList();
        }
    }
}