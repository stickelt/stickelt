using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlazorWebAssembly.Components;
using Blazored.LocalStorage;

namespace BlazorWebAssembly.Pages
{
    public partial class Login : ComponentBase
    {
        private LoginModel loginModel = new LoginModel();
        private string errorMessage;

        [Inject] private HttpClient HttpClient { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private ILocalStorageService LocalStorageService { get; set; }

        private async Task HandleLogin()
        {
            try
            {
                // Make the request to the API
                var response = await HttpClient.PostAsJsonAsync("https://localhost:7077/api/users/login", loginModel);
                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the response as JSON (assuming API returns { "token": "your-jwt-token" })
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    if (result != null && !string.IsNullOrEmpty(result.Token))
                    {
                        // Store the token in local storage
                        await LocalStorageService.SetItemAsync("authToken", result.Token);

                        // Attach the token to HttpClient for future requests
                        HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Token);

                        // Redirect to the home page or another protected page
                        NavigationManager.NavigateTo("/profile");
                    }
                    else
                    {
                        errorMessage = "Invalid token in response.";
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    errorMessage = $"Login failed: {response.StatusCode} - {errorContent}";
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Login failed: {ex.Message}";
            }
        }

        private class LoginResponse
        {
            public string Token { get; set; }
        }

    }

}