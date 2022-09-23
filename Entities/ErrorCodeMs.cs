namespace Misa.Web08.TCDN.API.Entities
{
    public class ErrorCodeMs
    {
        public ErrorCodeMs(string messageCode)
        {
            this.MessageCode = messageCode;
        }
        public string MessageCode { get; set; }
    }
}
