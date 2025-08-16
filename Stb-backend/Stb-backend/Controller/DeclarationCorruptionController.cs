using Microsoft.AspNetCore.Mvc;
using stb_backend.Domain;
using stb_backend.DTOs;
using stb_backend.Interfaces;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace stb_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeclarationCorruptionController : ControllerBase
    {
        private readonly IDeclarationCorruptionService _declarationService;

        public DeclarationCorruptionController(IDeclarationCorruptionService declarationService)
        {
            _declarationService = declarationService;
        }

        // POST api/DeclarationCorruption/create-with-files
        [HttpPost("create-with-files")]
        public async Task<IActionResult> CreateWithFiles(
            [FromForm] DeclarationCorruptionCreateDto declarationDto,
            [FromForm] List<IFormFile> files)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var declaration = new DeclarationCorruption
            {
                IdUser = declarationDto.IdUser,
                ObjetSignalement = declarationDto.ObjetSignalement,
                Description = declarationDto.Description,
                EntitesConcernees = declarationDto.EntitesConcernees,
                DateObservation = declarationDto.DateObservation,
                TypeDomaine = declarationDto.TypeDomaine,
                Statut = declarationDto.Statut,
                Anonyme = declarationDto.Anonyme,
                DocumentFiles = new List<DocumentFile>()
            };

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            foreach (var file in files)
            {
                if (file != null && file.Length > 0)
                {
                    var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var doc = new DocumentFile
                    {
                        FileName = file.FileName,
                        FilePath = $"/uploads/{uniqueFileName}",
                        DateUpload = DateTime.UtcNow
                    };

                    declaration.DocumentFiles.Add(doc);
                }
            }

            var created = await _declarationService.CreateAsync(declaration);

            // Mise à jour IdCorruption FK dans les fichiers (optionnel selon tracking EF)
            foreach (var doc in declaration.DocumentFiles)
            {
                doc.IdCorruption = created.IdCorruption;
            }
            await _declarationService.UpdateAsync(created);

            var createdDto = ToDto(created);

            return CreatedAtAction(nameof(GetById), new { id = created.IdCorruption }, createdDto);
        }

        // PUT api/DeclarationCorruption/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] DeclarationCorruptionCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var declaration = await _declarationService.GetByIdAsync(id);
            if (declaration == null)
                return NotFound();

            declaration.IdUser = dto.IdUser;
            declaration.ObjetSignalement = dto.ObjetSignalement;
            declaration.Description = dto.Description;
            declaration.EntitesConcernees = dto.EntitesConcernees;
            declaration.DateObservation = dto.DateObservation;
            declaration.TypeDomaine = dto.TypeDomaine;
            declaration.Statut = dto.Statut;
            declaration.Anonyme = dto.Anonyme;

            await _declarationService.UpdateAsync(declaration);

            return NoContent();
        }

        // DELETE api/DeclarationCorruption/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var deleted = await _declarationService.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }

        // GET api/DeclarationCorruption/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DeclarationCorruptionDto>> GetById(long id)
        {
            var declaration = await _declarationService.GetByIdAsync(id);
            if (declaration == null) return NotFound();
            return Ok(ToDto(declaration));
        }

        // Mapping Entity -> DTO
        private DeclarationCorruptionDto ToDto(DeclarationCorruption entity)
        {
            return new DeclarationCorruptionDto
            {
                IdCorruption = entity.IdCorruption,
                IdUser = entity.IdUser,
                ObjetSignalement = entity.ObjetSignalement,
                Description = entity.Description,
                EntitesConcernees = entity.EntitesConcernees,
                DateObservation = entity.DateObservation,
                TypeDomaine = entity.TypeDomaine,
                Statut = entity.Statut,
                Anonyme = entity.Anonyme,
                DocumentFiles = entity.DocumentFiles.Select(df => new DocumentFileDto
                {
                    IdFile = df.IdFile,
                    FileName = df.FileName,
                    FilePath = df.FilePath,
                    DownloadUrl = GenerateDownloadUrl(df.FilePath),
                    DateUpload = df.DateUpload
                }).ToList()
            };
        }

        private string GenerateDownloadUrl(string filePath)
        {
            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}{filePath}";
        }
    }

    // DTO de création et update
    public class DeclarationCorruptionCreateDto
    {
        public long IdUser { get; set; }
        public string ObjetSignalement { get; set; }
        public string Description { get; set; }
        public string EntitesConcernees { get; set; }
        public DateTime DateObservation { get; set; }
        public TypeDomaine TypeDomaine { get; set; }
        public Statut Statut { get; set; }
        public bool Anonyme { get; set; }
    }
}
