using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using net_il_mio_fotoalbum.Database;
using net_il_mio_fotoalbum.Models;

namespace net_il_mio_fotoalbum.Controllers
{
    public class PhotoController : Controller
    {
        private PhotoContext _photoDatabase;

        public PhotoController(PhotoContext photoDatabase)
        {
            _photoDatabase = photoDatabase;
        }
        [HttpGet]
        public IActionResult Index()
        {
            List<Photo> photos = _photoDatabase.Photos.Include(p => p.Categories).ToList();
            return View("Index", photos);
        }

        [HttpGet]
        public IActionResult Details(long id)
        {
            Photo? detailedPhoto = _photoDatabase.Photos.Where(photo => photo.Id == id).Include(photo => photo.Categories).FirstOrDefault();

            if (detailedPhoto == null)
            {
                return NotFound($"Non è stata trovata una foto con id={id}.");
            }
            else
            {
                return View("Details", detailedPhoto);
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            PhotoFormModel formModel = new PhotoFormModel();

            List<Category> dbCategories = _photoDatabase.Categories.ToList();

            List<SelectListItem> availableCategories = new List<SelectListItem>();

            if (dbCategories != null)
            {
                foreach (Category category in dbCategories)
                {
                    string ingredientStringId = category.Id.ToString();

                    availableCategories.Add(new SelectListItem { Value = ingredientStringId, Text = category.Name });
                }
            }

            formModel.newPhoto = new Photo();
            formModel.AvailableCategories = availableCategories;

            return View("Create", formModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PhotoFormModel formData)
        {
            if (!ModelState.IsValid)
            {
                List<Category> dbCategories = _photoDatabase.Categories.ToList();

                List<SelectListItem> availableCategories = new List<SelectListItem>();

                if (dbCategories != null)
                {
                    foreach (Category ingredient in dbCategories)
                    {
                        string ingredientStringId = ingredient.Id.ToString();

                        availableCategories.Add(new SelectListItem { Value = ingredientStringId, Text = ingredient.Name });
                    }
                }

                formData.AvailableCategories = availableCategories;

                return View("Create", formData);
            }

            formData.newPhoto.Categories = new List<Category>();

            if (formData.SelectedCategories != null && formData.SelectedCategories.Count > 0)
            {
                foreach(string categoryId in formData.SelectedCategories)
                {
                    long parsedCategoryId = long.Parse(categoryId);

                    Category? dbCategory = _photoDatabase.Categories.Where(c => c.Id == parsedCategoryId).FirstOrDefault();

                    if (dbCategory != null)
                    {
                        formData.newPhoto.Categories.Add(dbCategory);
                    }
                }
            }

            SetImageFileFromFormFile(formData);

            _photoDatabase.Add(formData.newPhoto);
            _photoDatabase.SaveChanges();

            return RedirectToAction("Index", "Photo");
        }

        private void SetImageFileFromFormFile(PhotoFormModel formData)
        {
            if (formData.ImageFormFile == null)
            {
                return;
            }

            MemoryStream stream = new MemoryStream();
            formData.ImageFormFile.CopyTo(stream);
            formData.newPhoto.ImageFile = stream.ToArray();
        }
    }
}
