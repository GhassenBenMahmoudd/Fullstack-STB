using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace stb_backend.Domain
{
    [Table("Managers")]
    public class Manager
    {
        [Key]
        public long IdUser { get; set; }

        [Required]
        [StringLength(50)]
        public string Matricule { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }

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

        
        public virtual ICollection<Employe> EquipeSupervise { get; set; } = new List<Employe>();
    }

}
