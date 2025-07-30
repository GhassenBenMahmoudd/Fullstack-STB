using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using stb_backend.Domain;
using stb_backend.DTOs;
using stb_backend.Interfaces;

namespace stb_backend.Controller
{
    [Route("api/[controller]")]
    [ApiController]
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
            var cadeaux = await _service.GetAllAsync();

            // On "mappe" (transforme) la liste d'entités en liste de DTOs
            var cadeauxDto = cadeaux.Select(c => new DeclarationCadeauDto
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

        [HttpGet("{id}")]
        public async Task<ActionResult<DeclarationCadeau>> GetById(long id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }
        // BONNE PRATIQUE : L'endpoint POST reçoit un DTO de création
        [HttpPost]
        public async Task<ActionResult<DeclarationCadeauDto>> Create([FromBody] CreateDeclarationCadeauDto cadeauDto)
        {
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
        // ON MET À JOUR LA DOCUMENTATION SWAGGER
        [ProducesResponseType(typeof(DeclarationCadeauDto), 200)] // Succès, renvoie l'objet mis à jour
        [ProducesResponseType(400)]                             // Erreur de validation
        [ProducesResponseType(404)]                             // Ressource non trouvée
        public async Task<IActionResult> Update(long id, [FromBody] UpdateDeclarationCadeauDto cadeauDto)
        {
            // 1. Récupérer l'entité existante (inchangé)
            var existingCadeau = await _service.GetByIdAsync(id);

            // 2. Vérifier si l'entité existe (inchangé)
            if (existingCadeau == null)
            {
                // On peut aussi ajouter un message pour plus de clarté
                return NotFound(new { message = $"Aucune déclaration de cadeau trouvée avec l'ID {id}." });
            }

            // 3. Mapper les propriétés du DTO vers l'entité (inchangé)
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

            // 4. Appeler le service pour sauvegarder les changements (inchangé)
            await _service.UpdateAsync(existingCadeau);

            // 5. MAPPER L'ENTITÉ MISE À JOUR VERS UN DTO DE RÉPONSE (LA MODIFICATION EST ICI)
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
                Description = existingCadeau.Description
            };

            // 6. RENVOYER Ok() AVEC LE DTO (AU LIEU DE NoContent())
            return Ok(updatedCadeauDto);
        }
        // Dans Controller/DeclarationCadeauController.cs

        [HttpDelete("{id}")]
        // BONNE PRATIQUE : Documentez les codes de retour pour Swagger
        [ProducesResponseType(typeof(object), 200)] // Succès avec un message
        [ProducesResponseType(404)]                 // Ressource non trouvée
        public async Task<IActionResult> Delete(long id)
        {
            // 1. Tente de supprimer l'élément via le service.
            var success = await _service.DeleteAsync(id);

            // 2. Vérifie le résultat.
            if (success)
            {
                // 3. Si la suppression a réussi, renvoyer un statut 200 OK
                //    avec un objet JSON contenant le message.
                return Ok(new { message = "La déclaration de cadeau a été supprimée avec succès." });
            }
            else
            {
                // 4. Si l'élément n'a pas été trouvé, renvoyer 404 Not Found.
                return NotFound(new { message = $"Aucune déclaration de cadeau trouvée avec l'ID {id}." });
            }
        }

        // Dans DeclarationCadeauController.cs

        [HttpPatch("{id}/toggle-archive")]
        // ...
        public async Task<IActionResult> ToggleArchiveStatus(long id)
        {
            var declaration = await _service.GetByIdAsync(id);

            if (declaration == null)
            {
                return NotFound(/*...*/);
            }

            // CORRECTION : Utiliser le nom de la propriété de l'entité
            declaration.EstArchive = !declaration.EstArchive;

            await _service.UpdateAsync(declaration);

            // CORRECTION : Renvoyer le bon nom de propriété
            return Ok(new
            {
                message = "Le statut d'archivage a été mis à jour avec succès.",
                nouvelEtat = declaration.EstArchive ? "Archivé" : "Désarchivé",
                archived = declaration.EstArchive // J'utilise camelCase pour la réponse JSON, c'est une convention
            });
        }

        [HttpPatch("{id}/statut")]
       // [Authorize(Policy = "PeutGererStatut")] // ON APPLIQUE LA SÉCURITÉ
        [ProducesResponseType(typeof(DeclarationCadeauDto), 200)] // On renvoie l'objet mis à jour
        [ProducesResponseType(400)] // Bad Request (ex: statut invalide)
        [ProducesResponseType(403)] // Forbidden (l'utilisateur n'est pas un manager)
        [ProducesResponseType(404)] // Not Found
        public async Task<IActionResult> UpdateStatus(long id, [FromBody] UpdateStatutDto statutDto)
        {
            // 1. Récupérer l'entité depuis le service
            var declaration = await _service.GetByIdAsync(id);

            // 2. Vérifier si elle existe
            if (declaration == null)
            {
                return NotFound(new { message = $"Aucune déclaration trouvée avec l'ID {id}." });
            }

            // OPTIONNEL : Ajouter une logique métier. Par exemple, on ne peut pas changer le statut d'une déclaration déjà acceptée/refusée.
            if (declaration.Statut == Statut.ACCEPTER || declaration.Statut == Statut.REFUSER)
            {
                return BadRequest(new { message = "Le statut de cette déclaration ne peut plus être modifié." });
            }

            // 3. Appliquer la modification
            declaration.Statut = statutDto.NouveauStatut;

            // 4. Sauvegarder les changements
            await _service.UpdateAsync(declaration);

            // 5. Mapper l'entité mise à jour vers un DTO pour la réponse
            var updatedDto = new DeclarationCadeauDto
            {
                // ... mappez tous les champs de `declaration` vers `updatedDto` ...
                IdCadeaux = declaration.IdCadeaux,
                Statut = declaration.Statut.ToString(),
                // etc.
            };

            // 6. Renvoyer une réponse de succès avec l'objet mis à jour
            return Ok(updatedDto);
        }

    }
}


