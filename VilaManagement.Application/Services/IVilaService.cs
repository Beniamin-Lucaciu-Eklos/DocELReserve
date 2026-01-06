using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VilaManagement.Application.Common.Interfaces;
using VilaManagement.Application.IO;
using VilaManagement.Domain.Entities;

namespace VilaManagement.Application.Services
{
    public interface IVilaService
    {
        IEnumerable<Vila> GetAll();

        Vila Get(int id);

        void Add(Vila vila);

        void UploadImage(Vila vila);
        void SaveImageUrl(Vila vila);
        void DeleteImage(Vila vila);

        void Update(Vila vila);

        void Remove(Vila vila);

    }

    public class VilaService : IVilaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFilePathService _filePathService;

        public VilaService(IUnitOfWork unitOfWork,
            IWebHostEnvironment webHostEnvironment,
            IFilePathService filePathService)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _filePathService = filePathService;
        }

        public void Add(Vila vila)
        {
            _unitOfWork.Villa.Add(vila);
            _unitOfWork.SaveChanges();
        }

        private bool CanSaveImageUrl(Vila vila) => vila.Image is IFormFile;

        public void SaveImageUrl(Vila vila)
        {
            if (CanSaveImageUrl(vila))
            {
                var imagePath = GetImagePath(vila);

                var fileName = _filePathService.GetFileNameFromFullPath(imagePath);
                vila.ImageUrl = _filePathService.CreateRelativePath(
                    FileStorageConstants.VilaImagesFolderPath,
                    fileName);
            }
            else
            {
                vila.ImageUrl = FileStorageConstants.NoImageVilaPlaceHolder;
            }
        }

        public void UploadImage(Vila vila)
        {
            if (CanSaveImageUrl(vila))
            {
                var imagePath = GetImagePath(vila);
                _filePathService.Upload(vila.Image, imagePath);
            }
        }

        private string GetImagePath(Vila vila)
        {
            return _filePathService.GetFullPath(vila.Image, _webHostEnvironment.WebRootPath, FileStorageConstants.VilaImagesFolderPath);
        }

        public void DeleteImage(Vila vila)
        {
            if (string.IsNullOrEmpty(vila.ImageUrl)
                || vila.ImageUrl == FileStorageConstants.NoImageVilaPlaceHolder)
            {
                return;
            }

            var normalizedPath = vila.ImageUrl
                .Replace("~/", string.Empty)
                .Replace("/", Path.DirectorySeparatorChar.ToString());

            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, normalizedPath);
            _filePathService.Delete(fullPath);
        }

        public Vila Get(int id)
        {
            return _unitOfWork.Villa.Get(v => v.Id == id);
        }

        public IEnumerable<Vila> GetAll()
        {
            return _unitOfWork.Villa.GetAll();
        }

        public void Remove(Vila vila)
        {
            var vilaFromDb = Get(vila.Id);
            if (vilaFromDb is not null)
            {
                _unitOfWork.Villa.Remove(vilaFromDb);
                _unitOfWork.SaveChanges();
            }
        }

        public void Update(Vila vila)
        {
            _unitOfWork.Villa.Update(vila);
            _unitOfWork.SaveChanges();
        }
    }
}
