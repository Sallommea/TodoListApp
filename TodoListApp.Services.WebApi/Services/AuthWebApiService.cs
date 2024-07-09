using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using TodoListApp.Services.WebApi.Logging;
using TodoListApp.WebApi.Models.Auth;

namespace TodoListApp.Services.WebApi.Services;
public class AuthWebApiService
{
    private readonly HttpClient httpClient;
    private readonly ILogger<AuthWebApiService> logger;

    public AuthWebApiService(HttpClient httpClient, ILogger<AuthWebApiService> logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
    }

    public async Task<AuthResult> LoginAsync(LoginModel model)
    {
        try
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
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return new AuthResult { Success = false, Message = "Invalid email or password." };
            }
            else
            {
                _ = response.EnsureSuccessStatusCode();
            }

            return new AuthResult { Success = false, Message = "Login failed" };
        }
        catch (HttpRequestException ex)
        {
            AuthServiceLoggerMessages.HTTPErrorWhileSigninIn(this.logger, ex.Message, ex);
            return new AuthResult { Success = false, Message = "Unable to connect to the server. Please try again later." };
        }
        catch (Exception ex)
        {
            AuthServiceLoggerMessages.ErrorSigninIn(this.logger, ex.Message, ex);
            throw;
        }
    }

    public async Task<AuthResult> RegisterAsync(RegisterModel model)
    {
        try
        {
            var response = await this.httpClient.PostAsJsonAsync("/api/auth/register", model);

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                var errorResult = await response.Content.ReadFromJsonAsync<AuthResult>();
                return new AuthResult
                {
                    Success = false,
                    Message = errorResult?.Message ?? "Email already in use",
                };
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<AuthResult>();
            return apiResponse ?? new AuthResult
            {
                Success = true,
                Token = string.Empty,
                Message = "Registration successful",
            };
        }
        catch (HttpRequestException ex)
        {
            AuthServiceLoggerMessages.HTTPErrorWhileRegistering(this.logger, ex.Message, ex);
            return new AuthResult { Success = false, Message = "Unable to connect to the server. Please try again later." };
        }
        catch (Exception ex)
        {
            AuthServiceLoggerMessages.ErrorWhileRegistering(this.logger, ex.Message, ex);
            throw;
        }
    }
}
