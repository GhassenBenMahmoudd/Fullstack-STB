using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using stb_backend.Domain;
using stb_backend.DTOs;
using stb_backend.Interfaces;

namespace stb_backend.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // <-- PROTÈGE TOUS LES ENDPOINTS DE CE CONTRÔLEUR

    public class DeclarationCadeauController : ControllerBase
    {
        private readonly IDeclarationCadeauService _service;

        public DeclarationCadeauController(IDeclarationCadeauService service)
        {
            _service = service;
        }

        // BONNE PRATIQUE : L'endpoint GET renvoie une liste de DTOs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeclarationCadeauDto>>> GetAll()
        {
            var allCadeaux = await _service.GetAllAsync();

            // Récupérer les informations de l'utilisateur depuis le token
            var userIdFromToken = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            IEnumerable<DeclarationCadeau> filteredCadeaux;

            // Si l'utilisateur est un Manager, il voit tout.
            if (userRole == "Manager")
            {
                filteredCadeaux = allCadeaux;
            }
            else // Sinon (c'est un Employe), il ne voit que ses propres déclarations.
            {
                filteredCadeaux = allCadeaux.Where(c => c.IdUser.ToString() == userIdFromToken);
            }

            // On "mappe" (transforme) la liste d'entités en liste de DTOs
            var cadeauxDto = filteredCadeaux.Select(c => new DeclarationCadeauDto
            {
                IdCadeaux = c.IdCadeaux,
                IdUser = c.IdUser,
                GUID = c.GUID,
                ValeurEstime = c.ValeurEstime,
                IdentiteDonneur = c.IdentiteDonneur,
                TypeRelation = c.TypeRelation.ToString(), // Conversion propre
                Occasion = c.Occasion,
                Honneur = c.Honneur,
                DateDeclaration = c.DateDeclaration,
                Message = c.Message,
                Statut = c.Statut.ToString(), // Conversion propre
                DateReceptionCadeaux = c.DateReceptionCadeaux,
                Anonyme = c.Anonyme,
                Description = c.Description,
                Archived = c.EstArchive
            });

            return Ok(cadeauxDto);
        }

        // Dans DeclarationCadeauController.cs

        // Dans DeclarationCadeauController.cs

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DeclarationCadeauDto), 200)]
        [ProducesResponseType(403)] // Forbidden
        [ProducesResponseType(404)] // Not Found
        public async Task<ActionResult<DeclarationCadeauDto>> GetById(long id)
        {
            // 1. Récupérer la déclaration (avec les fichiers inclus)
            var declaration = await _service.GetByIdAsync(id);
            if (declaration == null)
            {
                return NotFound(new { message = $"Aucune déclaration trouvée avec l'ID {id}." });
            }

            // 2. Vérifier les autorisations
            var userIdFromToken = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (declaration.IdUser.ToString() != userIdFromToken && userRole != "Manager")
            {
                return Forbid();
            }

            // 3. Mapper l'entité DeclarationCadeau vers DeclarationCadeauDto
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

                // 4. Mapper la liste d'entités DocumentFile vers une liste de DocumentFileDto
                DocumentFiles = declaration.DocumentFiles?.Select(file => new DocumentFileDto
                {
                    IdFile = file.IdFile,
                    FileName = file.FileName,
                    DateUpload = file.DateUpload,
                    // Construire l'URL de téléchargement.
                    // Adaptez l'URL à la route de votre futur endpoint de téléchargement.
                    DownloadUrl = $"{Request.Scheme}://{Request.Host}/api/files/download/{file.IdFile}"
                }).ToList() ?? new List<DocumentFileDto>()
            };

            return Ok(declarationDto);
        }

        // BONNE PRATIQUE : L'endpoint POST reçoit un DTO de création
        [HttpPost]
        [Authorize(Roles = "Manager,Employe")] // Seuls les Managers et Employes peuvent accéder
        public async Task<ActionResult<DeclarationCadeauDto>> Create([FromBody] CreateDeclarationCadeauDto cadeauDto)
        {
            // Récupérer l'ID de l'utilisateur à partir du token, pas du corps de la requête.
            var userIdFromToken = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdFromToken == null)
            {
                // Ne devrait jamais arriver si [Authorize] est présent, mais c'est une sécurité supplémentaire.
                return Unauthorized();
            }
            // 1. Mapper le DTO en entité (cette partie est déjà correcte)
            var cadeau = new DeclarationCadeau
            {
                GUID = Guid.NewGuid(),
                DateDeclaration = DateTime.UtcNow,
                IdUser = cadeauDto.IdUser,
                ValeurEstime = cadeauDto.ValeurEstime,
                IdentiteDonneur = cadeauDto.IdentiteDonneur,
                TypeRelation = cadeauDto.TypeRelation,
                Occasion = cadeauDto.Occasion,
                Honneur = cadeauDto.Honneur,
                Message = cadeauDto.Message,
                Statut = Statut.EN_ATTENTE,
                DateReceptionCadeaux = cadeauDto.DateReceptionCadeaux,
                Anonyme = cadeauDto.Anonyme,
                Description = cadeauDto.Description
            };

            // 2. Créer l'entité dans la base de données
            var createdCadeau = await _service.CreateAsync(cadeau);

            // 3. Mapper l'entité créée vers le DTO de réponse (LA CORRECTION EST ICI)
            var createdCadeauDto = new DeclarationCadeauDto
            {
                // Assurez-vous de remplir TOUS les champs
                IdCadeaux = createdCadeau.IdCadeaux,
                IdUser = createdCadeau.IdUser,
                GUID = createdCadeau.GUID,
                ValeurEstime = createdCadeau.ValeurEstime,
                IdentiteDonneur = createdCadeau.IdentiteDonneur,
                TypeRelation = createdCadeau.TypeRelation.ToString(), // N'oubliez pas le .ToString() pour les enums
                Occasion = createdCadeau.Occasion,
                Honneur = createdCadeau.Honneur,
                DateDeclaration = createdCadeau.DateDeclaration,
                Message = createdCadeau.Message,
                Statut = createdCadeau.Statut.ToString(), // N'oubliez pas le .ToString() pour les enums
                DateReceptionCadeaux = createdCadeau.DateReceptionCadeaux,
                Anonyme = createdCadeau.Anonyme,
                Description = createdCadeau.Description
            };

            // 4. Renvoyer le DTO complet
            return CreatedAtAction(nameof(GetById), new { id = createdCadeau.IdCadeaux }, createdCadeauDto);
        }


        [HttpPut("{id}")]
        [ProducesResponseType(typeof(DeclarationCadeauDto), 200)]
        [ProducesResponseType(400)] // Bad Request
        [ProducesResponseType(403)] // Forbidden
        [ProducesResponseType(404)] // Not Found
        public async Task<IActionResult> Update(long id, [FromBody] UpdateDeclarationCadeauDto cadeauDto)
        {
            // 1. Récupérer l'entité existante pour vérifier son propriétaire
            var existingCadeau = await _service.GetByIdAsync(id);
            if (existingCadeau == null)
            {
                return NotFound(new { message = $"Aucune déclaration trouvée avec l'ID {id}." });
            }

            // 2. Récupérer l'ID de l'utilisateur depuis le token
            var userIdFromToken = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // --- DÉBUT DE LA MODIFICATION DE LA LOGIQUE ---

            // 3. Vérifier si l'utilisateur est le propriétaire de la déclaration, PEU IMPORTE SON RÔLE.
            if (existingCadeau.IdUser.ToString() != userIdFromToken)
            {
                // Si l'ID de l'utilisateur du token ne correspond pas à celui du propriétaire,
                // l'accès est refusé.
                return Forbid(); // Renvoie une réponse 403 Forbidden
            }

            // --- FIN DE LA MODIFICATION DE LA LOGIQUE ---

            // 4. Si l'autorisation est validée, procéder à la mise à jour.
            //    Mapper les propriétés du DTO vers l'entité.
            existingCadeau.ValeurEstime = cadeauDto.ValeurEstime;
            existingCadeau.IdentiteDonneur = cadeauDto.IdentiteDonneur;
            existingCadeau.TypeRelation = cadeauDto.TypeRelation;
            existingCadeau.Occasion = cadeauDto.Occasion;
            existingCadeau.Honneur = cadeauDto.Honneur;
            existingCadeau.Message = cadeauDto.Message;
            existingCadeau.Statut = cadeauDto.Statut; // Attention: un employé peut-il changer le statut ? À clarifier.
            existingCadeau.DateReceptionCadeaux = cadeauDto.DateReceptionCadeaux;
            existingCadeau.Anonyme = cadeauDto.Anonyme;
            existingCadeau.Description = cadeauDto.Description;

            await _service.UpdateAsync(existingCadeau);

            // 5. Mapper l'entité mise à jour vers un DTO pour la réponse
            var updatedCadeauDto = new DeclarationCadeauDto
            {
                // ... mappez tous les champs de `existingCadeau` vers le DTO ...
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
                Archived = existingCadeau.EstArchive
                // Ne pas oublier les DocumentFiles si nécessaire
            };

            return Ok(updatedCadeauDto);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(object), 200)] // Succès
        [ProducesResponseType(403)]                 // Interdit (Forbidden)
        [ProducesResponseType(404)]                 // Non trouvé (Not Found)
        public async Task<IActionResult> Delete(long id)
        {
            // 1. Récupérer l'entité pour vérifier son propriétaire
            var declarationToDelete = await _service.GetByIdAsync(id);
            if (declarationToDelete == null)
            {
                return NotFound(new { message = $"Aucune déclaration trouvée avec l'ID {id}." });
            }

            // 2. Récupérer l'ID de l'utilisateur depuis le token
            var userIdFromToken = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // --- DÉBUT DE LA MODIFICATION DE LA LOGIQUE ---

            // 3. Vérifier si l'utilisateur est le propriétaire de la déclaration, PEU IMPORTE SON RÔLE.
            //    La condition est maintenant beaucoup plus simple.
            if (declarationToDelete.IdUser.ToString() != userIdFromToken)
            {
                // Si l'ID de l'utilisateur du token ne correspond pas à l'ID du propriétaire de la déclaration,
                // alors l'accès est refusé.
                return Forbid(); // Renvoie une réponse 403 Forbidden
            }

            // --- FIN DE LA MODIFICATION DE LA LOGIQUE ---

            // 4. Si l'autorisation est validée (l'utilisateur est bien le propriétaire), procéder à la suppression.
            var success = await _service.DeleteAsync(id);
            if (success)
            {
                return Ok(new { message = "La déclaration de cadeau a été supprimée avec succès." });
            }

            // Ne devrait pas être atteint si la logique est correcte
            return NotFound(new { message = $"La ressource n'a pas pu être supprimée." });
        }


        [HttpPatch("{id}/toggle-archive")]
        // --- DÉBUT DE LA MODIFICATION DE LA SÉCURITÉ ---
        [Authorize(Roles = "Manager")] // Seuls les managers peuvent accéder à cet endpoint
                                       // --- FIN DE LA MODIFICATION DE LA SÉCURITÉ ---
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(403)] // Interdit (si la policy n'était pas là)
        [ProducesResponseType(404)] // Non trouvé
        public async Task<IActionResult> ToggleArchiveStatus(long id)
        {
            // Grâce à [Authorize(Roles = "Manager")], nous n'avons plus besoin de vérifier le rôle manuellement ici.
            // Le système le fait pour nous.

            var declaration = await _service.GetByIdAsync(id);

            if (declaration == null)
            {
                return NotFound(new { message = $"Aucune déclaration trouvée avec l'ID {id}." });
            }

            // Inverser le statut d'archivage
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
        // --- DÉBUT DE LA MODIFICATION DE SÉCURITÉ ---
        [Authorize(Roles = "Manager")] // Seuls les utilisateurs avec le rôle "Manager" peuvent accéder.
                                       // --- FIN DE LA MODIFICATION DE SÉCURITÉ ---
        [ProducesResponseType(typeof(DeclarationCadeauDto), 200)] // On renvoie l'objet mis à jour
        [ProducesResponseType(400)] // Bad Request (ex: statut invalide)
        [ProducesResponseType(403)] // Forbidden (géré automatiquement par l'attribut Authorize)
        [ProducesResponseType(404)] // Not Found
        public async Task<IActionResult> UpdateStatus(long id, [FromBody] UpdateStatutDto statutDto)
        {
            // Grâce à [Authorize(Roles = "Manager")], nous n'avons plus besoin de vérifier le rôle manuellement ici.
            // Le framework s'en charge pour nous.

            // 1. Récupérer l'entité depuis le service
            var declaration = await _service.GetByIdAsync(id);

            // 2. Vérifier si elle existe
            if (declaration == null)
            {
                return NotFound(new { message = $"Aucune déclaration trouvée avec l'ID {id}." });
            }

            // 3. Logique métier (déjà présente et correcte)
            if (declaration.Statut == Statut.ACCEPTER || declaration.Statut == Statut.REFUSER)
            {
                return BadRequest(new { message = "Le statut de cette déclaration ne peut plus être modifié." });
            }

            // 4. Appliquer la modification
            declaration.Statut = statutDto.NouveauStatut;

            // 5. Sauvegarder les changements
            await _service.UpdateAsync(declaration);

            // --- DÉBUT DU MAPPING COMPLET ---
            // 6. Mapper l'entité mise à jour vers un DTO pour la réponse
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
                Statut = declaration.Statut.ToString(), // Le champ mis à jour
                DateReceptionCadeaux = declaration.DateReceptionCadeaux,
                Anonyme = declaration.Anonyme,
                Description = declaration.Description,
                Archived = declaration.EstArchive,
                DocumentFiles = declaration.DocumentFiles?.Select(file => new DocumentFileDto
                {
                    IdFile = file.IdFile,
                    FileName = file.FileName,
                    DateUpload = file.DateUpload,
                    DownloadUrl = $"{Request.Scheme}://{Request.Host}/api/files/download/{file.IdFile}"
                }).ToList() ?? new List<DocumentFileDto>()
            };
            // --- FIN DU MAPPING COMPLET ---

            // 7. Renvoyer une réponse de succès avec l'objet mis à jour
            return Ok(updatedDto);
        }


    }
}


