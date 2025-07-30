// Dans DTOs/DeclarationCadeauDto.cs
namespace stb_backend.DTOs
{
    public class DeclarationCadeauDto
    {
        public long IdCadeaux { get; set; }
        public long IdUser { get; set; }
        public Guid GUID { get; set; }
        public decimal ValeurEstime { get; set; }
        public string IdentiteDonneur { get; set; }
        public string TypeRelation { get; set; } // Enum converti en string
        public string? Occasion { get; set; }
        public bool Honneur { get; set; }
        public DateTime DateDeclaration { get; set; }
        public string? Message { get; set; }
        public string Statut { get; set; } // Enum converti en string
        public DateTime DateReceptionCadeaux { get; set; }
        public bool Anonyme { get; set; }
        public string? Description { get; set; }
        public bool Archived { get; set; }

    }
}
