using Microsoft.AspNetCore.Authorization; using Microsoft.AspNetCore.Mvc; using System.Security.Claims; using stb_backend.Domain; using stb_backend.DTOs; using stb_backend.Interfaces; using System.IO; using System.Threading.Tasks; using System.Collections.Generic; using System.Linq;

namespace stb_backend.Controllers { [Route("api/[controller]")] [ApiController] [Authorize] public class DeclarationCadeauController : ControllerBase { private readonly IDeclarationCadeauService _service; private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

    public DeclarationCadeauController(IDeclarationCadeauService service)
    {
        _service = service;
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeclarationCadeauDto>>> GetAll()
    {
        var allCadeaux = await _service.GetAllAsync();
        var userIdFromToken = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userRole = User.FindFirstValue(ClaimTypes.Role);

        IEnumerable<DeclarationCadeau> filteredCadeaux;
        if (userRole == "Manager")
        {
            filteredCadeaux = allCadeaux;
        }
        else
        {
            filteredCadeaux = allCadeaux.Where(c => c.IdUser.ToString() == userIdFromToken);
        }

        var cadeauxDto = filteredCadeaux.Select(c => new DeclarationCadeauDto
        {
            IdCadeaux = c.IdCadeaux,
            IdUser = c.IdUser,
            GUID = c.GUID,
            ValeurEstime = c.ValeurEstime,
            IdentiteDonneur = c.IdentiteDonneur,
            TypeRelation = c.TypeRelation.ToString(),
            Occasion = c.Occasion,
            Honneur = c.Honneur,
            DateDeclaration = c.DateDeclaration,
            Message = c.Message,
            Statut = c.Statut.ToString(),
            DateReceptionCadeaux = c.DateReceptionCadeaux,
            Anonyme = c.Anonyme,
            Description = c.Description,
            Archived = c.EstArchive,
            DocumentFiles = c.DocumentFiles?.Select(file => new DocumentFileDto
            {
                IdFile = file.IdFile,
                FileName = file.FileName,
                FilePath = file.FilePath,
                DateUpload = file.DateUpload,
                DownloadUrl = $"{Request.Scheme}://{Request.Host}/api/files/download/{file.IdFile}"
            }).ToList() ?? new List<DocumentFileDto>()
        });

        return Ok(cadeauxDto);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DeclarationCadeauDto), 200)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<DeclarationCadeauDto>> GetById(long id)
    {
        var declaration = await _service.GetByIdAsync(id);
        if (declaration == null)
        {
            return NotFound(new { message = $"Aucune déclaration trouvée avec l'ID {id}." });
        }

        var userIdFromToken = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userRole = User.FindFirstValue(ClaimTypes.Role);

        if (declaration.IdUser.ToString() != userIdFromToken && userRole != "Manager")
        {
            return Forbid();
        }

        var declarationDto = new DeclarationCadeauDto
        {
            IdCadeaux = declaration.IdCadeaux,
            IdUser = declaration.IdUser,
            GUID = declaration.GUID,
            ValeurEstime = declaration.ValeurEstime,
            IdentiteDonneur = declaration.IdentiteDonneur,
            TypeRelation = declaration.TypeRelation.ToString(),
            Occasion = declaration.Occasion,
            Honneur = declaration.Honneur,
            DateDeclaration = declaration.DateDeclaration,
            Message = declaration.Message,
            Statut = declaration.Statut.ToString(),
            DateReceptionCadeaux = declaration.DateReceptionCadeaux,
            Anonyme = declaration.Anonyme,
            Description = declaration.Description,
            Archived = declaration.EstArchive,
            DocumentFiles = declaration.DocumentFiles?.Select(file => new DocumentFileDto
            {
                IdFile = file.IdFile,
                FileName = file.FileName,
                FilePath = file.FilePath,
                DateUpload = file.DateUpload,
                DownloadUrl = $"{Request.Scheme}://{Request.Host}/api/files/download/{file.IdFile}"
            }).ToList() ?? new List<DocumentFileDto>()
        };

        return Ok(declarationDto);
    }

    [HttpPost]
    [Authorize(Roles = "Manager,Employe")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(DeclarationCadeauDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<DeclarationCadeauDto>> Create([FromForm] CreateDeclarationCadeauDto cadeauDto)
    {
        var userIdFromToken = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdFromToken == null)
        {
            return Unauthorized();
        }

        // Validate file inputs
        if (cadeauDto.Files != null)
        {
            if (cadeauDto.Files.Any(f => f.Length > 10 * 1024 * 1024)) // 10MB limit
            {
                return BadRequest(new { message = "Un ou plusieurs fichiers dépassent la taille maximale de 10 Mo." });
            }
            if (cadeauDto.Files.Any(f => !IsValidFileType(f.ContentType)))
            {
                return BadRequest(new { message = "Type de fichier non autorisé. Seuls PDF, JPG, JPEG et PNG sont acceptés." });
            }
        }

        var cadeau = new DeclarationCadeau
        {
            GUID = Guid.NewGuid(),
            DateDeclaration = DateTime.UtcNow,
            IdUser = long.Parse(userIdFromToken), // Use token user ID for security
            ValeurEstime = cadeauDto.ValeurEstime,
            IdentiteDonneur = cadeauDto.IdentiteDonneur,
            TypeRelation = cadeauDto.TypeRelation,
            Occasion = cadeauDto.Occasion,
            Honneur = cadeauDto.Honneur,
            Message = cadeauDto.Message,
            Statut = Statut.EN_ATTENTE,
            DateReceptionCadeaux = cadeauDto.DateReceptionCadeaux,
            Anonyme = cadeauDto.Anonyme,
            Description = cadeauDto.Description,
            DocumentFiles = new List<DocumentFile>()
        };

        // Handle file uploads
        if (cadeauDto.Files != null && cadeauDto.Files.Any())
        {
            foreach (var file in cadeauDto.Files)
            {
                if (file.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                    var filePath = Path.Combine(_uploadPath, fileName);

                    try
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var documentFile = new DocumentFile
                        {
                            FileName = file.FileName,
                            FilePath = filePath,
                            DateUpload = DateTime.UtcNow
                        };
                        cadeau.DocumentFiles.Add(documentFile);
                    }
                    catch (IOException ex)
                    {
                        return StatusCode(500, new { message = $"Erreur lors de l'enregistrement du fichier {file.FileName}: {ex.Message}" });
                    }
                }
            }
        }

        // Save declaration and files in a transaction
        var createdCadeau = await _service.CreateAsync(cadeau);

        // Map to response DTO
        var createdCadeauDto = new DeclarationCadeauDto
        {
            IdCadeaux = createdCadeau.IdCadeaux,
            IdUser = createdCadeau.IdUser,
            GUID = createdCadeau.GUID,
            ValeurEstime = createdCadeau.ValeurEstime,
            IdentiteDonneur = createdCadeau.IdentiteDonneur,
            TypeRelation = createdCadeau.TypeRelation.ToString(),
            Occasion = createdCadeau.Occasion,
            Honneur = createdCadeau.Honneur,
            DateDeclaration = createdCadeau.DateDeclaration,
            Message = createdCadeau.Message,
            Statut = createdCadeau.Statut.ToString(),
            DateReceptionCadeaux = createdCadeau.DateReceptionCadeaux,
            Anonyme = createdCadeau.Anonyme,
            Description = createdCadeau.Description,
            Archived = createdCadeau.EstArchive,
            DocumentFiles = createdCadeau.DocumentFiles?.Select(file => new DocumentFileDto
            {
                IdFile = file.IdFile,
                FileName = file.FileName,
                FilePath = file.FilePath,
                DateUpload = file.DateUpload,
                DownloadUrl = $"{Request.Scheme}://{Request.Host}/api/files/download/{file.IdFile}"
            }).ToList() ?? new List<DocumentFileDto>()
        };

        return CreatedAtAction(nameof(GetById), new { id = createdCadeau.IdCadeaux }, createdCadeauDto);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(DeclarationCadeauDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateDeclarationCadeauDto cadeauDto)
    {
        var existingCadeau = await _service.GetByIdAsync(id);
        if (existingCadeau == null)
        {
            return NotFound(new { message = $"Aucune déclaration trouvée avec l'ID {id}." });
        }

        var userIdFromToken = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (existingCadeau.IdUser.ToString() != userIdFromToken)
        {
            return Forbid();
        }

        existingCadeau.ValeurEstime = cadeauDto.ValeurEstime;
        existingCadeau.IdentiteDonneur = cadeauDto.IdentiteDonneur;
        existingCadeau.TypeRelation = cadeauDto.TypeRelation;
        existingCadeau.Occasion = cadeauDto.Occasion;
        existingCadeau.Honneur = cadeauDto.Honneur;
        existingCadeau.Message = cadeauDto.Message;
        existingCadeau.Statut = cadeauDto.Statut;
        existingCadeau.DateReceptionCadeaux = cadeauDto.DateReceptionCadeaux;
        existingCadeau.Anonyme = cadeauDto.Anonyme;
        existingCadeau.Description = cadeauDto.Description;

        await _service.UpdateAsync(existingCadeau);

        var updatedCadeauDto = new DeclarationCadeauDto
        {
            IdCadeaux = existingCadeau.IdCadeaux,
            IdUser = existingCadeau.IdUser,
            GUID = existingCadeau.GUID,
            ValeurEstime = existingCadeau.ValeurEstime,
            IdentiteDonneur = existingCadeau.IdentiteDonneur,
            TypeRelation = existingCadeau.TypeRelation.ToString(),
            Occasion = existingCadeau.Occasion,
            Honneur = existingCadeau.Honneur,
            DateDeclaration = existingCadeau.DateDeclaration,
            Message = existingCadeau.Message,
            Statut = existingCadeau.Statut.ToString(),
            DateReceptionCadeaux = existingCadeau.DateReceptionCadeaux,
            Anonyme = existingCadeau.Anonyme,
            Description = existingCadeau.Description,
            Archived = existingCadeau.EstArchive,
            DocumentFiles = existingCadeau.DocumentFiles?.Select(file => new DocumentFileDto
            {
                IdFile = file.IdFile,
                FileName = file.FileName,
                FilePath = file.FilePath,
                DateUpload = file.DateUpload,
                DownloadUrl = $"{Request.Scheme}://{Request.Host}/api/files/download/{file.IdFile}"
            }).ToList() ?? new List<DocumentFileDto>()
        };

        return Ok(updatedCadeauDto);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(long id)
    {
        var declarationToDelete = await _service.GetByIdAsync(id);
        if (declarationToDelete == null)
        {
            return NotFound(new { message = $"Aucune déclaration trouvée avec l'ID {id}." });
        }

        var userIdFromToken = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (declarationToDelete.IdUser.ToString() != userIdFromToken)
        {
            return Forbid();
        }

        var success = await _service.DeleteAsync(id);
        if (success)
        {
            return Ok(new { message = "La déclaration de cadeau a été supprimée avec succès." });
        }

        return NotFound(new { message = $"La ressource n'a pas pu être supprimée." });
    }

    [HttpPatch("{id}/toggle-archive")]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> ToggleArchiveStatus(long id)
    {
        var declaration = await _service.GetByIdAsync(id);
        if (declaration == null)
        {
            return NotFound(new { message = $"Aucune déclaration trouvée avec l'ID {id}." });
        }

        declaration.EstArchive = !declaration.EstArchive;
        await _service.UpdateAsync(declaration);

        return Ok(new
        {
            message = "Le statut d'archivage a été mis à jour avec succès.",
            nouvelEtat = declaration.EstArchive ? "Archivé" : "Désarchivé",
            archived = declaration.EstArchive
        });
    }

    [HttpPatch("{id}/statut")]
    [Authorize(Roles = "Manager")]
    [ProducesResponseType(typeof(DeclarationCadeauDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateStatus(long id, [FromBody] UpdateStatutDto statutDto)
    {
        var declaration = await _service.GetByIdAsync(id);
        if (declaration == null)
        {
            return NotFound(new { message = $"Aucune déclaration trouvée avec l'ID {id}." });
        }

        if (declaration.Statut == Statut.ACCEPTER || declaration.Statut == Statut.REFUSER)
        {
            return BadRequest(new { message = "Le statut de cette déclaration ne peut plus être modifié." });
        }

        declaration.Statut = statutDto.NouveauStatut;
        await _service.UpdateAsync(declaration);

        var updatedDto = new DeclarationCadeauDto
        {
            IdCadeaux = declaration.IdCadeaux,
            IdUser = declaration.IdUser,
            GUID = declaration.GUID,
            ValeurEstime = declaration.ValeurEstime,
            IdentiteDonneur = declaration.IdentiteDonneur,
            TypeRelation = declaration.TypeRelation.ToString(),
            Occasion = declaration.Occasion,
            Honneur = declaration.Honneur,
            DateDeclaration = declaration.DateDeclaration,
            Message = declaration.Message,
            Statut = declaration.Statut.ToString(),
            DateReceptionCadeaux = declaration.DateReceptionCadeaux,
            Anonyme = declaration.Anonyme,
            Description = declaration.Description,
            Archived = declaration.EstArchive,
            DocumentFiles = declaration.DocumentFiles?.Select(file => new DocumentFileDto
            {
                IdFile = file.IdFile,
                FileName = file.FileName,
                FilePath = file.FilePath,
                DateUpload = file.DateUpload,
                DownloadUrl = $"{Request.Scheme}://{Request.Host}/api/files/download/{file.IdFile}"
            }).ToList() ?? new List<DocumentFileDto>()
        };

        return Ok(updatedDto);
    }

    private bool IsValidFileType(string contentType)
    {
        var allowedTypes = new[] { "application/pdf", "image/jpeg", "image/jpg", "image/png" };
        return allowedTypes.Contains(contentType.ToLower());
    }
}

}