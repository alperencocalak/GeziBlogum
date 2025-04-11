using System.Runtime.CompilerServices;

namespace GeziBlogum.Utils
{
public static class FileHelper
{
    public static async Task<string> FileLoaderAsync(IFormFile formFile, string filePath = "/img/")
    {
        if (formFile == null || formFile.Length == 0)
            return string.Empty;

        var extension = Path.GetExtension(formFile.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var directory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.Trim('/'));
        var fileFullPath = Path.Combine(directory, fileName);

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var stream = new FileStream(fileFullPath, FileMode.Create);
        await formFile.CopyToAsync(stream);
        

        return $"{filePath}{fileName}";
    }
}

}