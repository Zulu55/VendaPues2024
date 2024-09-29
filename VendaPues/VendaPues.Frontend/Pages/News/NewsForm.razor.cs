using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;
using VendaPues.Frontend.Repositories;
using VendaPues.Frontend.Shared;
using VendaPues.Shared.Entities;

namespace VendaPues.Frontend.Pages.News
{
    public partial class NewsForm
    {
        private EditContext editContext = null!;
        private string? imageUrl;

        [Inject] private IDialogService DialogService { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;

        [Parameter, EditorRequired] public NewsArticle NewsArticle { get; set; } = null!;
        [Parameter, EditorRequired] public EventCallback OnValidSubmit { get; set; }
        [Parameter, EditorRequired] public EventCallback ReturnAction { get; set; }
        [Parameter] public bool IsEdit { get; set; } = false;

        public bool FormPostedSuccessfully { get; set; } = false;
        private string titleLabel => IsEdit ? "Editar Noticia" : "Crear Noticia";

        protected override void OnInitialized()
        {
            base.OnInitialized();
            editContext = new(NewsArticle);
        }

        private void ImageSelected(string imagenBase64)
        {
            NewsArticle.ImageUrl = imagenBase64;
            imageUrl = null;
        }

        private async Task OnDataAnnotationsValidatedAsync()
        {
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