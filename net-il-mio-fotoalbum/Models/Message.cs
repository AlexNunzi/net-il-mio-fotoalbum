using System.ComponentModel.DataAnnotations;

namespace net_il_mio_fotoalbum.Models
{
    public class Message
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Il campo E-mail è obbligatorio.")]
        [EmailAddress(ErrorMessage = "Devi inserire una E-mail valida.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Il campo Messaggio è obbligatorio.")]
        [MaxLength(5000, ErrorMessage = "Il campo Messaggio non può contenere più di 5000 caratteri.")]
        public string Content { get; set; }

        public Message() { }
    }
}
