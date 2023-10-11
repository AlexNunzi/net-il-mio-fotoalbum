using System.ComponentModel.DataAnnotations;

namespace net_il_mio_fotoalbum.Models
{
    public class Photo
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "Il titolo della foto è obbligatorio")]
        [MaxLength(100, ErrorMessage = "La massima lunghezza del titolo è di 100 caratteri")]
        public string Title { get; set; }
        [Required(ErrorMessage = "La descrizione della foto è obbligatoria!")]
        public string Description { get; set; }
        [MaxLength(500, ErrorMessage = "Il link non può essere lungo più di 500 caratteri")]
        public string? ImageUrl { get; set; }

        public byte[]? ImageFile { get; set; }

        public string ImageSrc =>
            ImageFile is null ? ImageUrl is null ? "" : ImageUrl : $"data:image/png;base64,{Convert.ToBase64String(ImageFile)}";
        [Required(ErrorMessage = "Impostare la visibilità dell'immagine è obbligatorio")]
        public bool Visibility { get; set; }

        // Relazione con Category
        public List<Category>? Categories { get; set; }

        public Photo() { }
    }
}
