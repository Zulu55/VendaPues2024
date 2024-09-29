using System.Net;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using VendaPues.Frontend.Repositories;
using VendaPues.Shared.Entities;

namespace VendaPues.Frontend.Pages.News
{
    public partial class NewsDetail
    {
        private NewsArticle? newsArticle;
        private bool loading = true;

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;

        [EditorRequired, Parameter] public int Id { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            loading = true;
            var responseHttp = await Repository.GetAsync<NewsArticle>($"/api/news/{Id}");
            loading = false;

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
    }
}