using stb_backend.Domain;

namespace stb_backend.Interfaces
{
    public interface IDeclarationCorruptionService
    {
        Task<IEnumerable<DeclarationCorruption>> GetAllAsync();
        Task<DeclarationCorruption?> GetByIdAsync(long id);
        Task<DeclarationCorruption> CreateAsync(DeclarationCorruption declaration);
        Task<bool> UpdateAsync(DeclarationCorruption declaration);
        Task<bool> DeleteAsync(long id);
    }
}
