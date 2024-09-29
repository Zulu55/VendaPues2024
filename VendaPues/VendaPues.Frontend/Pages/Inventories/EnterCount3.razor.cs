using Microsoft.AspNetCore.Components;
using MudBlazor;
using VendaPues.Frontend.Repositories;
using VendaPues.Frontend.Shared;
using VendaPues.Shared.Entities;
using VendaPues.Shared.Responses;

namespace VendaPues.Frontend.Pages.Inventories
{
    public partial class EnterCount3
    {
        private int totalRecords = 0;
        private bool loading;
        private const string baseUrl = "api/InventoryDetails";
        private readonly int[] pageSizeOptions = { 5, 10, 20, int.MaxValue };
        private bool enableModifyCost = false;
        private string infoFormat = "{first_item}-{last_item} de {all_items}";

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private IDialogService DialogService { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;

        [EditorRequired, Parameter] public int Id { get; set; }
        [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;

        private MudTable<InventoryDetail> table = new();
        public List<InventoryDetail>? InventoryDetails { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        private void OnEnableModifyCostChanged(bool value)
        {
            enableModifyCost = value;
        }

        private async Task LoadAsync()
        {
            await LoadTotalRecords();
        }

        private async Task<bool> LoadTotalRecords()
        {
            loading = true;
            var url = $"{baseUrl}/recordsNumberCount3?page=1&recordsnumber={int.MaxValue}&id={Id}";
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                url += $"&filter={Filter}";
            }

            var responseHttp = await Repository.GetAsync<int>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                var parameters = new DialogParameters
                {
                    { "Message", message }
                };
                var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
                DialogService.Show<GenericDialog>("Error", parameters, options);
                return false;
            }
            totalRecords = responseHttp.Response;
            loading = false;
            return true;
        }

        private async Task<TableData<InventoryDetail>> LoadListAsync(TableState state, CancellationToken cancellationToken)
        {
            int page = state.Page + 1;
            int pageSize = state.PageSize;
            var url = $"{baseUrl}/Count3?page={page}&recordsnumber={pageSize}&id={Id}";
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                url += $"&filter={Filter}";
            }

            var responseHttp = await Repository.GetAsync<List<InventoryDetail>>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                var parameters = new DialogParameters
                {
                    { "Message", message }
                };
                var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
                DialogService.Show<GenericDialog>("Error", parameters, options);
                return new TableData<InventoryDetail> { Items = [], TotalItems = 0 };
            }
            if (responseHttp.Response == null)
            {
                return new TableData<InventoryDetail> { Items = [], TotalItems = 0 };
            }
            InventoryDetails = responseHttp.Response;
            return new TableData<InventoryDetail>
            {
                Items = InventoryDetails,
                TotalItems = totalRecords
            };
        }

        private async Task SetFilterValue(string value)
        {
            Filter = value;
            await LoadAsync();
            await table.ReloadServerData();
        }

        private void SaveCount()
        {
            if (InventoryDetails == null) return;

            var isValid = ValidateCount();
            if (!isValid.WasSuccess)
            {
                Snackbar.Add(isValid.Message!, Severity.Error);
                return;
            }

            foreach (var inventoryDetail in InventoryDetails!)
            {
                _ = SaveInventoryDetailAsync(inventoryDetail);
            }
            Snackbar.Add("Cambios guardados con exito.", Severity.Success);
        }

        private ActionResponse<bool> ValidateCount()
        {
            foreach (var inventoryDetail in InventoryDetails!)
            {
                if (inventoryDetail.Count3 < 0 || inventoryDetail.Cost < 0)
                {
                    return new ActionResponse<bool>
                    {
                        WasSuccess = false,
                        Message = $"El producto: {inventoryDetail.Product!.Name}, tiene costo: {inventoryDetail.Cost} y conteo: {inventoryDetail.Count3}. No son permitidos valores negativos."
                    };
                }

                if (inventoryDetail.Count3 != 0 && inventoryDetail.Cost <= 0)
                {
                    return new ActionResponse<bool>
                    {
                        WasSuccess = false,
                        Message = $"El producto: {inventoryDetail.Product!.Name}, tiene costo 0 y una cantidad ingresada el el conteo de: {inventoryDetail.Count3}, debe ingresar un costo si ingresa conteo."
                    };
                }
            }

            return new ActionResponse<bool> { WasSuccess = true };
        }

        private async Task SaveInventoryDetailAsync(InventoryDetail inventoryDetail)
        {
            var responseHttp = await Repository.PutAsync(baseUrl, inventoryDetail);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                var parameters = new DialogParameters
                {
                    { "Message", message }
                };
                var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
                DialogService.Show<GenericDialog>("Error", parameters, options);
                return;
            }
        }

        private async Task FinishCountAsync()
        {
            var parameters = new DialogParameters
            {
                { "Message", "¿Esta seguro que quieres finalizar el conteo #3?" }
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
            var dialog = DialogService.Show<ConfirmDialog>("Confirmación", parameters, options);
            var result = await dialog.Result;
            if (result.Canceled)
            {
                return;
            }

            if (InventoryDetails!.Count > 0)
            {
                var count0 = InventoryDetails!.Count(x => x.Count3 == 0);
                if (count0 / InventoryDetails!.Count > 0.5)
                {
                    parameters = new DialogParameters
                    {
                        { "Message", "Hay una gran cantidad de productos con conteo en cero, ¿Estas seguro de cerrar este primer conteo?" }
                    };
                    options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall };
                    dialog = DialogService.Show<ConfirmDialog>("Confirmación", parameters, options);
                    result = await dialog.Result;
                    if (result.Canceled)
                    {
                        return;
                    }
                }
            }

            var responseHttp = await Repository.GetAsync($"/api/inventories/finishCount3/{Id}");
            if (responseHttp.Error)
            {
                MudDialog.Close(DialogResult.Cancel());
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message!, Severity.Error);
                return;
            }

            MudDialog.Close(DialogResult.Ok(true));
            Snackbar.Add("Conteo #3 cerrado. Proceso de inventario físico finalizado.", Severity.Success);
            NavigationManager.NavigateTo("/inventories");
        }
    }
}