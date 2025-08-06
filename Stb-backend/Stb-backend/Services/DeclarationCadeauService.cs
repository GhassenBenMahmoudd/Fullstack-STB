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
            try
            {
                // Récupérer l'entité existante depuis la base de données
                var existingCadeau = await _context.DeclarationsCadeaux
                    .Include(d => d.DocumentFiles)
                    .FirstOrDefaultAsync(d => d.IdCadeaux == cadeau.IdCadeaux);

                if (existingCadeau == null)
                {
                    return false;
                }

                // Mettre à jour les propriétés de l'entité existante
                existingCadeau.ValeurEstime = cadeau.ValeurEstime;
                existingCadeau.IdentiteDonneur = cadeau.IdentiteDonneur;
                existingCadeau.TypeRelation = cadeau.TypeRelation;
                existingCadeau.Occasion = cadeau.Occasion;
                existingCadeau.Honneur = cadeau.Honneur;
                existingCadeau.Message = cadeau.Message;
                existingCadeau.Statut = cadeau.Statut;
                existingCadeau.DateReceptionCadeaux = cadeau.DateReceptionCadeaux;
                existingCadeau.Anonyme = cadeau.Anonyme;
                existingCadeau.Description = cadeau.Description;
                existingCadeau.EstArchive = cadeau.EstArchive;

                // Marquer l'entité comme modifiée
                _context.Entry(existingCadeau).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la mise à jour: {ex.Message}");
                return false;
            }
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
