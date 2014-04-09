using DotNetRuntimeDataService.Models;
using Microsoft.WindowsAzure.Mobile.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DotNetRuntimeDataService.DataObjects
{
    public class MyEntityData : EntityData
    {
        public virtual Order Order { get; set; }

        public virtual Customer Customer { get; set; }

    }
}