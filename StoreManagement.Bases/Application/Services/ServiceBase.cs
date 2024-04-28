using StoreManagement.Bases.Infrastructure;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StoreManagement.Bases;

public abstract class ServiceBase
{

    public ServiceBase() { }
    // public Guid UserId => Guid.Parse(_httpContext?.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());
    //public Guid UserId => UserState.GetCurrentUserId();
    public Guid UserId { get { return UserState.UserId; } }
    protected async Task<ServiceResponse<T>> LogError<T>(Exception ex, T data, object inputs = default)
    {
        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), DateTime.Now.ToString(@"yyyy\\MM"))))
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), DateTime.Now.ToString(@"yyyy\\MM")));

        string path = Path.Combine(Directory.GetCurrentDirectory(), DateTime.Now.ToString(@"yyyy\\MM\\dd")) + ".log";
        string contents = $@"==={DateTime.Now.ToString("HH:mm:ss")}=======================================================================================
            {ex.Message}
----------------------------------------
            {ex.InnerException?.Message}
----------------------------------------
            {ex.StackTrace}
----------------------------------------
            {JsonConvert.SerializeObject(inputs)}
==================================================================================================" + Environment.NewLine;
        await File.AppendAllTextAsync(path, contents);
        return new ServiceResponse<T>() { Success = false, Data = data, Message = ex.Message + Environment.NewLine + ex.InnerException?.Message };
    }
    public static string GetCurrentLanguage()
    {
        return Thread.CurrentThread.CurrentCulture.ToString();
    }

}

