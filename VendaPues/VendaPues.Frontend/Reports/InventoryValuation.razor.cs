using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using OfficeOpenXml;
using VendaPues.Frontend.Repositories;
using VendaPues.Shared.Entities;

namespace VendaPues.Frontend.Reports
{
    public partial class InventoryValuation
    {
        private bool loading;
        private const string baseUrl = "api/products";

        [Inject] private IJSRuntime JS { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;

        private List<Product>? products;
        private float totalQuantiy = 0;
        private decimal totalValueCost = 0;
        private decimal totalValuePrice = 0;

        protected override async Task OnInitializedAsync()
        {
            await LoadProductsAsync();
        }

        private async Task LoadProductsAsync()
        {
            loading = true;
            var url = $"{baseUrl}?page=1&recordsnumber={int.MaxValue}";
            var responseHttp = await Repository.GetAsync<List<Product>>(url);
            loading = false;

            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message, Severity.Error);
                return;
            }

            products = responseHttp.Response;
            totalQuantiy = products!.Sum(x => x.Stock);
            totalValueCost = products!.Sum(x => x.CostValue);
            totalValuePrice = products!.Sum(x => x.PriceValue);
        }

        private void ExportToExcel()
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Reporte de Inventario");

            worksheet.Cells[1, 1].Value = "Producto";
            worksheet.Cells[1, 2].Value = "Precio";
            worksheet.Cells[1, 3].Value = "Costo";
            worksheet.Cells[1, 4].Value = "Cantidad";
            worksheet.Cells[1, 5].Value = "Valor a costo";
            worksheet.Cells[1, 6].Value = "Valor a precio";

            for (int i = 0; i < products!.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = products[i].Name;
                worksheet.Cells[i + 2, 2].Value = products[i].Price;
                worksheet.Cells[i + 2, 3].Value = products[i].Cost;
                worksheet.Cells[i + 2, 4].Value = products[i].Stock;
                worksheet.Cells[i + 2, 5].Value = products[i].CostValue;
                worksheet.Cells[i + 2, 6].Value = products[i].PriceValue;
            }

            var stream = new MemoryStream(package.GetAsByteArray());
            JS.InvokeVoidAsync("BlazorDownloadFile", "Valoración de inventario.xlsx", Convert.ToBase64String(stream.ToArray()));
        }
    }
}