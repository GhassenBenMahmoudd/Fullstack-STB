using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace stb_backend.Domain
{
   
   
    [Table("DeclarationsCorruption")]
    public class DeclarationCorruption
    {
        [Key]
        public long IdCorruption { get; set; }

        [ForeignKey("Employe")]
        public long IdUser { get; set; }

        [Required]
        [StringLength(100)]
        public string ObjetSignalement { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }

        [StringLength(200)]
        public string EntitesConcernees { get; set; }

        [Required]
        public DateTime DateObservation { get; set; }

        [Required]
        public TypeDomaine TypeDomaine { get; set; }

        public Statut Statut { get; set; } 

        public bool Anonyme { get; set; }

        // Relations
        public virtual Employe Employe { get; set; }
        public virtual ICollection<DocumentFile> DocumentFiles { get; set; } = new List<DocumentFile>();
    }

}
