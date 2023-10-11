using Microsoft.AspNetCore.Mvc;
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
    }
}
