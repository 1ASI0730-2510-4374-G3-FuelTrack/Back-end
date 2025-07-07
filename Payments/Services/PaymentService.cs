using Microsoft.EntityFrameworkCore;
using FuelTrack.Api.Shared.Data;
using FuelTrack.Api.Shared.Models;
using FuelTrack.Api.Payments.DTOs;

namespace FuelTrack.Api.Payments.Services;

public class PaymentService : IPaymentService
{
    private readonly FuelTrackDbContext _context;

    public PaymentService(FuelTrackDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PaymentMethodDto>> GetPaymentMethodsAsync(int userId)
    {
        return await _context.PaymentMethods
            .Where(pm => pm.UserId == userId && pm.IsActive)
            .Select(pm => new PaymentMethodDto
            {
                Id = pm.Id,
                CardHolderName = pm.CardHolderName,
                LastFourDigits = pm.LastFourDigits,
                CardType = pm.CardType,
                ExpiryDate = pm.ExpiryDate,
                IsDefault = pm.IsDefault,
                CreatedAt = pm.CreatedAt
            })
            .OrderByDescending(pm => pm.IsDefault)
            .ThenByDescending(pm => pm.CreatedAt)
            .ToListAsync();
    }

    public async Task<PaymentMethodDto> CreatePaymentMethodAsync(CreatePaymentMethodDto createDto, int userId)
    {
        // Simulate card validation (in real implementation, use a payment processor)
        if (!IsValidCardNumber(createDto.CardNumber))
            throw new InvalidOperationException("Número de tarjeta inválido");

        var cardType = GetCardType(createDto.CardNumber);
        var lastFourDigits = createDto.CardNumber.Substring(createDto.CardNumber.Length - 4);
        var encryptedCardNumber = EncryptCardNumber(createDto.CardNumber); // Simulate encryption

        // If this is set as default, unset other default cards
        if (createDto.IsDefault)
        {
            var existingDefaults = await _context.PaymentMethods
                .Where(pm => pm.UserId == userId && pm.IsDefault)
                .ToListAsync();
                
            foreach (var pm in existingDefaults)
            {
                pm.IsDefault = false;
            }
        }

        var paymentMethod = new PaymentMethod
        {
            UserId = userId,
            CardHolderName = createDto.CardHolderName,
            LastFourDigits = lastFourDigits,
            CardType = cardType,
            EncryptedCardNumber = encryptedCardNumber,
            ExpiryDate = new DateTime(createDto.ExpiryYear, createDto.ExpiryMonth, 1),
            IsDefault = createDto.IsDefault
        };

        _context.PaymentMethods.Add(paymentMethod);
        await _context.SaveChangesAsync();

        return new PaymentMethodDto
        {
            Id = paymentMethod.Id,
            CardHolderName = paymentMethod.CardHolderName,
            LastFourDigits = paymentMethod.LastFourDigits,
            CardType = paymentMethod.CardType,
            ExpiryDate = paymentMethod.ExpiryDate,
            IsDefault = paymentMethod.IsDefault,
            CreatedAt = paymentMethod.CreatedAt
        };
    }

    public async Task<bool> DeletePaymentMethodAsync(int paymentMethodId, int userId)
    {
        var paymentMethod = await _context.PaymentMethods
            .FirstOrDefaultAsync(pm => pm.Id == paymentMethodId && pm.UserId == userId);

        if (paymentMethod == null)
            return false;

        paymentMethod.IsActive = false;
        paymentMethod.UpdatedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<PaymentDto> ProcessPaymentAsync(ProcessPaymentDto processDto, int userId)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == processDto.OrderId && o.UserId == userId);

        if (order == null)
            throw new KeyNotFoundException("Pedido no encontrado");

        var paymentMethod = await _context.PaymentMethods
            .FirstOrDefaultAsync(pm => pm.Id == processDto.PaymentMethodId && pm.UserId == userId && pm.IsActive);

        if (paymentMethod == null)
            throw new KeyNotFoundException("Método de pago no encontrado");

        // Check if order is already paid
        var existingPayment = await _context.Payments
            .FirstOrDefaultAsync(p => p.OrderId == processDto.OrderId && p.Status == PaymentStatus.Completed);

        if (existingPayment != null)
            throw new InvalidOperationException("El pedido ya ha sido pagado");

        // Simulate payment processing
        var transactionId = GenerateTransactionId();
        var paymentStatus = SimulatePaymentProcessing(); // 90% success rate

        var payment = new Payment
        {
            OrderId = processDto.OrderId,
            PaymentMethodId = processDto.PaymentMethodId,
            Amount = order.TotalAmount,
            Status = paymentStatus,
            TransactionId = paymentStatus == PaymentStatus.Completed ? transactionId : null,
            ProcessedAt = paymentStatus == PaymentStatus.Completed ? DateTime.UtcNow : null
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        return new PaymentDto
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            OrderNumber = order.OrderNumber,
            Amount = payment.Amount,
            Status = payment.Status,
            TransactionId = payment.TransactionId,
            ProcessedAt = payment.ProcessedAt,
            CreatedAt = payment.CreatedAt,
            CardHolderName = paymentMethod.CardHolderName,
            LastFourDigits = paymentMethod.LastFourDigits,
            CardType = paymentMethod.CardType
        };
    }

    public async Task<IEnumerable<PaymentDto>> GetPaymentsAsync(int? userId = null)
    {
        var query = _context.Payments
            .Include(p => p.Order)
            .Include(p => p.PaymentMethod)
            .AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(p => p.PaymentMethod.UserId == userId.Value);
        }

        return await query
            .Select(p => new PaymentDto
            {
                Id = p.Id,
                OrderId = p.OrderId,
                OrderNumber = p.Order.OrderNumber,
                Amount = p.Amount,
                Status = p.Status,
                TransactionId = p.TransactionId,
                ProcessedAt = p.ProcessedAt,
                CreatedAt = p.CreatedAt,
                CardHolderName = p.PaymentMethod.CardHolderName,
                LastFourDigits = p.PaymentMethod.LastFourDigits,
                CardType = p.PaymentMethod.CardType
            })
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<PaymentDto?> GetPaymentByIdAsync(int id, int? userId = null)
    {
        var query = _context.Payments
            .Include(p => p.Order)
            .Include(p => p.PaymentMethod)
            .Where(p => p.Id == id);

        if (userId.HasValue)
        {
            query = query.Where(p => p.PaymentMethod.UserId == userId.Value);
        }

        var payment = await query.FirstOrDefaultAsync();

        if (payment == null)
            return null;

        return new PaymentDto
        {
            Id = payment.Id,
            OrderId = payment.OrderId,
            OrderNumber = payment.Order.OrderNumber,
            Amount = payment.Amount,
            Status = payment.Status,
            TransactionId = payment.TransactionId,
            ProcessedAt = payment.ProcessedAt,
            CreatedAt = payment.CreatedAt,
            CardHolderName = payment.PaymentMethod.CardHolderName,
            LastFourDigits = payment.PaymentMethod.LastFourDigits,
            CardType = payment.PaymentMethod.CardType
        };
    }

    private static bool IsValidCardNumber(string cardNumber)
    {
        // Luhn algorithm implementation (simplified)
        cardNumber = cardNumber.Replace(" ", "").Replace("-", "");
        
        if (cardNumber.Length < 13 || cardNumber.Length > 19)
            return false;

        return cardNumber.All(char.IsDigit);
    }

    private static string GetCardType(string cardNumber)
    {
        cardNumber = cardNumber.Replace(" ", "").Replace("-", "");
        
        if (cardNumber.StartsWith("4"))
            return "Visa";
        else if (cardNumber.StartsWith("5") || cardNumber.StartsWith("2"))
            return "Mastercard";
        else if (cardNumber.StartsWith("3"))
            return "American Express";
        else
            return "Unknown";
    }

    private static string EncryptCardNumber(string cardNumber)
    {
        // In real implementation, use proper encryption
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(cardNumber));
    }

    private static string GenerateTransactionId()
    {
        return $"TXN-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";
    }

    private static PaymentStatus SimulatePaymentProcessing()
    {
        // 90% success rate simulation
        return Random.Shared.Next(1, 11) <= 9 ? PaymentStatus.Completed : PaymentStatus.Failed;
    }
}