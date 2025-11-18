using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechDesk.Models
{
    [Table("Mensagens", Schema = "TechDesk")]
    public class Mensagens
    {
        public int Id { get; set; }
        public int IdChamado { get; set; }
        public int? UsuarioId { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public DateTime Data { get; set; }
    }
}
