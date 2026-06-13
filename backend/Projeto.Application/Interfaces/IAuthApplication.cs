using Projeto.Application.DTOs.Auth.Request;
using Projeto.Application.DTOs.Auth.Response;

namespace Projeto.Application.Interfaces;

public interface IAuthApplication
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}
