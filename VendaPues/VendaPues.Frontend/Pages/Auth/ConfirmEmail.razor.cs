using Microsoft.AspNetCore.Components;
using MudBlazor;
using VendaPues.Frontend.Repositories;

namespace VendaPues.Frontend.Pages.Auth
{
    public partial class ConfirmEmail
    {
        private string? message;

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private IDialogService DialogService { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;

        [Parameter, SupplyParameterFromQuery] public string UserId { get; set; } = string.Empty;
        [Parameter, SupplyParameterFromQuery] public string Token { get; set; } = string.Empty;

        protected async Task ConfirmAccountAsync()
        {
            var responseHttp = await Repository.GetAsync($"/api/accounts/ConfirmEmail/?userId={UserId}&token={Token}");
            if (responseHttp.Error)
            {
                message = await responseHttp.GetErrorMessageAsync();
                NavigationManager.NavigateTo("/");
                Snackbar.Add(message, Severity.Error);
                return;
            }

            Snackbar.Add("Gracias por confirmar su email, ahora puedes ingresar al sistema.", Severity.Success);
            var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true };
            DialogService.Show<Login>("Inicio de Sesion", closeOnEscapeKey);
        }
    }
}