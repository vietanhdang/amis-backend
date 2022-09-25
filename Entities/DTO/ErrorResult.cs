using Misa.Web08.TCDN.API.Enum;

namespace Misa.Web08.TCDN.API.Entities.DTO
{
    public class ErrorResult
    {
        #region Property

        public AmisCode AmisCode { get; set; }
        /// <summary>
        /// Thông báo cho user. Không bắt buộc, tùy theo đặc thù từng dịch vụ và client tích hợp
        /// </summary>
        public object? UserMsg { get; set; }

        /// <summary>
        /// Thông báo cho Dev. Thông báo ngắn gọn để thông báo cho Dev biết vấn đề đang gặp phải
        /// </summary>
        public object? DevMsg { get; set; }

        /// <summary>
        /// Thông tin chi tiết hơn về vấn đề. Ví dụ: Đường dẫn mô tả về mã lỗi
        /// </summary>
        public string? MoreInfo { get; set; }

        /// <summary>
        /// Mã để tra cứu thông tin log: ELK, file log,...
        /// </summary>
        public string? TraceId { get; set; }

        #endregion

        #region Constructor
        public ErrorResult()
        {

        }
        public ErrorResult(AmisCode errorCode, object? userMsg, object? devMsg, string? moreInfo, string? traceId)
        {
            AmisCode = errorCode;
            UserMsg = userMsg;
            DevMsg = devMsg;
            MoreInfo = moreInfo;
            TraceId = traceId;
        }
        #endregion
    }
}
