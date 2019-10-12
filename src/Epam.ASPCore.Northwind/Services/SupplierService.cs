﻿using System;
using System.Collections.Generic;
using System.Linq;
using Epam.ASPCore.Northwind.Domain.Models;
using Epam.ASPCore.Northwind.Domain.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Epam.ASPCore.Northwind.WebUI.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly INorthwindRepository<Suppliers> _supplierRepository;

        public SupplierService(INorthwindRepository<Suppliers> supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public List<SelectListItem> GetSuppliersSelectedList(int? selectedItemId = null)
        {
            try
            {
                List<SelectListItem> list = new List<SelectListItem>();
                var suppliers = _supplierRepository.Get().ToList();

                foreach (var supplier in suppliers)
                {
                    var selected = supplier.SupplierId == selectedItemId;
                    list.Add(new SelectListItem
                    {
                        Value = supplier.SupplierId.ToString(),
                        Text = supplier.CompanyName,
                        Selected = selected
                    });
                }

                return list;
            }
            catch (Exception e)
            {
                Log.Error("Supplier service error!" + Environment.NewLine + $"{e}");
                throw;
            }
        }
    }
}
