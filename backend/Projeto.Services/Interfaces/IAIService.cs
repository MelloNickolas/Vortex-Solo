namespace Projeto.Services.Interfaces;

public interface IAIService
{
    Task<string> GetAiResponseAsync(string prompt);
}
