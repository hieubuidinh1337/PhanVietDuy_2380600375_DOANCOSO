// ================================================================
// Services/VNPayService.cs
// Tích hợp cổng thanh toán VNPay theo tài liệu chính thức
// https://sandbox.vnpayment.vn/apis/docs/thanh-toan-pay/pay.html
// ================================================================
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace PhanVietDuy_2380600375.Services
{
    // ── Model chứa thông tin để tạo link thanh toán ──────────────
    public class VNPayPaymentRequest
    {
        /// <summary>Mã đơn hàng nội bộ (dùng làm vnp_TxnRef)</summary>
        public string OrderCode { get; set; } = null!;
        /// <summary>Số tiền thanh toán (VND, không có phần thập phân)</summary>
        public long Amount { get; set; }
        /// <summary>Mô tả giao dịch hiển thị trên trang VNPay</summary>
        public string OrderInfo { get; set; } = "Thanh toan don hang Vietduy Atelier";
        /// <summary>IP người dùng — bắt buộc theo yêu cầu VNPay</summary>
        public string IpAddress { get; set; } = "127.0.0.1";
        /// <summary>Locale: "vn" hoặc "en"</summary>
        public string Locale { get; set; } = "vn";
    }

    // ── Model nhận kết quả callback từ VNPay ─────────────────────
    public class VNPayCallbackResult
    {
        public bool IsSuccess { get; set; }
        public string OrderCode { get; set; } = null!;
        public string TransactionId { get; set; } = null!;
        public long Amount { get; set; }
        public string ResponseCode { get; set; } = null!;
        public string Message { get; set; } = null!;
    }

    public interface IVNPayService
    {
        /// <summary>Tạo URL thanh toán để redirect người dùng sang VNPay</summary>
        string CreatePaymentUrl(VNPayPaymentRequest request, HttpContext httpContext);

        /// <summary>Xác thực callback và trả về kết quả giao dịch</summary>
        VNPayCallbackResult ProcessCallback(IQueryCollection query);
    }

    public class VNPayService : IVNPayService
    {
        private readonly IConfiguration _config;

        // Tên section trong appsettings.json: "VNPay"
        private string TmnCode   => _config["VNPay:TmnCode"]   ?? throw new InvalidOperationException("VNPay:TmnCode chưa cấu hình");
        private string HashSecret => _config["VNPay:HashSecret"] ?? throw new InvalidOperationException("VNPay:HashSecret chưa cấu hình");
        private string BaseUrl    => _config["VNPay:BaseUrl"]   ?? "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        private string ReturnUrl  => _config["VNPay:ReturnUrl"] ?? throw new InvalidOperationException("VNPay:ReturnUrl chưa cấu hình");

        public VNPayService(IConfiguration config)
        {
            _config = config;
        }

        // ──────────────────────────────────────────────────────────
        // BƯỚC 1: Tạo URL thanh toán VNPay
        // ──────────────────────────────────────────────────────────
        public string CreatePaymentUrl(VNPayPaymentRequest request, HttpContext httpContext)
        {
            // Theo yêu cầu VNPay: thời gian tạo và hết hạn
            var now       = DateTime.UtcNow.AddHours(7); // múi giờ Việt Nam (GMT+7)
            var expireTime = now.AddMinutes(15);

            var vnpParams = new SortedDictionary<string, string>(StringComparer.Ordinal)
            {
                // Phiên bản API
                ["vnp_Version"]    = "2.1.0",
                // Loại lệnh: "pay" = thanh toán
                ["vnp_Command"]    = "pay",
                // Mã website thương mại (lấy từ VNPay Merchant Portal)
                ["vnp_TmnCode"]    = TmnCode,
                // Số tiền * 100 (VNPay yêu cầu nhân 100)
                ["vnp_Amount"]     = (request.Amount * 100).ToString(),
                // Tiền tệ: VND
                ["vnp_CurrCode"]   = "VND",
                // Mã đơn hàng — duy nhất mỗi giao dịch
                ["vnp_TxnRef"]     = request.OrderCode,
                // Mô tả đơn hàng (URL-encode sau)
                ["vnp_OrderInfo"]  = request.OrderInfo,
                // Loại hàng: other = hàng hóa thông thường
                ["vnp_OrderType"]  = "other",
                // Ngôn ngữ trang thanh toán
                ["vnp_Locale"]     = request.Locale,
                // URL VNPay gọi về sau khi thanh toán
                ["vnp_ReturnUrl"]  = ReturnUrl,
                // IP người dùng — bắt buộc, tránh gian lận
                ["vnp_IpAddr"]     = request.IpAddress,
                // Thời gian tạo: yyyyMMddHHmmss
                ["vnp_CreateDate"] = now.ToString("yyyyMMddHHmmss"),
                // Thời gian hết hạn
                ["vnp_ExpireDate"] = expireTime.ToString("yyyyMMddHHmmss"),
            };

            // Tạo chuỗi ký: nối tất cả param theo thứ tự alphabet, URL-encode value
            var signData = BuildQueryString(vnpParams, encode: false);
            var signature = HmacSha512(HashSecret, signData);

            // Thêm chữ ký vào params
            vnpParams["vnp_SecureHash"] = signature;

            // Tạo URL cuối cùng (có encode)
            return BaseUrl + "?" + BuildQueryString(vnpParams, encode: true);
        }

        // ──────────────────────────────────────────────────────────
        // BƯỚC 2: Xử lý callback từ VNPay (GET /Checkout/VNPayReturn)
        // ──────────────────────────────────────────────────────────
        public VNPayCallbackResult ProcessCallback(IQueryCollection query)
        {
            // Tách riêng chữ ký ra, KHÔNG dùng để tính checksum
            var secureHash = query["vnp_SecureHash"].ToString();

            // Thu thập tất cả params trừ vnp_SecureHash và vnp_SecureHashType
            var vnpParams = new SortedDictionary<string, string>(StringComparer.Ordinal);
            foreach (var (key, value) in query)
            {
                if (!key.StartsWith("vnp_SecureHash"))
                    vnpParams[key] = value.ToString();
            }

            // Tính lại chữ ký để so sánh
            var signData   = BuildQueryString(vnpParams, encode: false);
            var checkHash  = HmacSha512(HashSecret, signData);

            // Kiểm tra tính toàn vẹn
            var isValidHash = string.Equals(checkHash, secureHash, StringComparison.OrdinalIgnoreCase);

            // Mã phản hồi: "00" = thành công
            var responseCode = query["vnp_ResponseCode"].ToString();
            var txnRef       = query["vnp_TxnRef"].ToString();
            var txnId        = query["vnp_TransactionNo"].ToString();
            var amountRaw    = long.TryParse(query["vnp_Amount"].ToString(), out var amt) ? amt / 100 : 0;

            var message = responseCode switch
            {
                "00" => "Giao dịch thành công",
                "07" => "Trừ tiền thành công. Giao dịch bị nghi ngờ gian lận",
                "09" => "Thẻ/Tài khoản chưa đăng ký Internet Banking",
                "10" => "Xác thực thẻ/tài khoản không đúng quá 3 lần",
                "11" => "Đã hết hạn chờ thanh toán",
                "12" => "Thẻ/Tài khoản bị khóa",
                "24" => "Giao dịch bị hủy",
                "51" => "Tài khoản không đủ số dư",
                "65" => "Tài khoản đã vượt hạn mức giao dịch trong ngày",
                "75" => "Ngân hàng thanh toán đang bảo trì",
                _    => $"Lỗi không xác định (mã {responseCode})"
            };

            return new VNPayCallbackResult
            {
                IsSuccess     = isValidHash && responseCode == "00",
                OrderCode     = txnRef,
                TransactionId = txnId,
                Amount        = amountRaw,
                ResponseCode  = responseCode,
                Message       = isValidHash ? message : "Chữ ký không hợp lệ — có thể bị giả mạo"
            };
        }

        // ── Helpers ──────────────────────────────────────────────

        /// <summary>
        /// Tạo chuỗi query string theo thứ tự alphabet (yêu cầu của VNPay).
        /// encode=false: dùng để ký (raw value)
        /// encode=true:  dùng để tạo URL
        /// </summary>
        private static string BuildQueryString(SortedDictionary<string, string> data, bool encode)
        {
            var parts = data
                .Where(kv => !string.IsNullOrEmpty(kv.Value))
                .Select(kv => encode
                    ? $"{kv.Key}={WebUtility.UrlEncode(kv.Value)}"
                    : $"{kv.Key}={kv.Value}");
            return string.Join("&", parts);
        }

        /// <summary>
        /// Tính HMAC-SHA512 theo secret key.
        /// VNPay yêu cầu dùng thuật toán này để ký giao dịch.
        /// </summary>
        private static string HmacSha512(string key, string data)
        {
            var keyBytes  = Encoding.UTF8.GetBytes(key);
            var dataBytes = Encoding.UTF8.GetBytes(data);
            using var hmac = new HMACSHA512(keyBytes);
            var hash = hmac.ComputeHash(dataBytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
