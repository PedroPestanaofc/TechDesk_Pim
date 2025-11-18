using Microsoft.EntityFrameworkCore;
using TechDesk.Data;
using TechDesk.Models;

namespace TechDesk.Repositories
{
    public class MensagemRepository
    {
        private readonly TechDeskDbContext _context;

        public MensagemRepository(TechDeskDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Mensagens>> GetAllAsync(int? idChamado = null)
        {
            var query = _context.Mensagens.AsQueryable();

            if (idChamado.HasValue)
                query = query.Where(m => m.IdChamado == idChamado.Value);

            return await query.OrderBy(m => m.Data).ToListAsync();
        }

        public Task<Mensagens> GetByIdAsync(int id)
            => _context.Mensagens.FirstOrDefaultAsync(m => m.Id == id);

        public async Task<Mensagens> CreateAsync(Mensagens mensagem)
        {
            _context.Mensagens.Add(mensagem);
            await _context.SaveChangesAsync();
            return mensagem;
        }

        public async Task<bool> UpdateAsync(int id, string descricao)
        {
            var msg = await _context.Mensagens.FindAsync(id);
            if (msg == null) return false;

            msg.Descricao = descricao;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var msg = await _context.Mensagens.FindAsync(id);
            if (msg == null) return false;

            _context.Mensagens.Remove(msg);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
