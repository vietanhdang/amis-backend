namespace Misa.Web08.TCDN.API.Enum
{
    /// <summary>
    /// Lớp này dùng để liệt kê các mã lỗi của Amis API
    /// </summary>
    /// Created by: TCDN AnhDV (24/09/2022)
    public enum AmisCode
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
       DuplicateCode = 3,

    }
}
