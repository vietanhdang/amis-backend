namespace Misa.Web08.TCDN.API.Entities
{
    /// <summary>
    /// Lớp mã lỗi
    /// </summary>
    /// Created by: TCDN AnhDV (16/09/2022)
    public class ErrorCodeMs
    {
        public ErrorCodeMs(string messageCode)
        {
            this.MessageCode = messageCode;
        }
        public string MessageCode { get; set; }
    }
}
