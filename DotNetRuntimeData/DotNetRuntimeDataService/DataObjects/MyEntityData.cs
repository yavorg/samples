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
        [Required]
        public virtual Order Order { get; set; }

    }
}