using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Implement;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _service;

        public PaymentController(IPaymentService service)
        {
            _service = service;
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            var result = await _service.CreatePaymentAsync(request);
            return Ok(result);
        }
        [HttpPut("update")]
        public async Task<IActionResult> UpdatePayment(int paymentId, string? status, string? description)
        {
            var result = await _service.UpdatePaymentAsync(paymentId, status, description);
            return Ok(result);
        }
        [HttpGet("search")]
        public async Task<IActionResult> Search(
        int? orderId,
        int? walletId,
        string? paymentMethod,
        string? paymentType,
        string? status,
        int? pageNumber,
        int? pageSize)
        {
            var result = await _service.SearchPaymentsAsync(
                orderId, walletId, paymentMethod, paymentType, status, pageNumber, pageSize);

            return Ok(result);
        }
        [HttpDelete("delete")]
        public async Task<IActionResult> DeletePayment(int paymentId)
        {
            var result = await _service.DeletePaymentAsync(paymentId);
            return Ok(new { success = result });
        }


        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook()
        {
            
            string rawJson = await new StreamReader(Request.Body).ReadToEndAsync();

         
            await _service.HandleWebhookAsync(rawJson);

            return Ok();
        }
    }
}
