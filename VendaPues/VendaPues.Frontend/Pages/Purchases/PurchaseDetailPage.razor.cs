using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using VendaPues.Frontend.Repositories;
using VendaPues.Shared.Entities;

namespace VendaPues.Frontend.Pages.Purchases
{
    [Authorize(Roles = "Admin")]
    public partial class PurchaseDetailPage
    {
        public List<PurchaseDetail>? PurchaseDetails { get; set; }
        public Purchase? Purchase { get; set; }

        private MudTable<PurchaseDetail> table = new();
        private readonly int[] pageSizeOptions = { 5, 10 };
        private int totalRecords = 0;
        private bool loading;
        private const string baseUrl = "api/purchaseDetails";
        private string infoFormat = "{first_item}-{last_item} de {all_items}";

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        [Parameter] public int PurchaseId { get; set; }
        [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            await LoadTotalRecordsAsync();
            await LoadPurchaseAsync();
        }

        private async Task LoadPurchaseAsync()
        {
            loading = true;
            var url = $"api/purchases/{PurchaseId}";
            var responseHttp = await Repository.GetAsync<Purchase>(url);
            loading = false;

            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/purchase");
                }
                else
                {
                    var messsage = await responseHttp.GetErrorMessageAsync();
                    Snackbar.Add(messsage, Severity.Error);
                }
            }
            else
            {
                Purchase = responseHttp.Response;
            }
        }

        private async Task<bool> LoadTotalRecordsAsync()
        {
            loading = true;
            var url = $"{baseUrl}/recordsnumber?page=1&recordsnumber={int.MaxValue}&id={PurchaseId}";
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                url += $"&filter={Filter}";
            }
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

        private async Task<TableData<PurchaseDetail>> LoadListAsync(TableState state, CancellationToken cancellationToken)
        {
            int page = state.Page + 1;
            int pageSize = state.PageSize;
            var url = $"{baseUrl}?page={page}&recordsnumber={pageSize}&id={PurchaseId}";

            if (!string.IsNullOrWhiteSpace(Filter))
            {
                url += $"&filter={Filter}";
            }

            var responseHttp = await Repository.GetAsync<List<PurchaseDetail>>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message, Severity.Error);
                return new TableData<PurchaseDetail> { Items = [], TotalItems = 0 };
            }
            if (responseHttp.Response == null)
            {
                return new TableData<PurchaseDetail> { Items = [], TotalItems = 0 };
            }
            return new TableData<PurchaseDetail>
            {
                Items = responseHttp.Response,
                TotalItems = totalRecords
            };
        }

        private async Task SetFilterValue(string value)
        {
            Filter = value;
            await LoadAsync();
            await table.ReloadServerData();
        }

        private static string TruncateContent(string content, int length)
        {
            if (content.Length > length)
            {
                return content.Substring(0, length) + "...";
            }
            return content;
        }
    }
}