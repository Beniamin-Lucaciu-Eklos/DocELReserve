using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VilaManagement.Application.IO;

namespace VilaManagement.Infrastructure.IO
{
    public class FilePathService : IFilePathService
    {
        public string CreateRootPath(string rootPath, string path)
        {
            return Path.Combine(rootPath, path);
        }

        public string GetFullPath(IFormFile file, string rootPath, string folderPath)
        {
            var root = CreateRootPath(rootPath, folderPath);
            var fileName = GenerateFileName(file);

            return Path.Combine(root, fileName);
        }

        public string GenerateFileName(IFormFile file)
        {
            var fileExtension = Path.GetExtension(file.FileName);
            return $"{Guid.NewGuid()}{fileExtension}";
        }

        public string GetFileNameFromFullPath(string fullPath) => Path.GetFileName(fullPath);

        public void Upload(IFormFile file, string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
        }

        public void Delete(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
