using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace stb_backend.Domain
{
   
    
    [Table("DemandesConseil")]
    public class DemandeConseil
    {
        [Key]
        public long IdConseil { get; set; }

        [ForeignKey("Employe")]
        public long IdUser { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        public DateTime DateDemande { get; set; }

        [Required]
        [StringLength(100)]
        public string Objet { get; set; }

        public Statut Statut { get; set; } 

        public bool Anonyme { get; set; }

        // Relations
        public virtual Employe Employe { get; set; }
        public virtual ICollection<DocumentFile> DocumentFiles { get; set; } = new List<DocumentFile>();
    }
}
