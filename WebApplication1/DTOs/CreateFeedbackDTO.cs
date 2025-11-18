namespace TechDesk.DTOs
{
    public class CreateFeedbackDTO
    {
        public int UsuarioId { get; set; }
        public int Nota { get; set; }
        public string? Comentario { get; set; }
    }
}