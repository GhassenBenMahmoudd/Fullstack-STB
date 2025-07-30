// Dans DTOs/UpdateStatutDto.cs
using System.ComponentModel.DataAnnotations;
using stb_backend.Domain; // Pour l'enum Statut

namespace stb_backend.DTOs
{
    public class UpdateStatutDto
    {
        [Required]
        // On s'assure que la valeur fournie est bien une des valeurs de l'enum.
        [EnumDataType(typeof(Statut), ErrorMessage = "La valeur du statut n'est pas valide.")]
        public Statut NouveauStatut { get; set; }
    }
}
