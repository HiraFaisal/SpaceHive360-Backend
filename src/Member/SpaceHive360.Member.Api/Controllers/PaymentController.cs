using Microsoft.AspNetCore.Mvc;
using SpaceHive360.Member.Application.Models;
using SpaceHive360.Member.Application.Services;
using System.IO;
using System.Threading.Tasks;

namespace SpaceHive360.Member.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CreateCheckoutSessionRequest request)
        {
            var response = await _paymentService.CreateCheckoutSessionAsync(request);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"].ToString();

            var response = await _paymentService.HandleWebhookAsync(json, stripeSignature);
            return response.Success ? Ok() : BadRequest(response);
        }
    }
}
