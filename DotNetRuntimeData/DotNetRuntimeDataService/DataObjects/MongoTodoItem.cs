using Microsoft.WindowsAzure.Mobile.Service;

namespace DotNetRuntimeDataService.DataObjects
{
    public class TodoItem : DocumentData
    {
        public string Text { get; set; }

        public bool Complete { get; set; }
    }
}