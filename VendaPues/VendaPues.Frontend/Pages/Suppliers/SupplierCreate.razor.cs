using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using VendaPues.Frontend.Repositories;
using VendaPues.Shared.Entities;

namespace VendaPues.Frontend.Pages.Suppliers
{
    [Authorize(Roles = "Admin")]
    public partial class SupplierCreate
    {
        private Supplier supplier = new();
        private SupplierForm? supplierForm;

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private IDialogService DialogService { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;

        private async Task CreateAsync()
        {
            var responseHttp = await Repository.PostAsync("/api/suppliers", supplier);
            if (responseHttp.Error)
            {
                MudDialog.Close(DialogResult.Cancel());
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message, Severity.Error);
                return;
            }

            MudDialog.Close(DialogResult.Ok(true));
            supplierForm!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo("/suppliers");
            Snackbar.Add("Registro creado con éxito.", Severity.Success);
        }

        private void Return()
        {
            MudDialog.Close(DialogResult.Cancel());
            supplierForm!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo("/suppliers");
        }
    }
}