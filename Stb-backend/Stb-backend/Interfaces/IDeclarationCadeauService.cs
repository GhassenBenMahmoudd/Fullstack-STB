using stb_backend.Domain;

namespace stb_backend.Interfaces
{
    public interface IDeclarationCadeauService
    {
        Task<IEnumerable<DeclarationCadeau>> GetAllAsync(bool includeFiles = true);
        Task<DeclarationCadeau?> GetByIdAsync(long id, bool includeFiles = true);
        Task<DeclarationCadeau> CreateAsync(DeclarationCadeau cadeau);
        Task<bool> UpdateAsync(DeclarationCadeau cadeau);
        Task<bool> DeleteAsync(long id);
    }
}
