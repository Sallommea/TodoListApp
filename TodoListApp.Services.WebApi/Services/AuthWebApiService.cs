using System.Net.Http;
using System.Net.Http.Json;
using TodoListApp.WebApi.Models.Auth;

namespace TodoListApp.Services.WebApi.Services;
public class AuthWebApiService
{
    private readonly HttpClient httpClient;
    public AuthWebApiService(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<AuthResult> LoginAsync(LoginModel model)
    {
        var response = await this.httpClient.PostAsJsonAsync("/api/auth/login", model);
        if (response.IsSuccessStatusCode)
        {
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiAuthResponse>();
            return new AuthResult
            {
                Success = true,
                Token = apiResponse?.Token ?? string.Empty,
                Message = apiResponse?.Message ?? "Login successful",
            };
        }

        return new AuthResult { Success = false, Message = "Login failed" };
    }

    public async Task<AuthResult> RegisterAsync(RegisterModel model)
    {
        var response = await this.httpClient.PostAsJsonAsync("/api/auth/register", model);
        if (response.IsSuccessStatusCode)
        {
            var apiResponse = await response.Content.ReadFromJsonAsync<ApiAuthResponse>();
            return new AuthResult
            {
                Success = true,
                Token = apiResponse?.Token ?? string.Empty,
                Message = apiResponse?.Message ?? "Registration successful",
            };
        }

        return new AuthResult { Success = false, Message = "Registration failed" };
    }
}
