using ElevenNote.Data;
using ElevenNote.Models.Category;
using ElevenNote.Models.Note;
using ElevenNote.Services.CategoryService;
using ElevenNote.Services.NoteService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace ElevenNote.WebMvc.Controllers
{
    [Authorize]
    public class NoteController : Controller
    {
        private readonly INoteService _noteService;
        //private readonly ApplicationDbContext _context;  // You should try to NEVER have access to the data layer from the controller when building an n-tier application

        public NoteController(INoteService noteService)
        {
            _noteService = noteService;
        }

        //In this method, we will first need to get the User Id as a string from the token -> data from the token is stored
        //in the User.Claims property. We can use the First() method to find the User Id within the Claims object, like so:
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
            _noteService.SetUserId(userId);
            return true;
        }

        public IActionResult Index()
        {
            if (!SetUserIdInService()) return Unauthorized();
            
            var notes = _noteService.GetNotes();
            return View(notes.ToList());
        }

        public ActionResult Create()
        {
            ViewBag.CategorySelectList = new SelectList(GetCategoryDropDownList(),"CategoryId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NoteCreate model)
        {
            if (!SetUserIdInService()) return Unauthorized();
            
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ViewBag.CategorySelectList = new SelectList(GetCategoryDropDownList(), "CategoryId", "Name");


            if (_noteService.CreateNote(model))
            {
                TempData["SaveResult"] = "Your note was created.";
                return RedirectToAction("Index");
            };

            ModelState.AddModelError("", "Note could not be created.");

            return View(model);
        }

        public ActionResult Details(int id)
        {
            if (!SetUserIdInService()) return Unauthorized();

            var model = _noteService.GetNoteById(id);

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!SetUserIdInService()) return Unauthorized(); 
            
            var detail = _noteService.GetNoteById(id);
            var model = new NoteEdit()
            {
                NoteId = detail.NoteId,
                Title = detail.Title,
                Content = detail.Content
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, NoteEdit model)
        {
            if (!ModelState.IsValid) return View (model);

            if (model.NoteId != id)
            {
                ModelState.AddModelError("", "Id Mismatch");
                return View(model);
            }

            if (!SetUserIdInService()) return Unauthorized();
            if (_noteService.UpdateNote(model))
            {
                TempData["SaveResult"] = "Your note was updated.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Your note could not be updated.");
            return View(model);
        }

        [ActionName("Delete")]
        public ActionResult Delete(int id)
        {
            if (!SetUserIdInService()) return Unauthorized();

            var model = _noteService.GetNoteById(id);

            return View(model);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(int id)
        {
            if (!SetUserIdInService()) return Unauthorized();
            _noteService.DeleteNote(id);
            TempData["SaveResult"] = "Your note was deleted!";
            return RedirectToAction(nameof(Index));
        }

        private IEnumerable<CategoryListItem> GetCategoryDropDownList()
        {
            if (!SetUserIdInService()) return default;
            return _noteService.CreateCategoryDropDownList();
        }
    }
}