using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using VilaManagement.Domain.Entities;

namespace VilaManagement.Web.ViewModels
{
    public class VilaNumberViewModel
    {
        public VilaNumber VilaNumber { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> VilaList { get; set; }
    }
}
