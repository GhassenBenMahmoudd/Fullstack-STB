using stb_backend.Domain;

namespace stb_backend.Interfaces
{
    public interface IDeclarationCadeauService
    {
        Task<IEnumerable<DeclarationCadeau>> GetAllAsync();
        Task<DeclarationCadeau?> GetByIdAsync(long id);
        Task<DeclarationCadeau> CreateAsync(DeclarationCadeau cadeau);
        Task<bool> UpdateAsync(DeclarationCadeau cadeau);
        Task<bool> DeleteAsync(long id);
    }
}
