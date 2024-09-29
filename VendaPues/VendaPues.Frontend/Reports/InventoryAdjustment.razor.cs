using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using OfficeOpenXml;
using VendaPues.Frontend.Repositories;
using VendaPues.Shared.Entities;

namespace VendaPues.Frontend.Reports
{
    public partial class InventoryAdjustment
    {
        private List<Inventory>? inventories;
        private Inventory selectedInventory = new();
        private Inventory? inventory;
        private bool loading;
        private bool showReport;
        private float totalQuantity = 0;
        private decimal totalValue = 0;

        [Inject] private IDialogService DialogService { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private IJSRuntime JS { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            await LoadInventoriesAsync();
        }

        private async Task LoadInventoriesAsync()
        {
            loading = true;
            var responseHttp = await Repository.GetAsync<List<Inventory>>($"/api/inventories/combo");
            loading = false;

            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message!, Severity.Error);
                return;
            }
            inventories = responseHttp.Response;
        }

        private async Task<IEnumerable<Inventory>> SearchSupplierAsync(string searchText, CancellationToken cancellationToken)
        {
            await Task.Delay(5);
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return inventories!;
            }

            return inventories!
                .Where(x => x.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }

        private void SuplierChanged(Inventory inventory)
        {
            selectedInventory = inventory;
        }

        private async Task GenerateReportAsync()
        {
            if (selectedInventory.Id == 0)
            {
                Snackbar.Add("Debes seleccionar un inventario.", Severity.Error);
                return;
            }

            var responseHttp = await Repository.GetAsync<Inventory>($"/api/inventories/{selectedInventory.Id}");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message!, Severity.Error);
                return;
            }

            inventory = responseHttp.Response;
            inventory!.InventoryDetails = inventory.InventoryDetails!
                .Where(x => x.Adjustment != 0)
                .ToList();

            totalQuantity = inventory!.InventoryDetails!.Sum(x => x.Adjustment);
            totalValue = inventory!.InventoryDetails!.Sum(x => x.AdjustmentValue);
            showReport = true;
        }

        private void ExportToExcel()
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Ajuste de Inventario");

            worksheet.Cells[1, 1].Value = "Producto";
            worksheet.Cells[1, 2].Value = "Costo";
            worksheet.Cells[1, 3].Value = "Stock";
            worksheet.Cells[1, 4].Value = "Ajuste";
            worksheet.Cells[1, 5].Value = "Valor ajuste";

            for (int i = 0; i < inventory!.InventoryDetails!.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = inventory.InventoryDetails.ElementAt(i).Product!.Name;
                worksheet.Cells[i + 2, 2].Value = inventory.InventoryDetails.ElementAt(i).Cost;
                worksheet.Cells[i + 2, 3].Value = inventory.InventoryDetails.ElementAt(i).Stock;
                worksheet.Cells[i + 2, 4].Value = inventory.InventoryDetails.ElementAt(i).Adjustment;
                worksheet.Cells[i + 2, 5].Value = inventory.InventoryDetails.ElementAt(i).AdjustmentValue;
            }

            var stream = new MemoryStream(package.GetAsByteArray());
            JS.InvokeVoidAsync("BlazorDownloadFile", "Ajuste de inventario.xlsx", Convert.ToBase64String(stream.ToArray()));
        }
    }
}