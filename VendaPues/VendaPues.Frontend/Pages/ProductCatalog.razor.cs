using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using VendaPues.Frontend.Pages.Auth;
using VendaPues.Frontend.Repositories;
using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;

namespace VendaPues.Frontend.Pages
{
    public partial class ProductCatalog
    {
        private int currentPage = 1;
        private int totalPages;
        private int counter = 0;
        private bool isAuthenticated;
        private string allCategories = "all_categories_list";

        public List<Product>? Products { get; set; }
        public List<Category>? Categories { get; set; }
        public string CategoryFilter { get; set; } = string.Empty;

        [Parameter, SupplyParameterFromQuery] public string Page { get; set; } = string.Empty;
        [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;
        [Parameter, SupplyParameterFromQuery] public int RecordsNumber { get; set; } = 8;

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private IDialogService DialogService { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;

        [CascadingParameter] private Task<AuthenticationState> authenticationStateTask { get; set; } = null!;
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        protected override async Task OnParametersSetAsync()
        {
            await CheckIsAuthenticatedAsync();
            await LoadCounterAsync();
            await LoadCategoriesAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            var responseHttp = await Repository.GetAsync<List<Category>>("api/categories/combo");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message, Severity.Error);
                return;
            }

            Categories = responseHttp.Response;
        }

        private async Task CheckIsAuthenticatedAsync()
        {
            var authenticationState = await authenticationStateTask;
            isAuthenticated = authenticationState.User.Identity!.IsAuthenticated;
        }

        private async Task LoadCounterAsync()
        {
            if (!isAuthenticated)
            {
                return;
            }

            var responseHttp = await Repository.GetAsync<int>("/api/temporalOrders/count");
            if (responseHttp.Error)
            {
                return;
            }
            counter = responseHttp.Response;
        }

        private async Task AddToCartAsync(int productId)
        {
            if (!isAuthenticated)
            {
                var closeOnEscapeKey = new DialogOptions() { CloseOnEscapeKey = true };
                DialogService.Show<Login>("Inicio de Sesion", closeOnEscapeKey);
                Snackbar.Add("Debes haber iniciado sesión para poder agregar productos al carro de compras.", Severity.Error);
                return;
            }

            var temporalOrderDTO = new TemporalOrderDTO
            {
                ProductId = productId
            };

            var httpActionResponse = await Repository.PostAsync("/api/temporalOrders/full", temporalOrderDTO);
            if (httpActionResponse.Error)
            {
                var message = await httpActionResponse.GetErrorMessageAsync();
                Snackbar.Add(message, Severity.Error);
                return;
            }

            await LoadCounterAsync();
            Snackbar.Add("Producto agregado al carro de compras.", Severity.Success);
        }

        private async Task FilterCallBack(string filter)
        {
            Filter = filter;
            await ApplyFilterAsync();
            StateHasChanged();
        }

        private async Task SelectedPageAsync(int page)
        {
            currentPage = page;
            await LoadAsync(page);
        }

        private async Task LoadAsync(int page = 1, string category = "")
        {
            if (!string.IsNullOrWhiteSpace(category))
            {
                if (category == allCategories)
                {
                    CategoryFilter = string.Empty;
                }
                else
                {
                    CategoryFilter = category;
                }
            }

            if (!string.IsNullOrWhiteSpace(Page))
            {
                page = Convert.ToInt32(Page);
            }

            var ok = await LoadListAsync(page);
            if (ok)
            {
                await LoadPagesAsync();
            }
        }

        private void ValidateRecordsNumber(int recordsnumber)
        {
            if (recordsnumber == 0)
            {
                RecordsNumber = 8;
            }
        }

        private async Task<bool> LoadListAsync(int page)
        {
            ValidateRecordsNumber(RecordsNumber);
            var url = $"api/products?page={page}&recordsnumber={RecordsNumber}";

            if (!string.IsNullOrEmpty(Filter))
            {
                url += $"&filter={Filter}";
            }
            if (!string.IsNullOrEmpty(CategoryFilter))
            {
                url += $"&CategoryFilter={CategoryFilter}";
            }

            var response = await Repository.GetAsync<List<Product>>(url);
            if (response.Error)
            {
                var message = await response.GetErrorMessageAsync();
                Snackbar.Add(message, Severity.Error);
                return false;
            }
            Products = response.Response;
            return true;
        }

        private async Task LoadPagesAsync()
        {
            ValidateRecordsNumber(RecordsNumber);
            var url = $"api/products/totalPages?recordsnumber={RecordsNumber}";
            if (!string.IsNullOrEmpty(Filter))
            {
                url += $"&filter={Filter}";
            }
            if (!string.IsNullOrEmpty(CategoryFilter))
            {
                url += $"&CategoryFilter={CategoryFilter}";
            }

            var response = await Repository.GetAsync<int>(url);
            if (response.Error)
            {
                var message = await response.GetErrorMessageAsync();
                Snackbar.Add(message, Severity.Error);
                return;
            }
            totalPages = response.Response;
        }

        private static string TruncateContent(string content, int length)
        {
            if (content.Length > length)
            {
                return content.Substring(0, length) + "...";
            }
            return content;
        }

        private async Task ApplyFilterAsync()
        {
            int page = 1;
            await LoadAsync(page);
            await SelectedPageAsync(page);
        }
    }
}