using Microsoft.EntityFrameworkCore;
using stb_backend.Data;
using stb_backend.Domain;
using stb_backend.Interfaces;

namespace stb_backend.Services
{
    
        public class DeclarationCorruptionService : IDeclarationCorruptionService
        {
            private readonly StbDbContext _context;

            public DeclarationCorruptionService(StbDbContext context)
            {
                _context = context;
            }

            public async Task<IEnumerable<DeclarationCorruption>> GetAllAsync()
            {
                return await _context.DeclarationsCorruption.Include(d => d.DocumentFiles).ToListAsync();
            }

            public async Task<DeclarationCorruption?> GetByIdAsync(long id)
            {
                return await _context.DeclarationsCorruption
                    .Include(d => d.DocumentFiles)
                    .FirstOrDefaultAsync(d => d.IdCorruption == id);
            }

            public async Task<DeclarationCorruption> CreateAsync(DeclarationCorruption declaration)
            {
                _context.DeclarationsCorruption.Add(declaration);
                await _context.SaveChangesAsync();
                return declaration;
            }

            public async Task<bool> UpdateAsync(DeclarationCorruption declaration)
            {
                _context.Entry(declaration).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }

            public async Task<bool> DeleteAsync(long id)
            {
                var item = await _context.DeclarationsCorruption.FindAsync(id);
                if (item == null) return false;

                _context.DeclarationsCorruption.Remove(item);
                await _context.SaveChangesAsync();
                return true;
            }
        }
    }

