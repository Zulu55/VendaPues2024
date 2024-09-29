using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;
using VendaPues.Frontend.Helpers;
using VendaPues.Frontend.Shared;
using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;

namespace VendaPues.Frontend.Pages.Products
{
    public partial class ProductForm
    {
        private EditContext editContext = null!;
        private string? imageUrl;
        private string titleLabel => IsEdit ? "Editar Producto" : "Crear Producto";

        private List<MultipleSelectorModel> selected { get; set; } = new();
        private List<MultipleSelectorModel> nonSelected { get; set; } = new();

        [Inject] private IDialogService DialogService { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;
        [Parameter, EditorRequired] public ProductDTO ProductDTO { get; set; } = null!;
        [Parameter, EditorRequired] public EventCallback OnValidSubmit { get; set; }
        [Parameter, EditorRequired] public EventCallback ReturnAction { get; set; }
        [Parameter, EditorRequired] public List<Category> NonSelectedCategories { get; set; } = new();
        [Parameter] public bool IsEdit { get; set; } = false;
        [Parameter] public EventCallback AddImageAction { get; set; }
        [Parameter] public EventCallback RemoveImageAction { get; set; }
        [Parameter] public List<Category> SelectedCategories { get; set; } = new();

        public bool FormPostedSuccessfully { get; set; } = false;

        protected override void OnInitialized()
        {
            editContext = new(ProductDTO);

            selected = SelectedCategories.Select(x => new MultipleSelectorModel(x.Id.ToString(), x.Name)).ToList();
            nonSelected = NonSelectedCategories.Select(x => new MultipleSelectorModel(x.Id.ToString(), x.Name)).ToList();
        }

        private void ImageSelected(string imagenBase64)
        {
            if (ProductDTO.ProductImages is null)
            {
                ProductDTO.ProductImages = [];
            }

            ProductDTO.ProductImages!.Add(imagenBase64);
            imageUrl = null;
        }

        private async Task OnDataAnnotationsValidatedAsync()
        {
            ProductDTO.ProductCategoryIds = selected.Select(x => int.Parse(x.Key)).ToList();
            await OnValidSubmit.InvokeAsync();
        }

        private async Task OnBeforeInternalNavigation(LocationChangingContext context)
        {
            var formWasEdited = editContext.IsModified();

            if (!formWasEdited)
            {
                return;
            }

            if (FormPostedSuccessfully)
            {
                return;
            }

            var parameters = new DialogParameters
            {
                { "Message", "¿Deseas abandonar la página y perder los cambios?" }
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
            var dialog = DialogService.Show<ConfirmDialog>("Confirmación", parameters, options);
            var result = await dialog.Result;
            if (result.Canceled)
            {
                return;
            }

            context.PreventNavigation();
        }
    }
}