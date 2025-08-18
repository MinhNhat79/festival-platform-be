using System.Text.Json;
using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace FestivalManagementFlatformm.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentController(IPaymentService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            try
            {
                var result = await _service.CreatePaymentAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdatePayment(int paymentId, string? status, string? description)
        {
            try
            {
                var result = await _service.UpdatePaymentAsync(paymentId, status, description);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
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
            try
            {
                var result = await _service.SearchPaymentsAsync(
                    orderId, walletId, paymentMethod, paymentType, status, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeletePayment(int paymentId)
        {
            try
            {
                var success = await _service.DeletePaymentAsync(paymentId);
                if (!success)
                    return BadRequest(new { Success = false, Message = "Delete failed" });

                return Ok(new { Success = true, Message = "Deleted successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = ex.Message });
            }
        }
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] JsonElement rawJsonElement)
        {
            try
            {
                var rawJson = rawJsonElement.GetRawText();
                var result = await _service.HandleWebhookAsync(rawJson);

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Bắt lỗi, nhưng vẫn trả 200 OK
                return Ok(new
                {
                    success = false,
                    message = $"Lỗi khi xử lý webhook: {ex.Message}"
                });
            }
        }

        [HttpGet("webhook")]
        public IActionResult Message()
        {
            return Ok(new
            {
                Success = true,
                Message = "PayOS webhook endpoint is working"
            });
        }
    }
}
