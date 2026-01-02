using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VilaManagement.Application.IO
{
    public interface IFilePathService
    {
        string CreateRelativePath(string rootPath, string path);

        string CreateRootPath(string rootPath, string path);

        string GetFullPath(IFormFile file, string rootPath, string folderPath);

        string GenerateFileName(IFormFile file);

        string GetFileNameFromFullPath(string fullPath);

        void Upload(IFormFile file, string filePath);

        void Delete(string filePath);
    }
}
