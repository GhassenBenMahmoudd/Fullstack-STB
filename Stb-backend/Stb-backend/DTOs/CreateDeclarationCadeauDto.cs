// Dans DTOs/CreateDeclarationCadeauDto.cs
using System.ComponentModel.DataAnnotations;
using stb_backend.Domain; // Pour accéder aux enums TypeRelation et Statut

namespace stb_backend.DTOs
{
    public class CreateDeclarationCadeauDto
    {
        [Required]
        public long IdUser { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "La valeur estimée doit être positive.")]
        public decimal ValeurEstime { get; set; }

        [Required]
        [StringLength(255)]
        public string IdentiteDonneur { get; set; }

        [Required]
        public TypeRelation TypeRelation { get; set; }

        [StringLength(255)]
        public string? Occasion { get; set; }

        public bool Honneur { get; set; }

        [StringLength(500)]
        public string? Message { get; set; }

        [Required]
        public Statut Statut { get; set; }

        [Required]
        public DateTime DateReceptionCadeaux { get; set; }

        public bool Anonyme { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }
    }
}
