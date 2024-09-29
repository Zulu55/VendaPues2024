using Microsoft.AspNetCore.Components;
using MudBlazor;
using VendaPues.Frontend.Repositories;
using VendaPues.Frontend.Services;
using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;
using VendaPues.Shared.Enums;

namespace VendaPues.Frontend.Pages.Auth
{
    public partial class Register
    {
        private UserDTO userDTO = new();
        private List<Country>? countries;
        private List<State>? states;
        private List<City>? cities;
        private bool loading;
        private string? imageUrl;
        private string? titleLabel;

        private Country selectedCountry = new();
        private State selectedState = new();
        private City selectedCity = new();

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private ILoginService LogInService { get; set; } = null!;
        [Inject] private IDialogService DialogService { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Parameter, SupplyParameterFromQuery] public bool IsAdmin { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadCountriesAsync();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            titleLabel = IsAdmin ? "Registro de Administrador" : "Registro de Usuario";
        }

        private void ImageSelected(string imageBase64)
        {
            userDTO.Photo = imageBase64;
            imageUrl = null;
        }

        private async Task LoadCountriesAsync()
        {
            var responseHttp = await Repository.GetAsync<List<Country>>("/api/countries/combo");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message, Severity.Error);
                return;
            }
            countries = responseHttp.Response;
        }

        private async Task LoadStatesAsyn(int countryId)
        {
            var responseHttp = await Repository.GetAsync<List<State>>($"/api/states/combo/{countryId}");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message, Severity.Error);
                return;
            }
            states = responseHttp.Response;
        }

        private async Task LoadCitiesAsyn(int stateId)
        {
            var responseHttp = await Repository.GetAsync<List<City>>($"/api/cities/combo/{stateId}");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message, Severity.Error);
                return;
            }
            cities = responseHttp.Response;
        }

        private async Task CountryChangedAsync(Country country)
        {
            selectedCountry = country;
            selectedState = new State();
            selectedCity = new City();
            states = null;
            cities = null;
            await LoadStatesAsyn(country.Id);
        }

        private async Task StateChangedAsync(State state)
        {
            selectedState = state;
            selectedCity = new City();
            cities = null;
            await LoadCitiesAsyn(state.Id);
        }

        private void CityChanged(City city)
        {
            selectedCity = city;
            userDTO.CityId = city.Id;
        }

        private async Task<IEnumerable<Country>> SearchCountries(string searchText, CancellationToken cancellationToken)
        {
            await Task.Delay(5);
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return countries!;
            }

            return countries!
                .Where(c => c.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }

        private async Task<IEnumerable<State>> SearchStates(string searchText, CancellationToken cancellationToken)
        {
            await Task.Delay(5);
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return states!;
            }

            return states!
                .Where(c => c.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }

        private async Task<IEnumerable<City>> SearchCity(string searchText, CancellationToken cancellationToken)
        {
            await Task.Delay(5);
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return cities!;
            }

            return cities!
                .Where(c => c.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase))
                .ToList();
        }

        private void ReturnAction()
        {
            NavigationManager.NavigateTo("/");
        }

        private async Task CreateUserAsync()
        {
            userDTO.UserType = UserType.User;
            userDTO.UserName = userDTO.Email;

            if (IsAdmin)
            {
                userDTO.UserType = UserType.Admin;
            }

            loading = true;
            var responseHttp = await Repository.PostAsync<UserDTO>("/api/accounts/CreateUser", userDTO);
            loading = false;
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message, Severity.Error);
                return;
            }

            Snackbar.Add("Su cuenta ha sido creada con exito. Se te ha enviado un correo electrónico con las instrucciones para activar tu usuario.", Severity.Success);
            NavigationManager.NavigateTo("/");
        }
    }
}