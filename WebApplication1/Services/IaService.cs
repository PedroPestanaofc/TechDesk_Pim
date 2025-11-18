using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Text;
using TechDesk.Data;
using TechDesk.DTOs;
using TechDesk.Models;

namespace TechDesk.Services
{
    //ISADORA EDITOU: DTO PARA RECEBER A REQUISIÇÃO PARA IA
    
    public class IaService
    {
        private readonly HttpClient _httpClient;
        private readonly TechDeskDbContext _context;
        private readonly MensagemService _mensagemService;
        public IaService(HttpClient httpClient, TechDeskDbContext context, MensagemService mensagemService)
        {
            _httpClient = httpClient;
            _context = context;
            _mensagemService = mensagemService;
        }

        internal async Task<string> Prompt(IaRequestDTO message)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent");
            request.Headers.Add("X-goog-api-key", "AIzaSyATe3eBdiDm2Uy0G3fd9G8xJbfp3fwM8vk");
            request.Content = new StringContent(
                $$"""
                {
                    "contents": [
                        {
                            "parts": [
                                {
                                    "text": "{{message.message}}"
                                }
                            ]
                        }
                    ]
                }
                """,
                Encoding.UTF8,
                "application/json"
            );

            using var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            using var doc = System.Text.Json.JsonDocument.Parse(responseString);
            var text = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return text;
        }


        public async Task criarMensagemComIA(int id, string descricao)
        {
            string result = await Prompt(new IaRequestDTO
            {
                message = $"Seguindo o problema a seguir, crie uma responsta formal que contenha a solução para o problema simples, seja reservado para não extender muito na resposta: {descricao}" +
                $"Você está respondendo para salvar em um banco de dados evite utilizar caracteres especiais e adicione tudo em uma pequena frase"
            });

            var mensagem = new MensagemCreateDTO
            {
                IdChamado = id,
                Descricao = result,
                UsuarioId = 61,
            };

            await _mensagemService.CreateAsync(mensagem);
        }

   
    }

}