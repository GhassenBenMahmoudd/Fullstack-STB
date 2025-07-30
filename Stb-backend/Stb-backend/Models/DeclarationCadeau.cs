using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization; // Ajout pour JsonStringEnumConverter

namespace stb_backend.Domain
{
    [Table("DeclarationsCadeaux")]
    public class DeclarationCadeau
    {
        [Key]
        public long IdCadeaux { get; set; }

        public long IdUser { get; set; }

        public Guid GUID { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValeurEstime { get; set; }

        [Required]
        [StringLength(255)]
        public string IdentiteDonneur { get; set; }

        [Required]
        [JsonConverter(typeof(JsonStringEnumConverter))] // Ajout pour la désérialisation de l'enum par chaîne
        public TypeRelation TypeRelation { get; set; }

        [StringLength(255)]
        public string? Occasion { get; set; } // Rendu nullable si non strictement requis par la logique métier

        public bool Honneur { get; set; }

        [Required]
        public DateTime DateDeclaration { get; set; }

        [StringLength(500)]
        public string? Message { get; set; } // Rendu nullable si non strictement requis par la logique métier

        public Statut Statut { get; set; }

        public DateTime DateReceptionCadeaux { get; set; }

        public bool EstArchive { get; set; } = false;

        public bool Anonyme { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; } // Rendu nullable si non strictement requis par la logique métier

        // Relations
        [ForeignKey("IdUser")]
        public virtual Employe? Employe { get; set; } // Rendu nullable
        public virtual ICollection<DocumentFile> DocumentFiles { get; set; } = new List<DocumentFile>();
    }

}
