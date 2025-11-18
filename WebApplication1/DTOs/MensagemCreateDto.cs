namespace TechDesk.DTOs
{
    public class MensagemCreateDTO
    {
        public int IdChamado { get; set; }
        public int? UsuarioId { get; set; }
        public string Descricao { get; set; } = string.Empty;
    }
}
