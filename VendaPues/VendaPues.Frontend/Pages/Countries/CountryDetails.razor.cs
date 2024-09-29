using System.Net;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using VendaPues.Frontend.Pages.States;
using VendaPues.Frontend.Repositories;
using VendaPues.Frontend.Shared;
using VendaPues.Shared.Entities;

namespace VendaPues.Frontend.Pages.Countries
{
    [Authorize(Roles = "Admin")]
    public partial class CountryDetails
    {
        private Country? country;
        private List<State>? states;

        private MudTable<State> table = new();
        private readonly int[] pageSizeOptions = { 10, 25, 50, 5, int.MaxValue };
        private int totalRecords = 0;
        private bool loading;
        private const string baseUrl = "api/states";
        private string infoFormat = "{first_item}-{last_item} de {all_items}";

        [Parameter] public int CountryId { get; set; }

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private IDialogService DialogService { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            await LoadTotalRecords();
        }

        private async Task<bool> LoadCountryAsync()
        {
            var responseHttp = await Repository.GetAsync<Country>($"/api/countries/{CountryId}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/countries");
                    return false;
                }

                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message, Severity.Error);
                return false;
            }
            country = responseHttp.Response;
            return true;
        }

        private async Task<bool> LoadTotalRecords()
        {
            loading = true;
            if (country is null)
            {
                var ok = await LoadCountryAsync();
                if (!ok)
                {
                    NoCountry();
                    return false;
                }
            }

            var url = $"{baseUrl}/recordsnumber?id={CountryId}&page=1&recordsnumber={int.MaxValue}";
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

        private async Task<TableData<State>> LoadListAsync(TableState state, CancellationToken cancellationToken)
        {
            int page = state.Page + 1;
            int pageSize = state.PageSize;
            var url = $"{baseUrl}?id={CountryId}&page={page}&recordsnumber={pageSize}";

            if (!string.IsNullOrWhiteSpace(Filter))
            {
                url += $"&filter={Filter}";
            }

            var responseHttp = await Repository.GetAsync<List<State>>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message, Severity.Error);
                return new TableData<State> { Items = [], TotalItems = 0 };
            }
            if (responseHttp.Response == null)
            {
                return new TableData<State> { Items = [], TotalItems = 0 };
            }
            return new TableData<State>
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

        private void ReturnAction()
        {
            NavigationManager.NavigateTo("/countries");
        }

        private async Task ShowModalAsync(int id = 0, bool isEdit = false)
        {
            var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
            IDialogReference? dialog;
            if (isEdit)
            {
                var parameters = new DialogParameters
                {
                    { "Id", id }
                };
                dialog = DialogService.Show<StateEdit>("Editar Departamento/Estado", parameters, options);
            }
            else
            {
                var parameters = new DialogParameters
                {
                    { "CountryId", CountryId }
                };
                dialog = DialogService.Show<StateCreate>("Crear Departamento/Estado", parameters, options);
            }

            var result = await dialog.Result;
            if (!result.Canceled)
            {
                await LoadAsync();
                await table.ReloadServerData();
            }
        }

        private void CitiesAction(State state)
        {
            NavigationManager.NavigateTo($"/states/details/{state.Id}");
        }

        private void NoCountry()
        {
            NavigationManager.NavigateTo("/countries");
        }

        private async Task DeleteAsync(State state)
        {
            var parameters = new DialogParameters
            {
                { "Message", $"¿Estás seguro de que quieres eliminar el estado {state.Name}?" }
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
            var dialog = DialogService.Show<ConfirmDialog>("Confirmación", parameters, options);
            var result = await dialog.Result;
            if (result.Canceled)
            {
                return;
            }

            var responseHttp = await Repository.DeleteAsync<State>($"api/states/{state.Id}");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message, Severity.Error);
                return;
            }
            await LoadAsync();
            await table.ReloadServerData();
            Snackbar.Add("Estado eliminado.", Severity.Success);
        }
    }
}