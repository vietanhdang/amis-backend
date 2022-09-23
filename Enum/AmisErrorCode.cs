namespace Misa.Web08.TCDN.API.Enum
{
    /// <summary>
    /// Khai báo mã lỗi của API khi thực hiện các thao tác phát sinh lỗi xảy ra
    /// </summary>
    /// Created by: TCDN AnhDV (16/09/2022)
    public enum AmisErrorCode
    {
        /// <summary>
        /// Lỗi do exception chưa xác định được
        /// </summary>
        Exception = 1,

        /// <summary>
        /// Lỗi do validate dữ liệu thất bại
        /// </summary>
        Validate = 2,

        /// <summary>
        /// Lỗi do trùng mã
        /// </summary>
        DuplicateCode = 3
    }
}