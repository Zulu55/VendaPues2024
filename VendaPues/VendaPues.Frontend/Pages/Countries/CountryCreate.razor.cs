using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using VendaPues.Frontend.Repositories;
using VendaPues.Frontend.Shared;
using VendaPues.Shared.Entities;

namespace VendaPues.Frontend.Pages.Countries
{
    [Authorize(Roles = "Admin")]
    public partial class CountryCreate
    {
        private Country country = new();
        private FormWithName<Country>? countryForm;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private IDialogService DialogService { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;

        private async Task CreateAsync()
        {
            var responseHttp = await Repository.PostAsync("/api/countries", country);
            if (responseHttp.Error)
            {
                MudDialog.Close(DialogResult.Cancel());
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message, Severity.Error);
                return;
            }

            MudDialog.Close(DialogResult.Ok(true));
            countryForm!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo("/countries");
            Snackbar.Add("Registro creado con éxito.", Severity.Success);
        }

        private void Return()
        {
            MudDialog.Close(DialogResult.Cancel());
            countryForm!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo("/countries");
        }
    }
}