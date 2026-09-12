namespace OpsFlow.Server.Utils
{
    public class ServiceResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public ServiceResult(bool success, string message, T? data = default)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public static ServiceResult<T> Fail(string message) =>
            new ServiceResult<T>(false, message);

        public static ServiceResult<T> Ok(T data) =>
            new ServiceResult<T>(true, string.Empty, data);
    }
}
