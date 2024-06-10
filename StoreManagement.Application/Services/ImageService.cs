using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using StoreManagement.Application.Interfaces;
using StoreManagement.Domain.Const;

namespace StoreManagement.Application.Services;

public class ImageService(IOptionsSnapshot<AttachmentOptions> attachmentOptions) : IImageService
{

    private readonly IOptionsSnapshot<AttachmentOptions> _attachmentOptions = attachmentOptions;

    public async Task<string> UploadImage(string fileName, IFormFile image)
    {

        var allowedExtensions = _attachmentOptions.Value.AllowedExtension.Split(";");
        var fileExtension = Path.GetExtension(image.FileName);
        if (!allowedExtensions.Contains(fileExtension))
        {
            throw new ArgumentException($"Invalid file extension. Only {string.Join(", ", allowedExtensions)} files are allowed.");
        }

        var maxFileSizeInBytes = _attachmentOptions.Value.MaxSizeInMegaByte * 1024 * 1024; 
        if (image.Length > maxFileSizeInBytes)
        {
            throw new ArgumentException($"File size exceeds the maximum allowed size of {_attachmentOptions.Value.MaxSizeInMegaByte} MB.");
        }
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

        return storedFileName;
    }

    public string GetImageUrl(string path)
    {
         return path.Replace("wwwroot/", string.Empty);
    }
}
