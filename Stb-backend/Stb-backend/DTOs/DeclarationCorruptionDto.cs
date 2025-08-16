using stb_backend.Domain;

namespace stb_backend.DTOs
{
    public class DeclarationCorruptionDto
    {
        public long IdCorruption { get; set; }
        public long IdUser { get; set; }
        public string ObjetSignalement { get; set; }
        public string Description { get; set; }
        public string EntitesConcernees { get; set; }
        public DateTime DateObservation { get; set; }
        public TypeDomaine TypeDomaine { get; set; }
        public Statut Statut { get; set; }
        public bool Anonyme { get; set; }
        public List<DocumentFileDto> DocumentFiles { get; set; }
    }
}
