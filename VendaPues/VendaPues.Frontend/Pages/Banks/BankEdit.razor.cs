using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using VendaPues.Frontend.Repositories;
using VendaPues.Frontend.Shared;
using VendaPues.Shared.Entities;

namespace VendaPues.Frontend.Pages.Banks
{
    [Authorize(Roles = "Admin")]
    public partial class BankEdit
    {
        private Bank? bank;
        private FormWithName<Bank>? bankForm;

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private IDialogService DialogService { get; set; } = null!;
        [Inject] private ISnackbar Snackbar { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        [EditorRequired, Parameter] public int Id { get; set; }
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;

        protected override async Task OnParametersSetAsync()
        {
            var responseHttp = await Repository.GetAsync<Bank>($"/api/banks/{Id}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/banks");
                }
                else
                {
                    var messsage = await responseHttp.GetErrorMessageAsync();
                    Snackbar.Add(messsage, Severity.Error);
                }
            }
            else
            {
                bank = responseHttp.Response;
            }
        }

        private async Task EditAsync()
        {
            var responseHttp = await Repository.PutAsync("/api/banks", bank);
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
            bankForm!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo("/banks");
            Snackbar.Add("Cambios guardados con éxito.", Severity.Success);
            Snackbar.Add("Cambios guardados con éxito.", Severity.Success);
        }

        private void Return()
        {
            MudDialog.Close(DialogResult.Cancel());
            bankForm!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo("/banks");
        }
    }
}