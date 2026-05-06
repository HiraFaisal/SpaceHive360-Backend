using SpaceHive360.Member.Application.Models;
using System.Threading.Tasks;

namespace SpaceHive360.Member.Application.Services
{
    public interface IMembershipService
    {
        Task<ApiResponse> PurchaseMembershipAsync(MembershipPurchaseRequest request);
        Task<ApiResponse> PurchaseBookingAsync(BookingPurchaseRequest request);
        Task<ApiResponse> GetMyMembershipsAsync(Guid userId);
    }
}
