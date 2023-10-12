using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using net_il_mio_fotoalbum.Database;
using net_il_mio_fotoalbum.Models;

namespace net_il_mio_fotoalbum.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PhotoapiController : ControllerBase
    {
        private PhotoContext _photoDatabase;

        public PhotoapiController(PhotoContext photoDatabase)
        {
            _photoDatabase = photoDatabase;
        }

        [HttpGet]
        public IActionResult GetPhotos()
        {
            List<Photo> allPhotos = _photoDatabase.Photos.Where(p => p.Visibility == true).Include(p => p.Categories).ToList();
            if (allPhotos != null && allPhotos.Count > 0)
            {
                return Ok(allPhotos);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        public IActionResult SearchPhotos(string? searchingName)
        {
            if (searchingName != null && searchingName.Trim().Length > 0)
            {
                List<Photo> allFilteredPhotos = _photoDatabase.Photos.Where(p => p.Title.ToLower().Contains(searchingName.Trim().ToLower()) && p.Visibility == true).Include(p => p.Categories).ToList();
                if (allFilteredPhotos != null && allFilteredPhotos.Count > 0)
                {
                    return Ok(allFilteredPhotos);
                }
            }

            return GetPhotos();
        }
    }
}
