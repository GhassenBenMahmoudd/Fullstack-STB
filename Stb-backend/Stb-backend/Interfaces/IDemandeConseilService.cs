using stb_backend.Domain;

namespace stb_backend.Interfaces
{
    public interface IDemandeConseilService
    {
        Task<IEnumerable<DemandeConseil>> GetAllAsync();
        Task<DemandeConseil?> GetByIdAsync(long id);
        Task<DemandeConseil> CreateAsync(DemandeConseil demande);
        Task<bool> UpdateAsync(DemandeConseil demande);
        Task<bool> DeleteAsync(long id);
    }
}
