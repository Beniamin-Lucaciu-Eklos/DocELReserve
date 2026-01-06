using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VilaManagement.Application.Common.Interfaces;
using VilaManagement.Domain.Entities;

namespace VilaManagement.Application.Services
{
    public interface IVilaNumberService
    {
        IEnumerable<VilaNumber> GetAll();

        VilaNumber Get(int id);

        bool Exists(int vilaNumber);

        void Add(VilaNumber vilaNumber);

        void Update(VilaNumber vilaNumber);

        void Remove(VilaNumber vilaNumber);
    }

    public class VilaNumberService : IVilaNumberService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VilaNumberService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Add(VilaNumber vilaNumber)
        {
            _unitOfWork.VilaNumber.Add(vilaNumber);
            _unitOfWork.SaveChanges();
        }

        public bool Exists(int vilaNumber)
        {
            return _unitOfWork.VilaNumber.Any(vn => vn.Vila_Number == vilaNumber);
        }

        public VilaNumber Get(int id)
        {
            return _unitOfWork.VilaNumber.Get(vila => vila.Vila_Number == id);
        }

        public IEnumerable<VilaNumber> GetAll()
        {
            return _unitOfWork.VilaNumber.GetAll(includeProperties: new string[] { nameof(Vila) }).ToList();
        }

        public void Remove(VilaNumber vilaNumber)
        {
            _unitOfWork.VilaNumber.Remove(vilaNumber);
            _unitOfWork.SaveChanges();
        }

        public void Update(VilaNumber vilaNumber)
        {
            _unitOfWork.VilaNumber.Update(vilaNumber);
            _unitOfWork.SaveChanges();
        }
    }
}
