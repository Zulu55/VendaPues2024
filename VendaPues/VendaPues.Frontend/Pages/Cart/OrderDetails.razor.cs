using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using VendaPues.Frontend.Repositories;
using VendaPues.Frontend.Shared;
using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;
using VendaPues.Shared.Enums;

namespace VendaPues.Frontend.Pages.Cart
{
    [Authorize(Roles = "Admin, User")]
    public partial class OrderDetails
    {
        private Order? order;
        public List<OrderDetail>? Details { get; set; }
        private MudTable<OrderDetail> table = new();
        private const string baseUrl = "api/orders";

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private IDialogService DialogService { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;
        [Parameter] public int OrderId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            var responseHppt = await Repository.GetAsync<Order>($"{baseUrl}/{OrderId}");
            if (responseHppt.Error)
            {
                if (responseHppt.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/orders");
                    return;
                }
                var messageError = await responseHppt.GetErrorMessageAsync();
                Snackbar.Add(messageError, Severity.Error);
                return;
            }
            order = responseHppt.Response;
            Details = order!.OrderDetails!.ToList();
        }

        private async Task<TableData<OrderDetail>> LoadListAsync(TableState state, CancellationToken cancellationToken)
        {
            await LoadAsync();
            return new TableData<OrderDetail>
            {
                Items = Details,
                TotalItems = Details!.Count
            };
        }

        private async Task CancelOrderAsync()
        {
            await ModifyTemporalOrder("cancelar", OrderStatus.Cancelled);
        }

        private async Task DispatchOrderAsync()
        {
            await ModifyTemporalOrder("despachar", OrderStatus.Dispatched);
        }

        private async Task SendOrderAsync()
        {
            await ModifyTemporalOrder("enviar", OrderStatus.Sent);
        }

        private async Task ConfirmOrderAsync()
        {
            await ModifyTemporalOrder("confirmar", OrderStatus.Confirmed);
        }

        private async Task ModifyTemporalOrder(string message, OrderStatus status)
        {
            var parameters = new DialogParameters
            {
                { "Message", $"¿Esta seguro que quieres {message} el pedido?" }
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
            var dialog = DialogService.Show<ConfirmDialog>("Confirmación", parameters, options);
            var result = await dialog.Result;
            if (result.Canceled)
            {
                return;
            }

            var orderDTO = new OrderDTO
            {
                Id = OrderId,
                OrderStatus = status,
                Email = "payondeliver@yopmail.com",
                Reference = "na"
            };

            var responseHttp = await Repository.PutAsync("api/orders", orderDTO);
            if (responseHttp.Error)
            {
                message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message, Severity.Error);
                return;
            }

            NavigationManager.NavigateTo("/orders");
        }
    }
}