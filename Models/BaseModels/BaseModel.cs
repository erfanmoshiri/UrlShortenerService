using System;

namespace UrlService.Models.BaseModels
{
    public abstract class BaseModel : IBaseModel
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}