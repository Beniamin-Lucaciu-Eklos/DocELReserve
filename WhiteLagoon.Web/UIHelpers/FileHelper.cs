namespace WhiteLagoon.Web.UIHelpers
{
    public static class FileHelper
    {
        public static string CreateRootPath(string rootPath, string path)
        {
            return Path.Combine(rootPath, path);
        }

        public static string GenerateFileName(this IFormFile file)
        {
            var fileExtension = Path.GetExtension(file.FileName);
            return $"{Guid.NewGuid()}{fileExtension}";
        }

        public static string GetFullPath(this IFormFile file, string rootPath, string folderPath)
        {
            var root = CreateRootPath(rootPath, folderPath);
            var fileName = file.GenerateFileName();

            return Path.Combine(root, fileName);
        }

        public static string GetFileNameFromFullPath(this string fullPath) => Path.GetFileName(fullPath);

        public static void CopyTo(this IFormFile file, string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
        }
    }
}
