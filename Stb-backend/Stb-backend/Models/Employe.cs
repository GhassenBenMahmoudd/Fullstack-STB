using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace stb_backend.Domain
{
    [Table("Employes")]
    public class Employe : User
    {
        [Required]
        [StringLength(50)]
        public string Matricule { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }

        // Relations
        public virtual ICollection<DeclarationCorruption> DeclarationsCorruption { get; set; } = new List<DeclarationCorruption>();
        public virtual ICollection<DemandeConseil> DemandesConseil { get; set; } = new List<DemandeConseil>();
        public virtual ICollection<DeclarationCadeau> DeclarationsCadeaux { get; set; } = new List<DeclarationCadeau>();
    }

}
