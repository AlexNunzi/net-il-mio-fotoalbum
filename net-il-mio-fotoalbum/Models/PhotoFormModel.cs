using Microsoft.AspNetCore.Mvc.Rendering;

namespace net_il_mio_fotoalbum.Models
{
    public class PhotoFormModel
    {
        public Photo newPhoto { get; set; }
        public IFormFile? ImageFormFile { get; set; }
        public List<SelectListItem>? AvailableCategories { get; set; }
        public List<string>? SelectedCategories { get; set; }


    }
}
