using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Service.DTOs.Request;

namespace FestivalFlatform.Service.Services.Interface
{
    public interface IBoothService
    {
        Task<Booth> CreateBoothAsync(BoothCreateRequest request);
        Task<Booth?> UpdateBoothApproveAsync(int boothId, DateTime approvalDate, int pointsBalance);

        Task<List<Booth>> GetBooths(int? boothId, int? groupId, int? festivalId, int? locationId, string? boothType, string? status, int? pageNumber, int? pageSize);
        Task UpdateBoothStatusToRejected(int boothId, string? rejectReason);
        Task UpdateBoothStatusToActive(int boothId);
        Task DeleteBoothAsync(int boothId);
        Task<Booth?> UpdateBoothAsync(int boothId, BoothUpdateRequest request);
        Task<Booth> WithdrawBoothRevenueAsync(int boothId, int accountId);
        Task<bool> CanWithdrawRevenueAsync(int boothId, int accountId);
    }
}
