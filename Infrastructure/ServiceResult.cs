using System;

namespace UrlService.Infrastructure
{
    public class ServiceResult
    {
        public string ErrorMessage { get; set; }
        public string ReferenceId { get; set; }
        public bool HasError => ErrorMessage != null;

        public ServiceResult ()
        {
            ReferenceId = Guid.NewGuid().ToString ();
        }
        public ServiceResult (string error) : this ()
        {
            ErrorMessage = error;
        }
        

        public static ServiceResult Empty => new ServiceResult ();
        public static ServiceResult<T> Create<T> (T data) => new ServiceResult<T> (data);
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T Result { get; set; }

        public ServiceResult (T data)
        {
            this.Result = data;
        }

        public static ServiceResult<T> From (ServiceResult result) => new ServiceResult<T> (default (T));
    }
}