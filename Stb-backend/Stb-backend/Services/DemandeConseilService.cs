using Microsoft.EntityFrameworkCore;
using stb_backend.Data;
using stb_backend.Domain;
using stb_backend.Interfaces;

namespace stb_backend.Services
{
    public class DemandeConseilService : IDemandeConseilService
    {
        private readonly StbDbContext _context;

        public DemandeConseilService(StbDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DemandeConseil>> GetAllAsync()
        {
            return await _context.DemandesConseils
                                 .Include(d => d.DocumentFiles)
                                 .ToListAsync();
        }

        public async Task<DemandeConseil?> GetByIdAsync(long id)
        {
            return await _context.DemandesConseils
                                 .Include(d => d.DocumentFiles)
                                 .FirstOrDefaultAsync(d => d.IdConseil == id);
        }

        public async Task<DemandeConseil> CreateAsync(DemandeConseil demande)
        {
            _context.DemandesConseils.Add(demande);
            await _context.SaveChangesAsync();
            return demande;
        }

        public async Task<bool> UpdateAsync(DemandeConseil demande)
        {
            _context.Entry(demande).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _context.DemandesConseils.FindAsync(id);
            if (entity == null) return false;

            _context.DemandesConseils.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
