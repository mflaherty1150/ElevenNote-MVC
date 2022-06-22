using ElevenNote.Models.Category;
using ElevenNote.Services.CategoryService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ElevenNote.WebMvc.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.Claims.First(i => i.Type == ClaimTypes.NameIdentifier).Value;
            if (userIdClaim == null) return default;
            return Guid.Parse(userIdClaim);
        }

        //Now we can use GetUserId() to make the SetUserInService method - > this should return a bool that tells us
        //wheather or not the User Id can successfully be set in the service (that is, if the user is logged in with a valid
        //session
        private bool SetUserIdInService()
        {
            var userId = GetUserId();
            if (userId == null) return false;

            //if everything works from above...
            _categoryService.SetUserId(userId);
            return true;
        }


        // GET: CategoryController
        public IActionResult Index()
        {
            if (!SetUserIdInService()) return Unauthorized();

            var categories = _categoryService.GetAllCategories();
            return View(categories.ToList());
        }

        // GET: CategoryController/Details/5
        public ActionResult Details(int id)
        {
            if (!SetUserIdInService()) return Unauthorized();

            var model = _categoryService.GetCategoryById(id);

            return View(model);
        }

        // GET: CategoryController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CategoryController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CategoryCreate model)
        {
            if (!SetUserIdInService()) return Unauthorized();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_categoryService.CreateCategory(model))
            {
                TempData["SaveResult"] = "Your note was created.";
                return RedirectToAction("Index");
            };

            ModelState.AddModelError("", "Note could not be created.");

            return View(model);
            //try
            //{
            //    return RedirectToAction(nameof(Index));
            //}
            //catch
            //{
            //    return View();
            //}
        }

        // GET: CategoryController/Edit/5
        public ActionResult Edit(int id)
        {
            if (!SetUserIdInService()) return Unauthorized();

            var detail = _categoryService.GetCategoryById(id);
            var model = new CategoryEdit()
            {
                CategoryId = detail.CategoryId,
                Name = detail.Name,
            };
            return View(model);
        }

        // POST: CategoryController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, CategoryEdit model)
        {
            if (!ModelState.IsValid) return View(model);

            if (model.CategoryId != id)
            {
                ModelState.AddModelError("", "Id Mismatch");
                return View(model);
            }

            if (!SetUserIdInService()) return Unauthorized();
            if (_categoryService.UpdateCategory(model))
            {
                TempData["SaveResult"] = "Your category was updated.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Your category could not be updated.");
            return View(model);
            //try
            //{
            //    return RedirectToAction(nameof(Index));
            //}
            //catch
            //{
            //    return View();
            //}
        }

        // GET: CategoryController/Delete/5
        [ActionName("Delete")]
        public ActionResult Delete(int id)
        {
            if (!SetUserIdInService()) return Unauthorized();

            var model = _categoryService.GetCategoryById(id);

            return View(model);
        }

        // POST: CategoryController/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(int id)
        {
            if (!SetUserIdInService()) return Unauthorized();
            _categoryService.DeleteCategory(id);
            TempData["SaveResult"] = "Your note was deleted!";
            return RedirectToAction(nameof(Index));
            //try
            //{
            //    return RedirectToAction(nameof(Index));
            //}
            //catch
            //{
            //    return View();
            //}
        }
    }
}
