#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechDesk.Data;
using TechDesk.DTOs;
using TechDesk.Models;

namespace TechDesk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly TechDeskDbContext _context;

        public LoginController(TechDeskDbContext context)
        {
            _context = context;
        }

        // POST: api/Login
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest dto)
        {
            if (dto == null) return BadRequest("Dados inválidos.");

            var usuario = await _context.Usuarios
               .Where(u => u.Email == dto.Email && u.SenhaHash == dto.Senha)
               .Select(u => new UsuarioDTO
               {
                   Id = u.Id,
                   Nome = u.Nome,
                   Email = u.Email,
                   Perfil = u.Perfil,
                   Ativo = u.Ativo,
                   CriadoEm = u.CriadoEm,
                   AtualizadoEm = u.AtualizadoEm
               })
               .FirstOrDefaultAsync();

            if (usuario == null)
                return NotFound("Usuário não encontrado ou Senha incorreta.");

            return Ok(usuario);

        }
    }
}
#nullable restore