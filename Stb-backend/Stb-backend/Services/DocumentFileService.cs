using stb_backend.Data;
using stb_backend.Domain;
using stb_backend.Interfaces;
using Microsoft.Extensions.Logging;

public class DocumentFileService : IDocumentFileService
{
    private readonly StbDbContext _context;
    private readonly ILogger<DocumentFileService> _logger;

    public DocumentFileService(StbDbContext context, ILogger<DocumentFileService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SaveAsync(DocumentFile file)
    {
        try
        {
            await _context.DocumentFiles.AddAsync(file);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Fichier enregistré avec succès : {0}", file.FileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'enregistrement du fichier.");
            throw; // pour propagation si nécessaire
        }
    }
}
