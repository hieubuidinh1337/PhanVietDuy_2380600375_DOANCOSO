using PhanVietDuy_2380600375.Models.Domain;
using PhanVietDuy_2380600375.Models.DTOs;
using System.Threading.Tasks;

namespace PhanVietDuy_2380600375.Services.Interfaces
{
    public interface IOrderService
    {
        // ── Các method hiện có ──────────────────────────────────────────
        Task<int> CreateOrderAsync(CreateOrderRequest req);
        Task<Order?> GetByIdAsync(int id);
        Task UpdateStatusAsync(int id, string newStatus, string adminId);
        Task CancelAsync(int orderId, string userId);

        // ── Methods mới cho VNPay ────────────────────────────────────────

        /// <summary>
        /// Tìm đơn hàng theo OrderCode (VD-XXXXXX) — dùng sau VNPay callback.
        /// vnp_TxnRef chứa OrderCode, không phải OrderId.
        /// </summary>
        Task<Order?> GetByOrderCodeAsync(string orderCode);

        /// <summary>
        /// Cập nhật PaymentStatus thành "Paid" sau khi VNPay xác nhận thành công.
        /// Đồng thời cập nhật Order.Status thành "Confirmed".
        /// </summary>
        Task MarkAsPaidAsync(string orderCode, string transactionId, string paymentProvider);
    }
}
