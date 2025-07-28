using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace stb_backend.Domain
{
   
    [Table("DeclarationsCadeaux")]
    public class DeclarationCadeau
    {
        [Key]
        public long IdCadeaux { get; set; }

        [ForeignKey("Employe")]
        public long IdUser { get; set; }

        [Required]
        [StringLength(50)]
        public string GUID { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ValeurEstime { get; set; }

        [Required]
        [StringLength(100)]
        public string IdentiteDonneur { get; set; }

        [Required]
        public TypeRelation TypeRelation { get; set; }

        [StringLength(100)]
        public string Occasion { get; set; }

        public bool Honneur { get; set; }

        [Required]
        public DateTime DateDeclaration { get; set; }

        [StringLength(1000)]
        public string Message { get; set; }

        public Statut Statut { get; set; } 

        public DateTime DateReceptionCadeaux { get; set; }

        public bool Anonyme { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        // Relations
        public virtual Employe Employe { get; set; }
        public virtual ICollection<DocumentFile> DocumentFiles { get; set; } = new List<DocumentFile>();
    }
}
