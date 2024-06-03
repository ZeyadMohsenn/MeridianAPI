using Microsoft.AspNetCore.Http;
using StoreManagement.Application.Interfaces;
using StoreManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManagement.Application.Services;

public class ImageService(IHttpContextAccessor httpContextAccessor) : IImageService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    public async Task<string> UploadImage(string fileName, IFormFile image)
    {
        var fileExtension = Path.GetExtension(image.FileName);
        var fakeFileName = $"{Guid.NewGuid().ToString()}{fileExtension}";
        var storedFileName = "wwwroot/" + $"{fileName}/" + fakeFileName;
        var directory = Path.GetDirectoryName(storedFileName);

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), storedFileName);

        using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(fileStream);
        }

        //category.Photo = image.FileName;
        //category.ContentType = image.ContentType;
        //category.StoredFileName =
        //
        return storedFileName;
    }

    public string GetCategoryImageUrl(string path)
    {
        var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host.Value}";
        baseUrl = baseUrl.TrimEnd('/');

        var imageUrl = $"{baseUrl}/{path}";
        imageUrl = imageUrl.Replace("wwwroot/", string.Empty);

        return imageUrl;
    }
}
