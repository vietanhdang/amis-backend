using Misa_Web08_TCDN_AnhDv_Api.Enum;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;
using static Misa.Web08.TCDN.API.Attributes.AmisAttribute;

namespace Misa_Web08_TCDN_AnhDv_Api.Entities
{
    /// <summary>
    /// Bảng nhân viên
    /// </summary>
    /// Created by: TCDN AnhDV (16/09/2022)
    public class Employee
    {
        /// <summary>
        /// ID nhân viên
        /// </summary>
        public Guid? EmployeeID { get; set; }

        /// <summary>
        /// Mã nhân viên
        /// </summary>
        [NotNullAttribute("Mã nhân viên không được phép để trống")]
        [FormatAttribute("Mã nhân viên không đúng định dạng", @"^NV-[0-9]{1,}$")]
        public string? EmployeeCode { get; set; }

        /// <summary>
        /// Tên nhân viên
        /// </summary>
        // [Required(ErrorMessage = "e005")]
        [NotNullAttribute("Tên nhân viên không được để trống")]
        public string? EmployeeName { get; set; }

        /// <summary>
        /// Ngày sinh
        /// </summary>

        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Địa chỉ
        /// </summary>
        public string? EmployeeAddress { get; set; }

        /// <summary>
        /// Giới tính
        /// </summary>
        public Gender? Gender { get; set; }

        /// <summary>
        /// ID phòng ban
        /// </summary>
        [NotNullAttribute("Đơn vị không được để trống")]
        public Guid DepartmentID { get; set; }

        /// <summary>
        /// Tên phòng ban
        /// </summary>
        public string? DepartmentName { get; set; }


        /// <summary>
        /// Chức danh
        /// </summary>
        public string? JobTitle { get; set; }

        /// <summary>
        /// Số CMND
        /// </summary>
        // [Required(ErrorMessage = "e006")]
        public string? IdentityNumber { get; set; }

        /// <summary>
        /// Ngày cấp CMND
        /// </summary>
        public DateTime? IdentityDate { get; set; }

        /// <summary>
        /// Nơi cấp CMND
        /// </summary>
        public string? IdentityPlace { get; set; }

        /// <summary>
        /// Số điện thoại
        /// </summary>
        // [Required(ErrorMessage = "e008")]
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Số điện thoại cố định
        /// </summary>
        //[Required(ErrorMessage = "e008")]
        public string? TelephoneNumber { get; set; }


        /// <summary>
        /// Email
        /// </summary>
        // [Required(ErrorMessage = "e007")]
        // [EmailAddress(ErrorMessage = "e009")]
        public string? Email { get; set; }

        /// <summary>
        /// Số tài khoản ngân hàng
        /// </summary>
        public string? BankAccountNumber { get; set; }

        /// <summary>
        /// Tên ngân hàng
        /// </summary>
        public string? BankName { get; set; }

        /// <summary>
        /// Chi nhánh ngân hàng
        /// </summary>
        public string? BankBranch { get; set; }

        /// <summary>
        /// Là Khách hàng
        /// </summary>
        public bool? IsCustomer { get; set; }

        /// <summary>
        /// Là nhà cung cấp
        /// </summary>
        public bool? IsSupplier { get; set; }

        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime? CreatedDate { get; set; }

        /// <summary>
        /// Người tạo
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Ngày sửa gần nhất
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Người sửa gần nhất
        /// </summary>
        public string? ModifiedBy { get; set; }

        /// <summary>
        /// Validate dữ liệu
        /// </summary>
        /// <param name="erorrMsg">Thông báo lỗi</param>
        /// <returns>Trả về true nếu dữ liệu hợp lệ</returns>
        /// Created by: TCDN AnhDV (24/09/2022)
        public bool IsValid(out List<string> erorrMsg)
        {
            erorrMsg = new List<string>();
            var isValid = true;
            var properties = this.GetType().GetProperties();
            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(this);
                var propertyAttributes = property.GetCustomAttributes();
                foreach (var propertyAttribute in propertyAttributes)
                {
                    if (propertyAttribute is NotNullAttribute)
                    {
                        if (string.IsNullOrEmpty(propertyValue?.ToString()))
                        {

                            isValid = false;
                            erorrMsg.Add(((NotNullAttribute)propertyAttribute).ErrorMessage);
                        }
                    }
                    if (propertyAttribute is FormatAttribute)
                    {
                        if (!string.IsNullOrEmpty(propertyValue?.ToString()))
                        {

                            if (!Regex.IsMatch(propertyValue.ToString(), ((FormatAttribute)propertyAttribute).Regex))
                            {
                                isValid = false;
                                erorrMsg.Add(((FormatAttribute)propertyAttribute).ErrorMessage);
                            }
                        }
                    }
                }
            }
            return isValid;
        }
    }
}

