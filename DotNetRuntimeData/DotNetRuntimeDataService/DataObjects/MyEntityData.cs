using DotNetRuntimeDataService.Models;
using Microsoft.WindowsAzure.Mobile.Service;

namespace DotNetRuntimeDataService.DataObjects
{
    public class MyEntityData : EntityData
    {
        public virtual Order Order { get; set; }

        public virtual Customer Customer { get; set; }

    }
}