using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using net_il_mio_fotoalbum.Database;
using net_il_mio_fotoalbum.Models;

namespace net_il_mio_fotoalbum.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class Messageapi : ControllerBase
    {

        private PhotoContext _photoDatabase;

        public Messageapi(PhotoContext photoDatabase)
        {
            _photoDatabase = photoDatabase;
        }

        [HttpPost]
        public IActionResult CreateMessage(Message newMessage)
        {
            try
            {
                _photoDatabase.Add(newMessage);
                _photoDatabase.SaveChanges();
                return Ok("Messaggio ricevuto con successo");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
