using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Projeto.Services.Interfaces;

namespace Projeto.Services.AI;

public class AIService : IAIService
{
    private readonly HttpClient _httpClient; // onde faz requisicao
    private readonly IConfiguration _config; // da acesso ao appsettings.json (onde fica a url e o token)

    public AIService(IConfiguration config)
    {
        _httpClient = new HttpClient();
        _config = config;
    }

    // Recebe um prompt e retorna a resposta da IA como string
    public async Task<string> GetAiResponseAsync(string prompt)
    {
        // Lê a URL e o token do appsettings.json
        var url   = _config["GitHubModels:ApiUrl"];
        var token = _config["GitHubModels:Token"];

        // Sem isso a API retorna 401 Unauthorized, entao passsamos o token
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        // GitHub Models exige um User-Agent identificando quem está fazendo a requisição
        // Sem isso pode retornar 403 Forbidden
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("VortexApp");

        // Monta o corpo da requisição no formato que o GitHub Models espera (igual ao OpenAI)
        // model é qual modelo de IA usar
        // messages é o array de mensagens
        // max_tokens é o limite de tokens na resposta (evita respostas muito longas
        var body = new StringContent(
            JsonSerializer.Serialize(new
            {
                model = "gpt-4.1",
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                max_tokens = 300
            }),
            Encoding.UTF8,
            "application/json"
        );

        // Faz o POST para a API do GitHub Models e aguarda a resposta
        var response = await _httpClient.PostAsync(url, body);

        // Se a API retornou erro ele lê o corpo do erro e retorna como string
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return $"Erro: {response.StatusCode} {error}";
        }

        // Lê o JSON de resposta como string
        var json = await response.Content.ReadAsStringAsync();

        // A resposta do GitHub Models tem esse formato:
        // {
        //   "choices": [
        //     {
        //       "message": {
        //         "content": "texto da resposta aqui"
        //       }
        //     }
        //   ]
        // }
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement // aqui ele começa na raiz do projeto
                  .GetProperty("choices")[0]  // pega o primeiro item do array choices
                  .GetProperty("message")  // entra no objeto message
                  .GetProperty("content")    // pega o campo content
                  .GetString() ?? "Sem resposta."; // converte para string
    }
}
