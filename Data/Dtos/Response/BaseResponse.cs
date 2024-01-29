using System.Net;

namespace Identity.Data.Dtos.Response
{
    public class BaseResponse<T>
    {
        public HttpStatusCode Code { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public BaseResponse(T data, HttpStatusCode code, string message)
        {
            Data = data;
            Message = message;
            Code = code;
        }
    }
}