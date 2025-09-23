using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Data.UnitOfWork;
using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Exceptions;
using FestivalFlatform.Service.Helpers;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FestivalFlatform.Service.Services.Implement
{
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;


        public SupplierService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Supplier> CreateSupplierAsync(SupplierCreateRequest request)
        {
            var accountExists = _unitOfWork.Repository<Account>()
      .GetAll().Any(s => s.AccountId == request.AccountId);

            if (!accountExists)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy account tương ứng", request.AccountId.ToString());
            }

            var supplier = new Supplier
            {
                AccountId = request.AccountId,
                CompanyName = request.CompanyName,
                BusinessLicense = request.BusinessLicense,
                Category = request.Category,
                Note = request.Note,
                Address = request.Address,
                ContactInfo = request.ContactInfo,
                Status = StatusSupplier.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Supplier>().InsertAsync(supplier);
            await _unitOfWork.CommitAsync();

            return supplier;
        }

        public async Task<bool> UpdateSupplierStatusAndRatingAsync(int supplierId, string status, decimal? rating)
        {
            var supplier = await _unitOfWork.Repository<Supplier>().FindAsync(s => s.SupplierId == supplierId);
            if (supplier == null)
            {
                return false; 
            }

            supplier.Status = status;
            supplier.Rating = rating;
            supplier.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.CommitAsync();

            return true;
        }
        public async Task<List<Supplier>> GetSuppliers(int? supplierId, int? accountId, string? companyName, string? status, int? pageNumber, int? pageSize)
        {
            var query = _unitOfWork.Repository<Supplier>().GetAll()
                .Include(s => s.Ingredients)
                .Where(s => !supplierId.HasValue || supplierId == 0 || s.SupplierId == supplierId.Value)
                .Where(s => !accountId.HasValue || accountId == 0 || s.AccountId == accountId.Value)
                .Where(s => string.IsNullOrWhiteSpace(companyName) || s.CompanyName.Contains(companyName.Trim()))
                .Where(s => string.IsNullOrWhiteSpace(status) || s.Status == status.Trim());

            //int currentPage = pageNumber.HasValue && pageNumber.Value > 0 ? pageNumber.Value : 1;
            //int currentSize = pageSize.HasValue && pageSize.Value > 0 ? pageSize.Value : 10;

            //query = query.Skip((currentPage - 1) * currentSize)
            //             .Take(currentSize);

            var suppliers = await query.ToListAsync();

         

            return suppliers;
        }
        public async Task DeleteSupplier(int supplierId)
        {
            var supplier = await _unitOfWork.Repository<Supplier>().FindAsync(s => s.SupplierId == supplierId);

            if (supplier == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy nhà cung cấp", supplierId.ToString());
            }

            _unitOfWork.Repository<Supplier>().Delete(supplier);
            await _unitOfWork.CommitAsync();
        }
    }
}
