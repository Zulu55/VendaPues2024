using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using OfficeOpenXml;
using VendaPues.Frontend.Helpers;
using VendaPues.Frontend.Repositories;
using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;

namespace VendaPues.Frontend.Reports
{
    public partial class GrossProfit
    {
        private bool loading;
        private bool showReport;
        private float totalQuantity = 0;
        private decimal totalValue = 0;
        private decimal totalProfit = 0;
        private DateTime initialDate = DateTime.Today;
        private DateTime finalDate = DateTime.Today.AddDays(1).AddMilliseconds(-1);
        private List<Order>? orders;
        private List<GrossProfitReportDTO>? reports;

        [Inject] private IDialogService DialogService { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private IJSRuntime JS { get; set; } = null!;

        private async Task OnInitialDateChange(DateTime? date)
        {
            await Task.Delay(1);
            if (date == null)
            {
                return;
            }
            initialDate = (DateTime)date;
        }

        private async Task OnFinalDateChange(DateTime? date)
        {
            await Task.Delay(1);
            if (date == null)
            {
                return;
            }
            finalDate = (DateTime)date;
        }

        private async Task GenerateReportAsync()
        {
            if (initialDate > finalDate)
            {
                Snackbar.Add("La fecha inicial no puede ser superior a la fecha final.", Severity.Error);
                return;
            }

            var datesDTO = new DatesDTO
            {
                InitialDate = initialDate,
                FinalDate = finalDate,
            };

            loading = true;
            var responseHttp = await Repository.PostAsync<DatesDTO, List<Order>>($"/api/orders/report", datesDTO);
            loading = false;

            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message!, Severity.Error);
                return;
            }

            orders = responseHttp.Response;
            reports = [];
            foreach (var order in orders!)
            {
                foreach (var orderDetail in order.OrderDetails!)
                {
                    reports.Add(new GrossProfitReportDTO
                    {
                        Id = order.Id,
                        Date = order.Date,
                        User = order.User,
                        OrderType = order.OrderType,
                        OrderStatus = order.OrderStatus,
                        Product = orderDetail.Product,
                        Name = orderDetail.Name,
                        Price = orderDetail.Price,
                        Quantity = orderDetail.Quantity,
                    });
                }
            }

            totalQuantity = reports!.Sum(x => x.Quantity);
            totalValue = reports!.Sum(x => x.Value);
            totalProfit = reports!.Sum(x => x.Profit);
            showReport = true;
        }

        private void ExportToExcel()
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Utilidad bruta");

            worksheet.Cells[1, 1].Value = "Fecha";
            worksheet.Cells[1, 2].Value = "Usuario";
            worksheet.Cells[1, 3].Value = "Status";
            worksheet.Cells[1, 4].Value = "Tipo";
            worksheet.Cells[1, 5].Value = "Producto";
            worksheet.Cells[1, 6].Value = "Costo";
            worksheet.Cells[1, 7].Value = "Cantidad";
            worksheet.Cells[1, 8].Value = "Valor";
            worksheet.Cells[1, 9].Value = "Utilidad";

            for (int i = 0; i < reports!.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = reports.ElementAt(i).Date;
                worksheet.Cells[i + 2, 2].Value = reports.ElementAt(i).User!.FullName;
                worksheet.Cells[i + 2, 3].Value = EnumHelper.GetEnumDescription(reports.ElementAt(i).OrderStatus);
                worksheet.Cells[i + 2, 4].Value = EnumHelper.GetEnumDescription(reports.ElementAt(i).OrderType);
                worksheet.Cells[i + 2, 5].Value = reports.ElementAt(i).Name;
                worksheet.Cells[i + 2, 6].Value = reports.ElementAt(i).Product!.Cost;
                worksheet.Cells[i + 2, 7].Value = reports.ElementAt(i).Quantity;
                worksheet.Cells[i + 2, 8].Value = reports.ElementAt(i).Value;
                worksheet.Cells[i + 2, 9].Value = reports.ElementAt(i).Profit;
            }

            var stream = new MemoryStream(package.GetAsByteArray());
            JS.InvokeVoidAsync("BlazorDownloadFile", "Utilidad bruta.xlsx", Convert.ToBase64String(stream.ToArray()));
        }
    }
}