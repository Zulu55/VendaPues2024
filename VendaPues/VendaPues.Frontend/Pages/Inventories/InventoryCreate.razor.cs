using Microsoft.AspNetCore.Components;
using MudBlazor;
using VendaPues.Frontend.Repositories;
using VendaPues.Frontend.Shared;
using VendaPues.Shared.Entities;

namespace VendaPues.Frontend.Pages.Inventories
{
    public partial class InventoryCreate
    {
        private Inventory inventory = new() { Date = DateTime.Now };

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        [Inject] private IDialogService DialogService { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;

        [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;

        private async Task OnDateChange(DateTime? date)
        {
            await Task.Delay(1);
            if (date == null)
            {
                return;
            }
            inventory.Date = (DateTime)date;
        }

        private async Task SaveInventoryAsync()
        {
            if (string.IsNullOrEmpty(inventory.Name))
            {
                Snackbar.Add("Debes ingresar un nombre al inventario.", Severity.Error);
                return;
            }

            if (string.IsNullOrEmpty(inventory.Description))
            {
                Snackbar.Add("Debes ingresar una descripción al inventario.", Severity.Error);
                return;
            }

            var parameters = new DialogParameters
            {
                { "Message", "¿Esta seguro que quieres crear este nuevo inventario?" }
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
            var dialog = DialogService.Show<ConfirmDialog>("Confirmación", parameters, options);
            var result = await dialog.Result;
            if (result.Canceled)
            {
                return;
            }

            var responseHttp = await Repository.PostAsync<Inventory>("/api/inventories", inventory);
            if (responseHttp.Error)
            {
                MudDialog.Close(DialogResult.Cancel());
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message!, Severity.Error);
                return;
            }

            MudDialog.Close(DialogResult.Ok(true));
            Snackbar.Add("Inventario creado.", Severity.Success);
            NavigationManager.NavigateTo("/inventories");
        }
    }
}