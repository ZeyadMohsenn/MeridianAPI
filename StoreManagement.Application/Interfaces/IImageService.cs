using Microsoft.AspNetCore.Http;

namespace StoreManagement.Application.Interfaces;

public interface IImageService
{
    Task<string> UploadImage(string fileName, IFormFile image);
    string GetImageUrl(string path);
}
