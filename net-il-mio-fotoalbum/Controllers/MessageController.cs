using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using net_il_mio_fotoalbum.Database;
using net_il_mio_fotoalbum.Models;

namespace net_il_mio_fotoalbum.Controllers
{
    public class MessageController : Controller
    {
        private PhotoContext _photoDatabase;

        public MessageController(PhotoContext photoDatabase)
        {
            _photoDatabase = photoDatabase;
        }

        [Authorize(Roles = "ADMIN, USER")]
        [HttpGet]
        public IActionResult Index()
        {
            List<Message> messages = _photoDatabase.Messages.ToList();
            return View("Index", messages);
        }
    }
}
