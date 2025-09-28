namespace Gradution_Project_G5.BLL.ViewModels
{
    public class Result<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public static class ResultHelper
    {
        public static Result<T> Success<T>(T data)
        {
            return new Result<T> { Success = true, Data = data };
        }

        public static Result<T> Failure<T>(string message)
        {
            return new Result<T> { Success = false, Message = message };
        }
    }

    public class PagedResult<T>
    {
        public System.Collections.Generic.IEnumerable<T> Items { get; set; } = new System.Collections.Generic.List<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)System.Math.Ceiling(TotalCount / (double)PageSize);
    }
}