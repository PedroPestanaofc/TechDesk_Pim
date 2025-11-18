using TechDesk.DTOs;
using TechDesk.Models;
using TechDesk.Repositories;

namespace TechDesk.Services
{
    public class MensagemService
    {
        private readonly MensagemRepository _repository;

        public MensagemService(MensagemRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Mensagens>> GetAllAsync(int? idChamado = null)
            => _repository.GetAllAsync(idChamado);

        public Task<Mensagens> GetByIdAsync(int id)
            => _repository.GetByIdAsync(id);

        public async Task<Mensagens> CreateAsync(MensagemCreateDTO dto)
        {
            var mensagem = new Mensagens
            {
                IdChamado = dto.IdChamado,
                UsuarioId = dto.UsuarioId,
                Descricao = dto.Descricao,
                Data = DateTime.UtcNow
            };

            return await _repository.CreateAsync(mensagem);
        }

        public Task<bool> UpdateAsync(int id, MensagemUpdateDTO dto)
            => _repository.UpdateAsync(id, dto.Descricao);

        public Task<bool> DeleteAsync(int id)
            => _repository.DeleteAsync(id);
    }
}
