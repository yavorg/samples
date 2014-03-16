using Microsoft.WindowsAzure.Mobile.Service;

namespace DotNetRuntimeDataService.DataObjects
{
    public class MongoTodoItem : EntityData
    {
        public string Text { get; set; }

        public bool Complete { get; set; }
    }
}