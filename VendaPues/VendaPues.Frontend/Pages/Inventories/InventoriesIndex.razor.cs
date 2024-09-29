using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using VendaPues.Frontend.Repositories;
using VendaPues.Shared.Entities;

namespace VendaPues.Frontend.Pages.Inventories
{
    [Authorize(Roles = "Admin")]
    public partial class InventoriesIndex
    {
        public List<Inventory>? Inventories { get; set; }

        private MudTable<Inventory> table = new();
        private readonly int[] pageSizeOptions = { 10, 25, 50, 5, int.MaxValue };
        private int totalRecords = 0;
        private bool loading;
        private const string baseUrl = "api/inventories";
        private string infoFormat = "{first_item}-{last_item} de {all_items}";

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private IDialogService DialogService { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            await LoadTotalRecords();
        }

        private async Task<bool> LoadTotalRecords()
        {
            loading = true;
            var url = $"{baseUrl}/recordsnumber?page=1&recordsnumber={int.MaxValue}";
            var responseHttp = await Repository.GetAsync<int>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message, Severity.Error);
                return false;
            }
            totalRecords = responseHttp.Response;
            loading = false;
            return true;
        }

        private async Task<TableData<Inventory>> LoadListAsync(TableState state, CancellationToken cancellationToken)
        {
            int page = state.Page + 1;
            int pageSize = state.PageSize;
            var url = $"{baseUrl}?page={page}&recordsnumber={pageSize}";
            var responseHttp = await Repository.GetAsync<List<Inventory>>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message, Severity.Error);
                return new TableData<Inventory> { Items = [], TotalItems = 0 };
            }
            if (responseHttp.Response == null)
            {
                return new TableData<Inventory> { Items = [], TotalItems = 0 };
            }
            return new TableData<Inventory>
            {
                Items = responseHttp.Response,
                TotalItems = totalRecords
            };
        }

        private async Task ShowCount1ModalAsync(Inventory inventory)
        {
            var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
            var parameters = new DialogParameters
            {
                { "Id", inventory.Id }
            };
            var dialog = DialogService.Show<EnterCount1>("Ingresar Conteo #1", parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                await LoadAsync();
                await table.ReloadServerData();
            }
        }

        private async Task ShowCount2ModalAsync(Inventory inventory)
        {
            if (!inventory.Count1Finish)
            {
                Snackbar.Add("Primero debes completar el conteo #1", Severity.Error);
                return;
            }

            var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
            var parameters = new DialogParameters
            {
                { "Id", inventory.Id }
            };
            var dialog = DialogService.Show<EnterCount2>("Ingresar Conteo #2", parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                await LoadAsync();
                await table.ReloadServerData();
            }
        }

        private async Task ShowCount3ModalAsync(Inventory inventory)
        {
            if (!inventory.Count2Finish || !inventory.Count2Finish)
            {
                Snackbar.Add("Primero debes completar el conteo #1 y #2", Severity.Error);
                return;
            }

            var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
            var parameters = new DialogParameters
            {
                { "Id", inventory.Id }
            };
            var dialog = DialogService.Show<EnterCount3>("Ingresar Conteo #3", parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                await LoadAsync();
                await table.ReloadServerData();
            }
        }

        private async Task ShowCreateModalAsync()
        {
            var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
            var dialog = DialogService.Show<InventoryCreate>("Crear Inventario", options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                await LoadAsync();
                await table.ReloadServerData();
            }
        }
    }
}