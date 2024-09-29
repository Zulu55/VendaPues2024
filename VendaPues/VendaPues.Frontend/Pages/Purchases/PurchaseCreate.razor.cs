using Microsoft.AspNetCore.Components;
using MudBlazor;
using VendaPues.Frontend.Repositories;
using VendaPues.Frontend.Shared;
using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;

namespace VendaPues.Frontend.Pages.Purchases
{
    public partial class PurchaseCreate
    {
        private TemporalPurchaseDTO temporalPurchaseDTO = new() { Date = DateTime.Now };
        private List<Supplier>? suppliers;
        private Supplier selectedSupplier = new();
        private List<Product>? products;
        private Product selectedProduct = new();
        private MudTable<TemporalPurchase> table = new();
        private List<TemporalPurchase> temporalPurchases = new();
        private float sumQuantity;
        private decimal sumValue;
        private const string baseUrl = "api/temporalPurchases";
        private readonly int[] pageSizeOptions = { 5, 10, 20, int.MaxValue };
        private int totalRecords = 0;
        private bool loading;
        private string infoFormat = "{first_item}-{last_item} de {all_items}";

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private IDialogService DialogService { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;

        public List<TemporalPurchase>? TemporalPurchases { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadSuppliersAsync();
            await LoadPrductsAsync();
            await LoadTemporalPurchasesAsync();
        }

        private async Task LoadTemporalPurchasesAsync()
        {
            var responseHttp = await Repository.GetAsync<List<TemporalPurchase>>($"/api/TemporalPurchases/my");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message!, Severity.Error);
                return;
            }

            foreach (var item in responseHttp.Response!)
            {
                temporalPurchases.Add(new TemporalPurchase
                {
                    Cost = item.Cost,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Remarks = item.Remarks,
                });
            }
        }

        private async Task LoadPrductsAsync()
        {
            var responseHttp = await Repository.GetAsync<List<Product>>($"/api/products/combo");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message!, Severity.Error);
                return;
            }
            products = responseHttp.Response;
        }

        private async Task<TableData<TemporalPurchase>> LoadListAsync(TableState state, CancellationToken cancellationToken)
        {
            var url = $"{baseUrl}/my";
            var responseHttp = await Repository.GetAsync<List<TemporalPurchase>>(url);

            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message!, Severity.Error);
                return new TableData<TemporalPurchase> { Items = [], TotalItems = 0 };
            }
            if (responseHttp.Response == null)
            {
                return new TableData<TemporalPurchase> { Items = [], TotalItems = 0 };
            }

            sumQuantity = responseHttp.Response.Sum(x => x.Quantity);
            sumValue = responseHttp.Response.Sum(x => x.Value);
            await InvokeAsync(StateHasChanged);

            return new TableData<TemporalPurchase>
            {
                Items = responseHttp.Response,
                TotalItems = responseHttp.Response.Count
            };
        }

        private async Task DeleteAsync(int temporalPurchaseId)
        {
            var parameters = new DialogParameters
            {
                { "Message", "¿Esta seguro que quieres borrar el registro?" }
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
            var dialog = DialogService.Show<ConfirmDialog>("Confirmación", parameters, options);
            var result = await dialog.Result;
            if (result.Canceled)
            {
                return;
            }

            var responseHttp = await Repository.DeleteAsync<TemporalPurchase>($"api/temporalPurchases/{temporalPurchaseId}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/");
                    return;
                }

                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message!, Severity.Error);
                return;
            }

            await table.ReloadServerData();
            Snackbar.Add("Producto eliminado de la compra.", Severity.Success);
        }

        private async Task OnDateChange(DateTime? date)
        {
            await Task.Delay(1);
            if (date == null)
            {
                return;
            }
            temporalPurchaseDTO.Date = (DateTime)date;
        }

        private async Task AddProductAsync()
        {
            if (selectedProduct.Id == 0)
            {
                Snackbar.Add("Debes seleccionar un producto.", Severity.Error);
                return;
            }

            if (temporalPurchaseDTO.Quantity <= 0)
            {
                Snackbar.Add("Debes ingresar una cantidad mayor que cero.", Severity.Error);
                return;
            }

            if (temporalPurchaseDTO.Cost <= 0)
            {
                Snackbar.Add("Debes ingresar un costo mayor que cero.", Severity.Error);
                return;
            }

            var responseHttp = await Repository.PostAsync<TemporalPurchaseDTO>("/api/TemporalPurchases/full", temporalPurchaseDTO);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message!, Severity.Error);
                return;
            }

            temporalPurchases.Add(new TemporalPurchase
            {
                Cost = temporalPurchaseDTO.Cost,
                ProductId = temporalPurchaseDTO.ProductId,
                Quantity = temporalPurchaseDTO.Quantity,
                Remarks = temporalPurchaseDTO.RemarksDetail,
            });

            selectedProduct = new Product();
            temporalPurchaseDTO.Quantity = 1;
            temporalPurchaseDTO.Cost = 0;
            await table.ReloadServerData();
            Snackbar.Add("Producto agregado a la compra.", Severity.Success);
        }

        private async Task SavePurchaseAsync()
        {
            if (selectedSupplier.Id == 0)
            {
                Snackbar.Add("Debes seleccionar un proveedor.", Severity.Error);
                return;
            }

            if (sumQuantity <= 0)
            {
                Snackbar.Add("Debes agregar al menos un producto en la compra.", Severity.Error);
                return;
            }

            var parameters = new DialogParameters
            {
                { "Message", "¿Esta seguro que quieres registrar esta compra?" }
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
            var dialog = DialogService.Show<ConfirmDialog>("Confirmación", parameters, options);
            var result = await dialog.Result;
            if (result.Canceled)
            {
                return;
            }

            var purchaseDTO = new PurchaseDTO
            {
                Date = temporalPurchaseDTO.Date.ToUniversalTime(),
                Remarks = temporalPurchaseDTO.RemarksGeneral,
                SupplierId = temporalPurchaseDTO.SupplierId,
                PurchaseDetails = []
            };

            foreach (var item in temporalPurchases)
            {
                purchaseDTO.PurchaseDetails.Add(new PurchaseDetailDTO
                {
                    Cost = item.Cost,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Remarks = item.Remarks
                });
            }

            var responseHttp = await Repository.PostAsync<PurchaseDTO>("/api/purchases/full", purchaseDTO);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message!, Severity.Error);
                return;
            }

            selectedSupplier = new Supplier();
            temporalPurchases.Clear();
            temporalPurchaseDTO.RemarksGeneral = string.Empty;
            MudDialog.Close(DialogResult.Ok(true));
            NavigationManager.NavigateTo("/purchases");
            Snackbar.Add("Compra agregada con exito.", Severity.Success);
        }

        private void SuplierChanged(Supplier supplier)
        {
            selectedSupplier = supplier;
            temporalPurchaseDTO.SupplierId = supplier.Id;
        }

        private void ProductChanged(Product product)
        {
            selectedProduct = product;
            temporalPurchaseDTO.ProductId = product.Id;
            temporalPurchaseDTO.Cost = product.Cost;
            temporalPurchaseDTO.Quantity = 1;
        }

        private async Task LoadSuppliersAsync()
        {
            var responseHttp = await Repository.GetAsync<List<Supplier>>($"/api/suppliers/combo");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message!, Severity.Error);
                return;
            }
            suppliers = responseHttp.Response;
        }

        private async Task<IEnumerable<Supplier>> SearchSupplierAsync(string searchText, CancellationToken cancellationToken)
        {
            await Task.Delay(5);
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return suppliers!;
            }

            return suppliers!
                .Where(x => x.SupplierName.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }

        private async Task<IEnumerable<Product>> SearchProductAsync(string searchText, CancellationToken cancellationToken)
        {
            await Task.Delay(5);
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return products!;
            }

            return products!
                .Where(x => x.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }
    }
}