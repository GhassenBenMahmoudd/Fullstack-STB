using Microsoft.AspNetCore.Mvc;
using stb_backend.Domain;
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeclarationCadeau>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DeclarationCadeau>> GetById(long id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<DeclarationCadeau>> Create([FromBody] DeclarationCadeau cadeau)
        {
            var created = await _service.CreateAsync(cadeau);
            return CreatedAtAction(nameof(GetById), new { id = created.IdCadeaux }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] DeclarationCadeau cadeau)
        {
            if (id != cadeau.IdCadeaux) return BadRequest();
            await _service.UpdateAsync(cadeau);
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


