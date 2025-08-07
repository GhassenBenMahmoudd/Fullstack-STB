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

        public async Task<IEnumerable<DeclarationCadeau>> GetAllAsync(bool includeFiles = true)
        {
            var query = _context.DeclarationsCadeaux.AsQueryable();
            if (includeFiles)
            {
                query = query.Include(d => d.DocumentFiles);
            }
            return await query.ToListAsync();
        }

        public async Task<DeclarationCadeau?> GetByIdAsync(long id, bool includeFiles = true)
        {
            var query = _context.DeclarationsCadeaux.AsQueryable();
            if (includeFiles)
            {
                query = query.Include(d => d.DocumentFiles);
            }
            return await query.FirstOrDefaultAsync(d => d.IdCadeaux == id);
        }

        public async Task<DeclarationCadeau> CreateAsync(DeclarationCadeau cadeau)
        {
            if (cadeau == null) throw new ArgumentNullException(nameof(cadeau));

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.DeclarationsCadeaux.Add(cadeau);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return cadeau;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateAsync(DeclarationCadeau cadeau)
        {
<<<<<<< HEAD
            if (cadeau == null) throw new ArgumentNullException(nameof(cadeau));

            var existing = await _context.DeclarationsCadeaux.FindAsync(cadeau.IdCadeaux);
            if (existing == null) return false;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Entry(existing).CurrentValues.SetValues(cadeau);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
=======
            _context.DeclarationsCadeaux.Update(cadeau);
            return await _context.SaveChangesAsync() > 0;
>>>>>>> 072604d5338ccd68d133a24a0c6538b2cb7d3e70
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var entity = await _context.DeclarationsCadeaux
                .Include(d => d.DocumentFiles)
                .FirstOrDefaultAsync(d => d.IdCadeaux == id);
            if (entity == null) return false;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var file in entity.DocumentFiles)
                {
                    if (File.Exists(file.FilePath))
                    {
                        File.Delete(file.FilePath);
                    }
                }
                _context.DeclarationsCadeaux.Remove(entity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

       
    }
}
