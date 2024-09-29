using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using VendaPues.Frontend.Repositories;
using VendaPues.Frontend.Shared;
using VendaPues.Shared.Entities;

namespace VendaPues.Frontend.Pages.News
{
    [Authorize(Roles = "Admin")]
    public partial class NewsEdit
    {
        private NewsArticle? newsArticle;
        private NewsForm? newsForm;

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private IDialogService DialogService { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        [EditorRequired, Parameter] public int Id { get; set; }
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;

        protected override async Task OnParametersSetAsync()
        {
            var responseHttp = await Repository.GetAsync<NewsArticle>($"/api/news/{Id}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/news");
                }
                else
                {
                    var messsage = await responseHttp.GetErrorMessageAsync();
                    Snackbar.Add(messsage, Severity.Error);
                }
            }
            else
            {
                newsArticle = responseHttp.Response;
            }
        }

        private async Task EditAsync()
        {
            var responseHttp = await Repository.PutAsync("/api/news", newsArticle);
            if (responseHttp.Error)
            {
                MudDialog.Close(DialogResult.Cancel());
                var message = await responseHttp.GetErrorMessageAsync();
                var parameters = new DialogParameters
                {
                    { "Message", message }
                };
                var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
                DialogService.Show<GenericDialog>("Error", parameters, options);
                return;
            }

            MudDialog.Close(DialogResult.Ok(true));
            newsForm!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo("/news");
            Snackbar.Add("Cambios guardados con éxito.", Severity.Success);
        }

        private void Return()
        {
            MudDialog.Close(DialogResult.Cancel());
            newsForm!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo("/news");
        }
    }
}