using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using UrlService.Models.BaseModels;

namespace UrlService.Models
{
    public class User : IdentityUser, IBaseModel
    {
        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}