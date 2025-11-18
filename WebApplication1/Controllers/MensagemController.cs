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
    public class MensagemController : ControllerBase
    {
        private readonly TechDeskDbContext _context;
        private readonly MensagemService _mensagemService;

        public MensagemController(TechDeskDbContext context, MensagemService mensagem)
        {
            _context = context;
            _mensagemService = mensagem;
        }

        //POST /api/Mensagem/{chamadoId}
        [HttpPost("{chamadoId:int}")]
        public async Task<IActionResult> EnviarMensagem(int chamadoId, [FromBody] MensagemCreateDTO dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Descricao))
                return BadRequest("A mensagem não pode estar vazia.");

            var chamadoExiste = await _context.Chamados.AnyAsync(c => c.IdChamado == chamadoId);
            if (!chamadoExiste)
                return NotFound($"Chamado com ID {chamadoId} não encontrado.");

            try 
            {
                var novaMensagem = await _mensagemService.CreateAsync(dto);
                return CreatedAtAction(nameof(ListarMensagensPorChamado),
                    new { chamadoId = novaMensagem.IdChamado },
                    novaMensagem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao criar mensagem: {ex.Message}");
            }

        }

        // PUT /api/Mensagem/{mensagemId}
        [HttpPut("{mensagemId:int}")]
        public async Task<IActionResult> EditarMensagem(int mensagemId, [FromBody] MensagemUpdateDTO dto)
        {
            
            if (dto == null || string.IsNullOrWhiteSpace(dto.Descricao))
                return BadRequest("A descrição da mensagem não pode estar vazia.");
            try 
            {
                var mensagemAtualizada = await _mensagemService.UpdateAsync(mensagemId, dto);
                return Ok(mensagemAtualizada);
            }
            catch (KeyNotFoundException knfEx)
            {
                return NotFound(knfEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao atualizar mensagem: {ex.Message}");
            }
        }

        // ✅ GET /api/Mensagem/chamado/{chamadoId}
        [HttpGet("chamado/{chamadoId:int}")]
        public async Task<IActionResult> ListarMensagensPorChamado(int chamadoId)
        {
            try 
            {
                var mensagens = await _mensagemService.GetAllAsync(chamadoId);
                return Ok(mensagens);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao listar mensagens: {ex.Message}");
            }
        }
    }
}