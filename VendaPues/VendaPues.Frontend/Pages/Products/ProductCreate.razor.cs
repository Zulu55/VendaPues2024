using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using VendaPues.Frontend.Pages.News;
using VendaPues.Frontend.Repositories;
using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;

namespace VendaPues.Frontend.Pages.Products
{
    [Authorize(Roles = "Admin")]
    public partial class ProductCreate
    {
        private ProductDTO productDTO = new()
        {
            ProductCategoryIds = new List<int>(),
            ProductImages = new List<string>()
        };

        private ProductForm? productForm;
        private List<Category> nonSelectedCategories = new();
        private bool loading = true;

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;

        [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            var httpActionResponse = await Repository.GetAsync<List<Category>>("/api/categories/combo");
            loading = false;

            if (httpActionResponse.Error)
            {
                var message = await httpActionResponse.GetErrorMessageAsync();
                Snackbar.Add(message, Severity.Error);
                return;
            }

            nonSelectedCategories = httpActionResponse.Response!;
        }

        private async Task CreateAsync()
        {
            var httpActionResponse = await Repository.PostAsync("/api/products/full", productDTO);
            if (httpActionResponse.Error)
            {
                var message = await httpActionResponse.GetErrorMessageAsync();
                Snackbar.Add(message, Severity.Error);
                return;
            }

            MudDialog.Close(DialogResult.Ok(true));
            productForm!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo($"/products");
            Snackbar.Add("Registro creado con éxito.", Severity.Success);
        }

        private void Return()
        {
            MudDialog.Close(DialogResult.Cancel());
            productForm!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo($"/products");
        }
    }
}