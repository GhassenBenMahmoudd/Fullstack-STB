using Microsoft.EntityFrameworkCore;
using stb_backend.Data;
using stb_backend.Domain;
using stb_backend.Interfaces;

namespace stb_backend.Services
{
    public class DeclarationCadeauService : IDeclarationCadeauService
    {
        private readonly StbDbContext _context;

        public DeclarationCadeauService(StbDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DeclarationCadeau>> GetAllAsync()
        {
            return await _context.DeclarationsCadeaux
                                 .Include(d => d.DocumentFiles)
                                 .ToListAsync();
        }

        public async Task<DeclarationCadeau?> GetByIdAsync(long id)
        {
            return await _context.DeclarationsCadeaux
                .Include(d => d.DocumentFiles)
                .FirstOrDefaultAsync(d => d.IdCadeaux == id);
        }

        public async Task<DeclarationCadeau> CreateAsync(DeclarationCadeau cadeau)
        {
            _context.DeclarationsCadeaux.Add(cadeau);
            await _context.SaveChangesAsync();
            return cadeau;
        }

        public async Task<bool> UpdateAsync(DeclarationCadeau cadeau)
        {
            _context.Entry(cadeau).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _context.DeclarationsCadeaux.FindAsync(id);
            if (entity == null) return false;

            _context.DeclarationsCadeaux.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
