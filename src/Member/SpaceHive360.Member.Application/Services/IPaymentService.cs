using SpaceHive360.Member.Application.Models;
using System.Threading.Tasks;

namespace SpaceHive360.Member.Application.Services
{
    public interface IPaymentService
    {
        Task<ApiResponse> CreateCheckoutSessionAsync(CreateCheckoutSessionRequest request);
        Task<ApiResponse> HandleWebhookAsync(string json, string stripeSignature);
        Task<ApiResponse> VerifyPaymentAsync(string sessionId);
    }
}
