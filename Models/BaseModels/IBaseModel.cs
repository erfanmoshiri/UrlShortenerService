using System;

namespace UrlService.Models.BaseModels
{
    public interface IBaseModel
    {
        string Id { get; set; }
        DateTime CreatedAt { get; set; }
        bool IsDeleted { get; set; }
    }
}