using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface ISupplierService
    {
        Task<Supplier> CreateSupplierAsync(SupplierCreateRequest request);
        Task<bool> UpdateSupplierStatusAndRatingAsync(int supplierId, string status, decimal? rating);

        Task<List<Supplier>> GetSuppliers(int? supplierId, int? accountId, string? companyName, string? status, int? pageNumber, int? pageSize);

        Task DeleteSupplier(int supplierId);
    }
}
