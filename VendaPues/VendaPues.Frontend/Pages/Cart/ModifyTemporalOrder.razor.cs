using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using VendaPues.Frontend.Repositories;
using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;

namespace VendaPues.Frontend.Pages.Cart
{
    [Authorize(Roles = "Admin, User")]
    public partial class ModifyTemporalOrder
    {
        private List<string>? categories;
        private List<string>? images;
        private bool loading = true;
        private Product? product;
        private TemporalOrderDTO? temporalOrderDTO;

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private IDialogService DialogService { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;
        [Parameter] public int TemporalOrderId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadTemporalOrderAsync();
        }

        private async Task LoadTemporalOrderAsync()
        {
            loading = true;
            var httpResponse = await Repository.GetAsync<TemporalOrder>($"/api/temporalOrders/{TemporalOrderId}");

            if (httpResponse.Error)
            {
                var message = await httpResponse.GetErrorMessageAsync();
                Snackbar.Add(message, Severity.Error);
                return;
            }

            var temporalOrder = httpResponse.Response!;
            temporalOrderDTO = new TemporalOrderDTO
            {
                Id = temporalOrder.Id,
                ProductId = temporalOrder.ProductId,
                Remarks = temporalOrder.Remarks!,
                Quantity = temporalOrder.Quantity
            };
            product = temporalOrder.Product;
            categories = product!.ProductCategories!.Select(x => x.Category.Name).ToList();
            images = product.ProductImages!.Select(x => x.Image).ToList();
            loading = false;
        }

        public async Task UpdateCartAsync()
        {
            var httpResponse = await Repository.PutAsync("/api/temporalOrders/full", temporalOrderDTO);
            if (httpResponse.Error)
            {
                var message = await httpResponse.GetErrorMessageAsync();
                Snackbar.Add(message, Severity.Error);
                return;
            }
            NavigationManager.NavigateTo("/");
            Snackbar.Add("Producto modificado en el carro de compras.", Severity.Success);
        }
    }
}