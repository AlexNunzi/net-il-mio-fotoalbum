using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace net_il_mio_fotoalbum.Models
{
    public class Category
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "Il nome della categoria è obbligatorio")]
        [MaxLength(100, ErrorMessage = "La massima lunghezza del nome è di 100 caratteri")]
        public string Name { get; set; }
        public string? Description { get; set; }

        // Relazione con Photo
        [JsonIgnore]
        public List<Photo>? Photos { get; set; }

        public Category() { }

    }
}
