using Microsoft.AspNetCore.Components;
using MudBlazor;
using VendaPues.Frontend.Pages.News;
using VendaPues.Frontend.Repositories;
using VendaPues.Shared.Entities;

namespace VendaPues.Frontend.Pages
{
    public partial class NewsAndPromotions
    {
        private List<NewsArticle>? newsArticles;
        private const string baseUrl = "api/news";
        private bool loading = true;

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;
        [Inject] private IDialogService DialogService { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            loading = true;
            var url = $"{baseUrl}?page=1&recordsnumber={int.MaxValue}&Id=1";
            var responseHttp = await Repository.GetAsync<List<NewsArticle>>(url);
            loading = false;

            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message, Severity.Error);
            }

            newsArticles = responseHttp.Response;
        }

        private static string TruncateContent(string content, int length)
        {
            if (content.Length > length)
            {
                return content.Substring(0, length) + "...";
            }
            return content;
        }

        private void ViewDetails(NewsArticle news)
        {
            var options = new DialogOptions() { CloseOnEscapeKey = true, CloseButton = true };
            var parameters = new DialogParameters
            {
                { "Id", news.Id }
            };
            DialogService.Show<NewsDetail>(news.Title, parameters, options);
        }
    }
}