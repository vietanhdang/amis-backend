namespace Misa.Web08.TCDN.API.Attributes
{
    /// <summary>
    /// Khai báo các Custom Attribute cho các property
    /// </summary>
    /// Created by: TCDN AnhDV (23/09/2022)
    public class AmisAttribute
    {
        /// <summary>
        /// Attribute dùng để xác định 1 property là khóa chính
        /// </summary>
        /// Created by: TCDN AnhDV (23/09/2022)
        [AttributeUsage(AttributeTargets.Property)]
        public class PrimaryKeyAttribute : Attribute
        {

        }
        /// <summary>
        /// Attribute dùng để validate các property không được phép null
        /// </summary>
        /// Created by: TCDN AnhDV (23/09/2022)
        [AttributeUsage(AttributeTargets.Property)]
        public class NotNullAttribute : Attribute
        {

            /// <summary>
            /// Khai báo field lưu thông báo lỗi
            /// </summary>
            readonly string errorMessage;

            /// <summary>
            /// Thông báo lỗi
            /// </summary>
            public string ErrorMessage { get => errorMessage; }


            /// <summary>
            /// Hàm khởi tạo
            /// </summary>
            /// <param name="ErrorMessage">Thông báo lỗi</param>
            /// Created by: TCDN AnhDV (23/09/2022)
            public NotNullAttribute(string errorMessage)
            {
                this.errorMessage = errorMessage;
            }
        }

        /// <summary>
        /// Attribute dùng để kiểm tra format của các property
        /// </summary>
        /// Created by: TCDN AnhDV (23/09/2022)
        [AttributeUsage(AttributeTargets.Property)]
        public class FormatAttribute : Attribute
        {
            /// <summary>
            /// Khai báo field lưu thông báo lỗi
            /// </summary>
            readonly string errorMessage;

            /// <summary>
            /// Thông báo lỗi
            /// </summary>
            public string ErrorMessage { get => errorMessage; }

            /// <summary>
            /// Khai báo field lưu regex
            /// </summary>
            readonly string regex;

            /// <summary>
            /// Regex
            /// </summary>
            public string Regex { get => regex; }

            /// <summary>
            /// Hàm khởi tạo
            /// </summary>
            /// <param name="ErrorMessage">Thông báo lỗi</param>
            /// <param name="Regex">Regex</param>
            /// Created by: TCDN AnhDV (23/09/2022)
            public FormatAttribute(string errorMessage, string regex)
            {
                this.errorMessage = errorMessage;
                this.regex = regex;
            }

        }
    }
}
