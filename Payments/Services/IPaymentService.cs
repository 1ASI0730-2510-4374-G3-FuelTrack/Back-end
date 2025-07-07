using FuelTrack.Api.Payments.DTOs;

namespace FuelTrack.Api.Payments.Services;

public interface IPaymentService
{
    Task<IEnumerable<PaymentMethodDto>> GetPaymentMethodsAsync(int userId);
    Task<PaymentMethodDto> CreatePaymentMethodAsync(CreatePaymentMethodDto createDto, int userId);
    Task<bool> DeletePaymentMethodAsync(int paymentMethodId, int userId);
    Task<PaymentDto> ProcessPaymentAsync(ProcessPaymentDto processDto, int userId);
    Task<IEnumerable<PaymentDto>> GetPaymentsAsync(int? userId = null);
    Task<PaymentDto?> GetPaymentByIdAsync(int id, int? userId = null);
}