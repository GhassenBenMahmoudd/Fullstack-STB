using System.ComponentModel.DataAnnotations;
using stb_backend.Domain;

public class UpdateDeclarationCadeauDto
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "La valeur estimée doit être positive.")]
    public decimal ValeurEstime { get; set; }

    [Required]
    [StringLength(255, ErrorMessage = "L'identité du donneur ne peut pas dépasser 255 caractères.")]
    public string IdentiteDonneur { get; set; } = string.Empty;

    [Required]
    [EnumDataType(typeof(TypeRelation), ErrorMessage = "Le type de relation n'est pas valide.")]
    public TypeRelation TypeRelation { get; set; }

    [StringLength(255, ErrorMessage = "L'occasion ne peut pas dépasser 255 caractères.")]
    public string? Occasion { get; set; }

    public bool Honneur { get; set; }

    [StringLength(500, ErrorMessage = "Le message ne peut pas dépasser 500 caractères.")]
    public string? Message { get; set; }

    [Required]
    [EnumDataType(typeof(Statut), ErrorMessage = "Le statut n'est pas valide.")]
    public Statut Statut { get; set; }

    [Required]
    public DateTime DateReceptionCadeaux { get; set; }

    public bool Anonyme { get; set; }

    [StringLength(1000, ErrorMessage = "La description ne peut pas dépasser 1000 caractères.")]
    public string? Description { get; set; }

    // ✅ fichiers à ajouter
    public List<IFormFile>? NewFiles { get; set; }

    // ✅ fichiers existants à conserver (id des fichiers déjà liés)
    public List<int>? ExistingFileIds { get; set; }
}
