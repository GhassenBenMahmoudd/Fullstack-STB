using Microsoft.AspNetCore.Mvc;
using stb_backend.Domain;
using stb_backend.Interfaces;

namespace stb_backend.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeclarationCorruptionController : ControllerBase
    {
        private readonly IDeclarationCorruptionService _service;

        public DeclarationCorruptionController(IDeclarationCorruptionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeclarationCorruption>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DeclarationCorruption>> GetById(long id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<DeclarationCorruption>> Create(DeclarationCorruption declaration)
        {
            var created = await _service.CreateAsync(declaration);
            return CreatedAtAction(nameof(GetById), new { id = created.IdCorruption }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, DeclarationCorruption declaration)
        {
            if (id != declaration.IdCorruption) return BadRequest();
            await _service.UpdateAsync(declaration);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var success = await _service.DeleteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
