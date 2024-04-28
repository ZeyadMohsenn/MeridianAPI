namespace StoreManagement.Bases;

public class ServiceResponse<T>
{
    public ServiceResponse()
    {

    }

    public ServiceResponse(string message)
    {
        Message = message;
    }

    public ServiceResponse(bool success=false,string message="",T data = default(T))
    {
        Success = success;
        Message = message;
        Data = data;
    }
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; } 
}