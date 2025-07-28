using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace stb_backend.Domain
{

    [Table("User")]
    public class User
    {
        [Key]
        public long IdUser { get; set; }

        [Required]
        [StringLength(100)]
        public string Prenom { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; }

        [StringLength(20)]
        public string NumeroTelephone { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }
    }
}

