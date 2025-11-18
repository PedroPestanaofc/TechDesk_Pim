using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechDesk.Data;
using TechDesk.DTOs;
using TechDesk.Models;
using TechDesk.Services;

namespace TechDesk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChamadosController : ControllerBase
    {
        private readonly TechDeskDbContext _context;
        private readonly IaService _iaService;

        public ChamadosController(TechDeskDbContext context, IaService iaService)
        {
            _context = context;
            _iaService = iaService;
        }

        // 🔹 GET: api/Chamados
        [HttpGet]
        public async Task<IActionResult> GetChamados()
        {
            var chamados = await _context.Chamados
                .Include(c => c.IdCategoriaNavigation)
                .Include(c => c.IdUsuarioNavigation)
                .Include(c => c.IdTecnicoNavigation)
                .Select(c => new
                {
                    c.IdChamado,
                    c.Titulo,
                    c.Descricao,
                    c.Status,
                    c.Nivel,
                    c.DataInicio,
                    c.DataFinal,
                    UsuarioNome = c.IdUsuarioNavigation != null ? (string?)c.IdUsuarioNavigation.Nome : null,
                    CategoriaNome = c.IdCategoriaNavigation != null ? (string?)c.IdCategoriaNavigation.Nome : null,
                    TecnicoNome = c.IdTecnicoNavigation != null ? (string?)c.IdTecnicoNavigation.Nome : null
                })
                .ToListAsync();

            return Ok(chamados);
        }

        // 🔹 GET: api/Chamados/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetChamadoPorId(int id)
        {
            var chamado = await _context.Chamados
                .Include(c => c.IdCategoriaNavigation)
                .Include(c => c.IdUsuarioNavigation)
                .Include(c => c.IdTecnicoNavigation)
                .Where(c => c.IdChamado == id)
                .Select(c => new
                {
                    c.IdChamado,
                    c.Titulo,
                    c.Descricao,
                    c.Status,
                    c.Nivel,
                    c.DataInicio,
                    c.DataFinal,
                    UsuarioNome = c.IdUsuarioNavigation != null ? (string?)c.IdUsuarioNavigation.Nome : null,
                    CategoriaNome = c.IdCategoriaNavigation != null ? (string?)c.IdCategoriaNavigation.Nome : null,
                    TecnicoNome = c.IdTecnicoNavigation != null ? (string?)c.IdTecnicoNavigation.Nome : null
                })
                .FirstOrDefaultAsync();

            if (chamado == null)
                return NotFound("Chamado não encontrado.");

            return Ok(chamado);
        }

        // 🔹 POST: api/Chamados
        [HttpPost]
        public async Task<IActionResult> CriarChamado([FromBody] CreateChamadoDTO dto)
        {
            if (dto == null)
                return BadRequest("Os dados do chamado são obrigatórios.");

            var usuario = await _context.Usuarios.FindAsync(dto.IdUsuario);
            if (usuario == null) return BadRequest("Usuário não encontrado.");

            var categoria = await _context.Categorias.FindAsync(dto.IdCategoria);
            if (categoria == null) return BadRequest("Categoria não encontrada.");

            var tecnico = dto.IdTecnico.HasValue
                ? await _context.Tecnicos.FindAsync(dto.IdTecnico)
                : null;

            if (dto.IdTecnico.HasValue && tecnico == null)
                return BadRequest("Técnico não encontrado.");

            var chamado = new Chamado
            {
                Titulo = dto.Titulo,
                Descricao = dto.Descricao,
                Status = "Aberto",
                DataInicio = DateTime.UtcNow,
                IdUsuario = dto.IdUsuario,
                IdCategoria = dto.IdCategoria,
                IdTecnico = dto.IdTecnico,
                Nivel = dto.Nivel ?? string.Empty
            };

            _context.Chamados.Add(chamado);
            await _context.SaveChangesAsync();

            // 🔹 Chama IA para gerar primeira mensagem
            try
            {
                await _iaService.criarMensagemComIA(chamado.IdChamado, chamado.Descricao);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[IA ERRO] Falha ao gerar mensagem com IA: {ex.Message}");
                // Não lança a exceção, apenas registra — assim o chamado é criado normalmente
            }

            var retorno = new
            {
                chamado.IdChamado,
                chamado.Titulo,
                chamado.Descricao,
                chamado.Status,
                chamado.Nivel,
                chamado.DataInicio,
                chamado.DataFinal,
                UsuarioNome = usuario.Nome,
                CategoriaNome = categoria.Nome,
                TecnicoNome = tecnico != null ? tecnico.Nome : null
            };

            return CreatedAtAction(nameof(GetChamadoPorId), new { id = chamado.IdChamado }, retorno);
        }

        // 🔹 PUT: api/Chamados/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarChamado(int id, [FromBody] UpdateChamadoDTO dto)
        {
            var chamado = await _context.Chamados.FirstOrDefaultAsync(c => c.IdChamado == id);
            if (chamado == null)
                return NotFound("Chamado não encontrado.");

            chamado.Titulo = dto.Titulo ?? chamado.Titulo;
            chamado.Descricao = dto.Descricao ?? chamado.Descricao;
            chamado.Status = dto.Status ?? chamado.Status;
            chamado.Nivel = dto.Nivel ?? chamado.Nivel;
            chamado.DataFinal = dto.DataFinal ?? chamado.DataFinal;

            await _context.SaveChangesAsync();

            var usuario = await _context.Usuarios.FindAsync(dto.IdUsuario);
            var categoria = await _context.Categorias.FindAsync(dto.IdCategoria);
            var tecnico = dto.IdTecnico.HasValue
                ? await _context.Tecnicos.FindAsync(dto.IdTecnico)
                : null;

            var retorno = new
            {
                chamado.IdChamado,
                chamado.Titulo,
                chamado.Descricao,
                chamado.Status,
                chamado.Nivel,
                chamado.DataInicio,
                chamado.DataFinal,
                UsuarioNome = usuario != null ? usuario.Nome : null,
                CategoriaNome = categoria != null ? categoria.Nome : null,
                TecnicoNome = tecnico != null ? tecnico.Nome : null
            };

            return Ok(retorno);
        }

        // 🔹 DELETE: api/Chamados/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarChamado(int id)
        {
            var chamado = await _context.Chamados.FirstOrDefaultAsync(c => c.IdChamado == id);
            if (chamado == null)
                return NotFound("Chamado não encontrado.");

            _context.Chamados.Remove(chamado);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}