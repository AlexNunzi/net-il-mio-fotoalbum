using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using net_il_mio_fotoalbum.Database;
using net_il_mio_fotoalbum.Models;

namespace net_il_mio_fotoalbum.Controllers
{
    public class CategoryController : Controller
    {
        private PhotoContext _photoDatabase;

        public CategoryController(PhotoContext photoDatabase)
        {
            _photoDatabase = photoDatabase;
        }

        [Authorize(Roles = "ADMIN, USER")]
        [HttpGet]
        public IActionResult Index()
        {
            List<Category> categories = _photoDatabase.Categories.ToList();
            return View("Index", categories);
        }

        [Authorize(Roles = "ADMIN, USER")]
        [HttpGet]
        public IActionResult Details(long id)
        {
            Category? detailedCategory = _photoDatabase.Categories.Where(c => c.Id == id).FirstOrDefault();

            if (detailedCategory == null)
            {
                return NotFound($"Non è stata trovata una categoria con id={id}.");
            }
            else
            {
                return View("Details", detailedCategory);
            }
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public IActionResult Create()
        {

            Category newCategory = new Category();

            return View("Create", newCategory);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category creatingCategory)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", creatingCategory);
            }

            _photoDatabase.Add(creatingCategory);
            _photoDatabase.SaveChanges();

            return RedirectToAction("Index", "Category");
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public IActionResult Update(long id)
        {
            Category? updatingCategory = _photoDatabase.Categories.Where(c => c.Id == id).FirstOrDefault();

            if (updatingCategory != null)
            {
                return View("Update", updatingCategory);
            }

            return NotFound($"Non è stato possibile trovare una categoria con id={id}");
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(long id, Category updatingCategory)
        {
            if (!ModelState.IsValid)
            {
                return View("Update", updatingCategory);
            }

            Category? previousCategory = _photoDatabase.Categories.Where(c => c.Id == id).FirstOrDefault();

            if (previousCategory != null)
            {
                previousCategory.Name = updatingCategory.Name;
                previousCategory.Description = updatingCategory.Description;
                _photoDatabase.SaveChanges();
            }
            else
            {
                return NotFound($"Non è stato possibile trovare una categoria con id={id} da modificare.");
            }
            return RedirectToAction("Details", "Category", new { id });
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public IActionResult Delete(long id)
        {
            Category? deletingCategory = _photoDatabase.Categories.Where(c => c.Id == id).FirstOrDefault();

            if (deletingCategory != null)
            {
                _photoDatabase.Remove(deletingCategory);
                _photoDatabase.SaveChanges();
                return RedirectToAction("Index", "Category");
            }
            else
            {
                return NotFound($"Non è stato possibile trovare una foto da eliminare con id={id}");
            }
        }
    }
}
