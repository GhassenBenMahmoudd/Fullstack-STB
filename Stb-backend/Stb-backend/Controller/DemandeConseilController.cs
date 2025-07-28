using Microsoft.AspNetCore.Mvc;
using stb_backend.Domain;
using stb_backend.Interfaces;

namespace stb_backend.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class DemandeConseilController : ControllerBase
    {
        private readonly IDemandeConseilService _service;

        public DemandeConseilController(IDemandeConseilService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DemandeConseil>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<DemandeConseil>> GetById(long id)
        {
            var demande = await _service.GetByIdAsync(id);
            if (demande == null)
                return NotFound();

            return Ok(demande);
        }

        [HttpPost]
        public async Task<ActionResult<DemandeConseil>> Create([FromBody] DemandeConseil demande)
        {
            if (demande == null)
                return BadRequest("Données invalides");

            var created = await _service.CreateAsync(demande);

            return CreatedAtAction(nameof(GetById),
                new { id = created.IdConseil },
                created);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] DemandeConseil demande)
        {
            if (id != demande.IdConseil)
                return BadRequest("L'ID dans l'URL ne correspond pas à l'objet");

            var updated = await _service.UpdateAsync(demande);
            if (!updated)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
