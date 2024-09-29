using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace VendaPues.Frontend.Shared
{
    public partial class GenericDialog
    {
        [Parameter] public string Message { get; set; } = null!;
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; } = null!;

        private void Close()
        {
            MudDialog.Close();
        }
    }
}