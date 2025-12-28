using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using WhiteLagoon.Domain.Entities;

namespace WhiteLagoon.Web.ViewModels
{
    public class VilaNumberViewModel
    {
        public VilaNumber VilaNumber { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> VilaList { get; set; }
    }
}
