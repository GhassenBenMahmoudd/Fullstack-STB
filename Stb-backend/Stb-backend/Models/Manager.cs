using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace stb_backend.Domain
{

    [Table("Managers")]
    public class Manager : Employe
    {
        [Required]
        [StringLength(50)]
        public string Departement { get; set; }
        // Propriétés spécifiques au Manager
        public virtual ICollection<DeclarationCadeau> DeclarationCadeau { get; set; } = new List<DeclarationCadeau>();
    }

}
