using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "ADMIN, USER")]
        [HttpGet]
        public IActionResult Index(string? filteringString)
        {
            PhotoIndexFiltered filteredPhotos = new PhotoIndexFiltered();
            if(filteringString == null || filteringString == "")
            {
                List<Photo> photos = _photoDatabase.Photos.Include(p => p.Categories).ToList();
                filteredPhotos.Photos = photos;
                filteredPhotos.FilteringString = filteringString;
            } else
            {
                List<Photo> photos = _photoDatabase.Photos.Where(p => p.Title.ToLower().Contains(filteringString.ToLower())).Include(p => p.Categories).ToList();
                filteredPhotos.Photos = photos;
            }
            return View("Index", filteredPhotos);
        }

        [Authorize(Roles = "ADMIN, USER")]
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

        [Authorize(Roles = "ADMIN")]
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

            formModel.Photo = new Photo();
            formModel.AvailableCategories = availableCategories;

            return View("Create", formModel);
        }

        [Authorize(Roles = "ADMIN")]
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

            formData.Photo.Categories = new List<Category>();

            if (formData.SelectedCategories != null && formData.SelectedCategories.Count > 0)
            {
                foreach(string categoryId in formData.SelectedCategories)
                {
                    long parsedCategoryId = long.Parse(categoryId);

                    Category? dbCategory = _photoDatabase.Categories.Where(c => c.Id == parsedCategoryId).FirstOrDefault();

                    if (dbCategory != null)
                    {
                        formData.Photo.Categories.Add(dbCategory);
                    }
                }
            }

            SetImageFileFromFormFile(formData);

            _photoDatabase.Add(formData.Photo);
            _photoDatabase.SaveChanges();

            return RedirectToAction("Index", "Photo");
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public IActionResult Update(long id)
        {
            Photo? updatingPhoto = _photoDatabase.Photos.Where(p => p.Id == id).Include(p => p.Categories).FirstOrDefault();

            if(updatingPhoto != null)
            {
                List<Category> dbCategories = _photoDatabase.Categories.ToList();
                PhotoFormModel formData = new PhotoFormModel();
                List<SelectListItem> availableCategories = new List<SelectListItem>();

                if (dbCategories != null)
                {
                    foreach (Category category in dbCategories)
                    {
                        string ingredientStringId = category.Id.ToString();

                        

                        availableCategories.Add(new SelectListItem { Value = ingredientStringId, Text = category.Name, Selected = updatingPhoto.Categories.Any(i => i.Id == category.Id) });
                    }
                }

                formData.Photo = updatingPhoto;
                formData.AvailableCategories = availableCategories;

                return View("Update", formData);
            }

            return NotFound($"Non è stato possibile trovare una foto con id={id}");
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(long id, PhotoFormModel formData)
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

                        availableCategories.Add(new SelectListItem { Value = ingredientStringId, Text = ingredient.Name,  });
                    }
                }

                formData.AvailableCategories = availableCategories;

                return View("Update", formData);
            }

            Photo? previousPhoto = _photoDatabase.Photos.Where(p => p.Id == id).Include(p => p.Categories).FirstOrDefault();

            if (previousPhoto != null)
            {
                previousPhoto.Title = formData.Photo.Title;
                previousPhoto.Description = formData.Photo.Description;
                previousPhoto.ImageUrl = formData.Photo.ImageUrl;
                previousPhoto.Visibility = formData.Photo.Visibility;
                if (formData.ImageFormFile != null)
                {
                    SetImageFileFromFormFile(formData);
                    previousPhoto.ImageFile = formData.Photo.ImageFile;
                }


                if (previousPhoto.Categories != null)
                {
                    previousPhoto.Categories.Clear();
                }
                else
                {
                    previousPhoto.Categories = new List<Category>();
                }

                if (formData.SelectedCategories != null && formData.SelectedCategories.Count > 0)
                {
                    foreach (string categoryId in formData.SelectedCategories)
                    {
                        long parsedCategoryId = long.Parse(categoryId);

                        Category? dbCategory = _photoDatabase.Categories.Where(c => c.Id == parsedCategoryId).FirstOrDefault();

                        if (dbCategory != null)
                        {
                            previousPhoto.Categories.Add(dbCategory);
                        }
                    }
                }
               

                _photoDatabase.SaveChanges();
            }
            else
            {
                return NotFound($"Non è stato possibile trovare una foto con id={id} da modificare.");
            }
            return RedirectToAction("Details", "Photo", new { id });
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public IActionResult Delete(long id)
        {
            Photo? deletingPhoto = _photoDatabase.Photos.Where(p => p.Id == id).FirstOrDefault();

            if(deletingPhoto != null)
            {
                _photoDatabase.Remove(deletingPhoto);
                _photoDatabase.SaveChanges();
                return RedirectToAction("Index", "Photo");
            } else
            {
                return NotFound($"Non è stato possibile trovare una foto da eliminare con id={id}");
            }
        }

        private void SetImageFileFromFormFile(PhotoFormModel formData)
        {
            if (formData.ImageFormFile == null)
            {
                return;
            }

            MemoryStream stream = new MemoryStream();
            formData.ImageFormFile.CopyTo(stream);
            formData.Photo.ImageFile = stream.ToArray();
        }
    }
}
