// File: VietCommerce.Data/Repositories/PaymentRepository.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VietCommerce.Core.Entities.Payments;
using VietCommerce.Core.Enums.Payments;
using VietCommerce.Data.Context;
using VietCommerce.Data.Repositories.Interfaces;

namespace VietCommerce.Data.Repositories
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Payment?> GetByOrderIdAsync(Guid orderId)
        {
            return await _dbSet
                .Include(p => p.Order)
                .Include(p => p.PaymentMethod)
                .Include(p => p.PaymentTransactions)
                .FirstOrDefaultAsync(p => p.OrderId == orderId);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByOrderIdAsync(Guid orderId)
        {
            return await _dbSet
                .Include(p => p.PaymentMethod)
                .Include(p => p.PaymentTransactions)
                .Where(p => p.OrderId == orderId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Payment?> GetByTransactionIdAsync(string transactionId)
        {
            if (string.IsNullOrWhiteSpace(transactionId))
                return null;

            return await _dbSet
                .Include(p => p.Order)
                .Include(p => p.PaymentMethod)
                .Include(p => p.PaymentTransactions)
                .FirstOrDefaultAsync(p => p.PaymentTransactions.Any(pt => pt.TransactionId == transactionId));
        }

        public async Task<bool> UpdatePaymentStatusAsync(
            Guid paymentId,
            PaymentStatus status,
            string? transactionId = null,
            string? gatewayResponse = null)
        {
            var payment = await _dbSet
                .Include(p => p.PaymentTransactions)
                .FirstOrDefaultAsync(p => p.Id == paymentId);

            if (payment == null)
                return false;

            // Update Payment status
            payment.Status = status;
            payment.UpdatedAt = DateTime.UtcNow;

            // Update PaidAt if status is Paid
            if (status == PaymentStatus.Paid)
            {
                payment.PaidAt = DateTime.UtcNow;
            }

            // Create new PaymentTransaction record
            if (!string.IsNullOrWhiteSpace(transactionId))
            {
                var transaction = new PaymentTransaction
                {
                    Id = Guid.NewGuid(),
                    PaymentId = paymentId,
                    TransactionId = transactionId,
                    Status = PaymentMethodType.CONFIRMED,
                    GatewayResponse = gatewayResponse,
                    TransactionDate = DateTime.UtcNow,
                    Amount = payment.Amount,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                payment.PaymentTransactions.Add(transaction);
            }

            _dbSet.Update(payment);
            return true;
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByStatusAsync(PaymentStatus status)
        {
            return await _dbSet
                .Include(p => p.Order)
                .Include(p => p.PaymentMethod)
                .Include(p => p.PaymentTransactions)
                .Where(p => p.Status == status)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Payment?> GetPaymentWithDetailsAsync(Guid paymentId)
        {
            return await _dbSet
                .Include(p => p.Order)
                    .ThenInclude(o => o.Customer)
                .Include(p => p.PaymentMethod)
                .Include(p => p.PaymentTransactions.OrderByDescending(pt => pt.CreatedAt))
                .FirstOrDefaultAsync(p => p.Id == paymentId);
        }

        public async Task<bool> HasSuccessfulPaymentAsync(Guid orderId)
        {
            return await _dbSet
                .AnyAsync(p => p.OrderId == orderId &&
                              p.Status == PaymentStatus.Paid);
        }

        public async Task<IEnumerable<PaymentTransaction>> GetPaymentTransactionsByOrderIdAsync(Guid orderId)
        {
            return await _context.Set<PaymentTransaction>()
                .Include(pt => pt.Payment)
                    .ThenInclude(p => p.PaymentMethod)
                .Where(pt => pt.Payment.OrderId == orderId)
                .OrderByDescending(pt => pt.CreatedAt)
                .ToListAsync();
        }
    }
}
