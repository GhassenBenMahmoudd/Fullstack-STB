using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace stb_backend.Domain
{

    [Table("DocumentFile")]
    public class DocumentFile
    {
        [Key]
        public long IdFile { get; set; }

        [ForeignKey("DeclarationCorruption")]
        public long? IdCorruption { get; set; }

        [ForeignKey("DeclarationCadeaux")]
        public long? IdCadeaux { get; set; }

        [ForeignKey("DemandeConseil")]
        public long? IdConseil { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; }

        [StringLength(500)]
        public string FilePath { get; set; }

        public DateTime DateUpload { get; set; } = DateTime.Now;

        // Relations
        public virtual DeclarationCorruption DeclarationCorruption { get; set; }
        public virtual DeclarationCadeau DeclarationCadeaux { get; set; }
        public virtual DemandeConseil DemandeConseil { get; set; }
    }
}

