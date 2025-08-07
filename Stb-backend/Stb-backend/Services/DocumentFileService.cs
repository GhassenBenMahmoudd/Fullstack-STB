using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using stb_backend.Data;
using stb_backend.Domain;

namespace stb_backend.Interfaces
{
    public class DocumentFileService : IDocumentFileService
    {
        private readonly StbDbContext _context;
        private readonly IWebHostEnvironment _env;

        public DocumentFileService(StbDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task SaveAsync(DocumentFile file)
        {
            _context.DocumentFiles.Add(file);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(long cadeauId, List<IFormFile> newFiles, List<long> existingFileIds)
        {
            // 1. Récupérer les fichiers existants associés à ce cadeau
            var currentFiles = await _context.DocumentFiles
                .Where(f => f.IdCadeaux == cadeauId)
                .ToListAsync();

            // 2. Supprimer les fichiers qui ne sont plus inclus dans existingFileIds
            var filesToRemove = currentFiles
                .Where(f => !existingFileIds.Contains(f.IdFile))
                .ToList();

            foreach (var file in filesToRemove)
            {
                var fullPath = Path.Combine(_env.ContentRootPath, "uploads", file.FileName);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath); // Supprimer physiquement le fichier
                }

                _context.DocumentFiles.Remove(file); // Supprimer de la base
            }

            // 3. Ajouter les nouveaux fichiers
            foreach (var newFile in newFiles)
            {
                if (newFile.Length > 0)
                {
                    var fileName = Path.GetFileName(newFile.FileName);
                    var uniqueName = $"{Guid.NewGuid()}_{fileName}";
                    var uploadDir = Path.Combine(_env.ContentRootPath, "uploads");

                    if (!Directory.Exists(uploadDir))
                        Directory.CreateDirectory(uploadDir);

                    var fullPath = Path.Combine(uploadDir, uniqueName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await newFile.CopyToAsync(stream);
                    }

                    var documentFile = new DocumentFile
                    {
                        FileName = uniqueName,
                        FilePath = fullPath,
                        IdCadeaux = cadeauId,
                        DateUpload = DateTime.Now
                    };

                    _context.DocumentFiles.Add(documentFile);
                }
            }

            // 4. Sauvegarder les changements
            await _context.SaveChangesAsync();
        }
    }
}
