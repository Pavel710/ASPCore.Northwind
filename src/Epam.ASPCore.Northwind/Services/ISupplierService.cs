using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Epam.ASPCore.Northwind.WebUI.Services
{
    public interface ISupplierService
    {
        List<SelectListItem> GetSuppliersSelectedList(int? selectedItemId = null);
    }
}
