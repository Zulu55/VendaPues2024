using System.Net.Mail;
using System.Security.AccessControl;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using VendaPues.Frontend.Repositories;
using VendaPues.Frontend.Shared;
using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;
using VendaPues.Shared.Enums;
using VendaPues.Shared.Responses;

namespace VendaPues.Frontend.Pages.Cart
{
    [Authorize(Roles = "Admin, User")]
    public partial class ShowCart
    {
        public List<TemporalOrder>? TemporalOrders { get; set; }
        private MudTable<TemporalOrder> table = new();
        private float sumQuantity;
        private decimal sumValue;
        private const string baseUrl = "api/temporalOrders";
        private Bank selectedBank = new();
        private List<Bank>? banks;
        private string email = null!;
        private bool loading;

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private IDialogService DialogService { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;

        public OrderDTO OrderDTO { get; set; } = new();
        private int selectedPaymentOption { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadBanksAsync();
        }

        private async Task LoadBanksAsync()
        {
            var responseHttp = await Repository.GetAsync<List<Bank>>($"/api/banks/combo");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message!, Severity.Error);
                return;
            }
            banks = responseHttp.Response;
        }

        private void BankChanged(Bank supplier)
        {
            selectedBank = supplier;
        }

        private async Task<IEnumerable<Bank>> SearchBankAsync(string searchText, CancellationToken cancellationToken)
        {
            await Task.Delay(5);
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return banks!;
            }

            return banks!
                .Where(x => x.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }

        private async Task<TableData<TemporalOrder>> LoadListAsync(TableState state, CancellationToken cancellationToken)
        {
            var url = $"{baseUrl}/my";
            var responseHttp = await Repository.GetAsync<List<TemporalOrder>>(url);

            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message, Severity.Error);
                return new TableData<TemporalOrder> { Items = [], TotalItems = 0 };
            }
            if (responseHttp.Response == null)
            {
                return new TableData<TemporalOrder> { Items = [], TotalItems = 0 };
            }

            sumQuantity = responseHttp.Response.Sum(x => x.Quantity);
            sumValue = responseHttp.Response.Sum(x => x.Value);
            await InvokeAsync(StateHasChanged);

            return new TableData<TemporalOrder>
            {
                Items = responseHttp.Response,
                TotalItems = responseHttp.Response.Count
            };
        }

        private async Task ConfirmOrderAsync()
        {
            if (selectedPaymentOption == 1)
            {
                if (selectedBank.Id == 0)
                {
                    Snackbar.Add("Debes seleccionar un banco.", Severity.Error);
                    return;
                }

                if (string.IsNullOrEmpty(email) || !IsValidEmail(email))
                {
                    Snackbar.Add("Debes ingresar un email válido.", Severity.Error);
                    return;
                }
            }

            var parameters = new DialogParameters
            {
                { "Message", "¿Esta seguro que quieres confirmar el pedido?" }
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
            var dialog = DialogService.Show<ConfirmDialog>("Confirmación", parameters, options);
            var result = await dialog.Result;
            if (result.Canceled)
            {
                return;
            }

            if (selectedPaymentOption == 1)
            {
                loading = true;
                await InvokeAsync(StateHasChanged);
                var paymentDTO = new PaymentDTO
                {
                    BankId = selectedBank.Id,
                    Email = email,
                    Value = sumValue
                };
                var httpActionPaymenyResponse = await Repository.PostAsync<PaymentDTO, ActionResponse<string>>("/api/payments", paymentDTO);
                var response = httpActionPaymenyResponse.Response;
                loading = false;

                if (!response!.WasSuccess)
                {
                    Snackbar.Add(response.Message, Severity.Error);
                    return;
                }

                Snackbar.Add(response.Message, Severity.Success);
                OrderDTO.Email = email;
                OrderDTO.BankId = selectedBank.Id;
                OrderDTO.Value = sumValue;
                OrderDTO.Reference = response.Result!;
            }

            if (selectedPaymentOption == 0)
            {
                OrderDTO.OrderType = OrderType.PaymentAgainstDelivery;
                OrderDTO.Email = "none@none.com";
                OrderDTO.Reference = "NA";
            }
            else
            {
                OrderDTO.OrderType = OrderType.PayOnLine;
            }

            var httpActionResponse = await Repository.PostAsync("/api/orders", OrderDTO);
            if (httpActionResponse.Error)
            {
                var message = await httpActionResponse.GetErrorMessageAsync();
                Snackbar.Add(message, Severity.Error);
                return;
            }

            NavigationManager.NavigateTo("/Cart/OrderConfirmed");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var mailAddress = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private async Task DeleteAsync(int temporalOrderId)
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

            var responseHttp = await Repository.DeleteAsync<TemporalOrder>($"api/temporalOrders/{temporalOrderId}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/");
                    return;
                }

                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message, Severity.Error);
                return;
            }

            await table.ReloadServerData();
            Snackbar.Add("Producto eliminado del carro de compras.", Severity.Success);
        }
    }
}