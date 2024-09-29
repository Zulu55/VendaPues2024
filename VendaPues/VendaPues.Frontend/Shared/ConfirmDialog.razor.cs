using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace VendaPues.Frontend.Shared
{
    public partial class ConfirmDialog
    {
        [Parameter] public string Message { get; set; } = null!;
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;

        private void Accept()
        {
            MudDialog.Close(DialogResult.Ok(true));
        }

        private void Cancel()
        {
            MudDialog.Close(DialogResult.Cancel());
        }
    }
}