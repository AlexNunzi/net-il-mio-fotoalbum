using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using net_il_mio_fotoalbum.Database;
using net_il_mio_fotoalbum.Models;

namespace net_il_mio_fotoalbum.Controllers
{
    public class PhotoController : Controller
    {
        private PhotoContext _PhotoDatabase;

        public PhotoController(PhotoContext photoDatabase)
        {
            _PhotoDatabase = photoDatabase;
        }
        [HttpGet]
        public IActionResult Index()
        {
            List<Photo> photos = _PhotoDatabase.Photos.Include(p => p.Categories).ToList();
            return View("Index", photos);
        }

        [HttpGet]
        public IActionResult Details()
        {
            return View();
        }
    }
}
