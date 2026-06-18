using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Projeto.Services.Interfaces;

namespace Projeto.Services.AI;

public class AIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public AIService(IConfiguration config)
    {
        _httpClient = new HttpClient();
        _config = config;
    }

    public async Task<string> GetAiResponseAsync(string prompt)
    {
        var url   = _config["GitHubModels:ApiUrl"];
        var token = _config["GitHubModels:Token"];

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("VortexApp");

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

        var response = await _httpClient.PostAsync(url, body);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            return $"Erro: {response.StatusCode} - {error}";
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement
                  .GetProperty("choices")[0]
                  .GetProperty("message")
                  .GetProperty("content")
                  .GetString() ?? "Sem resposta.";
    }
}
