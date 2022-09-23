﻿using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Misa.Web08.TCDN.API.Entities.DTO;
using Misa_Web08_TCDN_AnhDv_Api.Entities;
using Misa.Web08.TCDN.API.Enum;
using Misa_Web08_TCDN_AnhDv_Api.Entities.DTO;
using MySqlConnector;
using Swashbuckle.AspNetCore.Annotations;
using Misa.Web08.TCDN.API.Entities;

namespace Misa_Web08_TCDN_AnhDv_Api.Controllers
{
    [Route("api/v1/employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        /// <summary>
        /// Chuỗi kết nối đến Database
        /// </summary>        
        private const string mySqlconnectionString = "Server=localhost;Port=3306;Database=misa.web08.tcdn.dva;Uid=root;Pwd=12345678;";

        #region API DO GET
        /// <summary>
        /// API Lấy mã nhân viên mới tự động tăng
        /// </summary>
        /// <returns>Mã nhân viên mới tự động tăng</returns>
        /// Created by: TCDN AnhDV (16/09/2022)
        [HttpGet("new-employee-code")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(Employee))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public IActionResult GetNewEmployeeCode()
        {
            try
            {
                // Khởi tạo kết nối tới DB MySQL
                string connectionString = mySqlconnectionString;
                var mySqlConnection = new MySqlConnection(connectionString);

                // Chuẩn bị tên stored procedure
                string storedProcedureName = "Proc_Employee_GetMaxEmployeeCode";

                // Thực hiện gọi vào DB để chạy stored procedure ở trên
                string maxEmployeeCode = mySqlConnection.QueryFirstOrDefault<string>(storedProcedureName, commandType: System.Data.CommandType.StoredProcedure);

                // Xử lý sinh mã nhân viên mới tự động tăng
                // Cắt chuỗi mã nhân viên lớn nhất trong hệ thống để lấy phần số
                // Mã nhân viên mới = "NV" + Giá trị cắt chuỗi ở  trên + 1

                string codeNumber = maxEmployeeCode.Split("-")[1];

                string newEmployeeCode = $"NV-{(Int64.Parse(codeNumber) + 1)}".ToString();

                // Trả về dữ liệu cho client
                return StatusCode(StatusCodes.Status200OK, newEmployeeCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult(
                    AmisErrorCode.Exception,
                    "Catched an exception",
                    "Có lỗi xảy ra",
                    null,
                    HttpContext.TraceIdentifier));
            }
        }


        /// <summary>
        /// API Lấy thông tin chi tiết của 1 nhân viên
        /// </summary>
        /// <param name="employeeID">ID của nhân viên muốn lấy thông tin chi tiết</param>
        /// <returns>Đối tượng nhân viên muốn lấy thông tin chi tiết</returns>
        /// Created by: TCDN AnhDV (16/09/2022)
        [HttpGet("{employeeID}")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(Employee))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public IActionResult GetEmployeeByID([FromRoute] Guid employeeID)
        {
            try
            {
                // Khởi tạo kết nối tới DB MySQL
                string connectionString = mySqlconnectionString;
                var mySqlConnection = new MySqlConnection(connectionString);

                // Chuẩn bị tên Stored procedure
                string storedProcedureName = "Proc_Employee_GetEmployeeByID";

                // Chuẩn bị tham số đầu vào cho stored procedure
                var parameters = new DynamicParameters();
                parameters.Add("@$EmployeeID", employeeID);

                // Thực hiện gọi vào DB để chạy stored procedure với tham số đầu vào ở trên
                var employee = mySqlConnection.QueryFirstOrDefault<Employee>(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);

                // Xử lý kết quả trả về từ DB
                if (employee != null)
                {
                    return StatusCode(StatusCodes.Status200OK, employee);
                }
                else
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult(
                   AmisErrorCode.Exception,
                   "Catched an exception",
                   "Có lỗi xảy ra",
                   "https://openapi.amis.vn/api/v1/employees",
                   HttpContext.TraceIdentifier));
            }
        }

        /// <summary>
        /// API Lấy danh sách nhân viên cho phép lọc và phân trang
        /// </summary>
        /// <param name="search">Tìm kiếm theo Mã nhân viên, tên nhân viên</param>
        /// <param name="pageSize">Số trang muốn lấy</param>
        /// <param name="pageNumber">Thứ tự trang muốn lấy</param>
        /// <returns> Một đối tượng gồm:
        /// + Danh sách nhân viên thỏa mãn điều kiện lọc và phân trang
        /// + Tổng số nhân viên thỏa mãn điều kiện</returns>
        /// Created by: TCDN AnhDV (16/09/2022)
        [HttpGet("filter")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(PagingData<Employee>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public IActionResult FilterEmployees([FromQuery] string? search, [FromQuery] string? sort, [FromQuery] int pageSize = 10, [FromQuery] int pageNumber = 1)
        {
            try
            {
                // Khởi tạo kết nối tới DB MySQL
                string connectionString = mySqlconnectionString;
                var mySqlConnection = new MySqlConnection(connectionString);

                // Chuẩn bị tên Stored procedure
                string storedProcedureName = "Proc_Employee_GetPaging";

                // Chuẩn bị tham số đầu vào cho stored procedure
                var parameters = new DynamicParameters();
                parameters.Add("@v_Offset", (pageNumber - 1) * pageSize);
                parameters.Add("@v_Limit", pageSize);
                parameters.Add("@v_Sort", sort);


                var whereConditions = new List<string>();

                if (search != null)
                {
                    whereConditions.Add($"((EmployeeCode LIKE  \'%{search}%\' OR EmployeeName LIKE  \'%{search}%\'))");
                }
                string whereClause = string.Join(" AND ", whereConditions);
                parameters.Add("@v_Where", whereClause);

                // Thực hiện gọi vào DB để chạy stored procedure với tham số đầu vào ở trên
                var multipleResults = mySqlConnection.QueryMultiple(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);

                // Xử lý kết quả trả về từ DB
                if (multipleResults != null)
                {
                    var employees = multipleResults.Read<Employee>();
                    var totalCount = multipleResults.Read<long>().Single();
                    return StatusCode(StatusCodes.Status200OK, new PagingData<Employee>()
                    {
                        Data = employees.ToList(),
                        TotalCount = totalCount
                    });
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorCodeMs("e002"));
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult(
                   AmisErrorCode.Exception,
                   "Catched an exception",
                   "Có lỗi xảy ra",
                   "https://openapi.amis.vn/api/v1/employees",
                   HttpContext.TraceIdentifier));
            }
        }

        #endregion

        #region API DO DELETE
        /// <summary>
        /// API Xóa 1 nhân viên
        /// </summary>
        /// <param name="employeeID">ID của nhân viên muốn xóa</param>
        /// <returns>ID của nhân viên vừa xóa</returns>
        /// Created by: TCDN AnhDV (16/09/2022)
        [HttpDelete("{employeeID}")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(Guid))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteEmployeeByID([FromRoute] Guid employeeID)
        {
            try
            {
                // Khởi tạo kết nối tới DB MySQL
                string connectionString = mySqlconnectionString;
                var mySqlConnection = new MySqlConnection(connectionString);

                // Chuẩn bị câu lệnh DELETE
                string deleteEmployeeCommand = "DELETE FROM employee WHERE EmployeeID = @EmployeeID";

                // Chuẩn bị tham số đầu vào cho câu lệnh DELETE
                var parameters = new DynamicParameters();
                parameters.Add("@EmployeeID", employeeID);

                // Thực hiện gọi vào DB để chạy câu lệnh DELETE với tham số đầu vào ở trên
                int numberOfAffectedRows = mySqlConnection.Execute(deleteEmployeeCommand, parameters);

                // Xử lý kết quả trả về từ DB
                if (numberOfAffectedRows > 0)
                {
                    // Trả về dữ liệu cho client
                    return StatusCode(StatusCodes.Status200OK, employeeID);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorCodeMs("e002"));
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult(
                   AmisErrorCode.Exception,
                   "Catched an exception",
                   "Có lỗi xảy ra",
                   "https://openapi.amis.vn/api/v1/employees",
                   HttpContext.TraceIdentifier));
            }
        }
        #endregion

        #region API POST
        /// <summary>
        /// API Thêm mới 1 nhân viên
        /// </summary>
        /// <param name="employee">Đối tượng nhân viên muốn thêm mới</param>
        /// <returns>ID của nhân viên vừa thêm mới</returns>
        /// Created by: TCDN AnhDV (16/09/2022)
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, type: typeof(Guid))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public IActionResult InsertEmployee([FromBody] Employee employee)
        {
            try
            {
                // Khởi tạo kết nối tới DB MySQL
                string connectionString = mySqlconnectionString;
                var mySqlConnection = new MySqlConnection(connectionString);

                // Chuẩn bị tên Stored procedure
                string storedProcedureName = "Proc_Employee_InsertEmployee";
                // Chuẩn bị tham số đầu vào cho stored procedure
                var parameters = new DynamicParameters();
                var employeeID = Guid.NewGuid();
                parameters.Add("@$EmployeeID", employeeID);
                parameters.Add("@$EmployeeCode", employee.EmployeeCode);
                parameters.Add("@$EmployeeName", employee.EmployeeName);
                parameters.Add("@$DateOfBirth", employee.DateOfBirth);
                parameters.Add("@$Gender", employee.Gender);
                parameters.Add("@$EmployeeAddress", employee.EmployeeAddress);
                parameters.Add("@$DepartmentID", employee.DepartmentID);
                parameters.Add("@$JobTitle", employee.JobTitle);
                parameters.Add("@$IdentityNumber", employee.IdentityNumber);
                parameters.Add("@$IdentityDate", employee.IdentityDate);
                parameters.Add("@$IdentityPlace", employee.IdentityPlace);
                parameters.Add("@$PhoneNumber", employee.PhoneNumber);
                parameters.Add("@$TelephoneNumber", employee.TelephoneNumber);
                parameters.Add("@$Email", employee.Email);
                parameters.Add("@$BankAccountNumber", employee.BankAccountNumber);
                parameters.Add("@$BankName", employee.BankName);
                parameters.Add("@$BankBranch", employee.BankBranch);
                parameters.Add("@$IsCustomer", employee.IsCustomer);
                parameters.Add("@$IsSupplier", employee.IsSupplier);
                parameters.Add("@$CreatedDate", DateTime.Now);
                parameters.Add("@$CreatedBy", "Đặng Việt Anh");
                parameters.Add("@$ModifiedDate", DateTime.Now);
                parameters.Add("@$ModifiedBy", "Đặng Việt Anh");

                // Thực hiện gọi vào DB để chạy stored procedure với tham số đầu vào ở trên
                var rowEffects = mySqlConnection.Execute(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);

                if (rowEffects > 0)
                {
                    return StatusCode(StatusCodes.Status201Created, employeeID);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorCodeMs("e002"));
                }

            }
            catch (MySqlException mySqlException)
            {
                Console.WriteLine(mySqlException.Message);
                // TODO: Sau này có thể bổ sung log lỗi ở đây để khi gặp exception trace lỗi cho dễ
                if (mySqlException.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorCodeMs("e002"));
                }
                return StatusCode(StatusCodes.Status400BadRequest, new ErrorCodeMs("e001"));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult(
                   AmisErrorCode.Exception,
                   "Catched an exception",
                   "Có lỗi xảy ra",
                   "https://openapi.amis.vn/api/v1/employees",
                   HttpContext.TraceIdentifier));
            }
        }
        #endregion

        #region API PUT
        /// <summary>
        /// API Sửa 1 nhân viên
        /// </summary>
        /// <param name="employeeID">ID của nhân viên muốn sửa</param>
        /// <param name="employee">Đối tượng nhân viên muốn sửa</param>
        /// <returns>ID của nhân viên vừa sửa</returns>
        /// Created by: TCDN AnhDV (16/09/2022)
        [HttpPut("{employeeID}")]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(Guid))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateEmployee([FromRoute] Guid employeeID, [FromBody] Employee employee)
        {
            try
            {
                // Khởi tạo kết nối tới DB MySQL
                string connectionString = mySqlconnectionString;
                var mySqlConnection = new MySqlConnection(connectionString);

                // Chuẩn bị tên Stored procedure
                string storedProcedureName = "Proc_Employee_EditEmployee";

                // Chuẩn bị tham số đầu vào cho câu lệnh UPDATE
                var parameters = new DynamicParameters();

                parameters.Add("@$EmployeeID", employeeID);
                parameters.Add("@$EmployeeCode", employee.EmployeeCode);
                parameters.Add("@$EmployeeName", employee.EmployeeName);
                parameters.Add("@$DateOfBirth", employee.DateOfBirth);
                parameters.Add("@$Gender", employee.Gender);
                parameters.Add("@$EmployeeAddress", employee.EmployeeAddress);
                parameters.Add("@$DepartmentID", employee.DepartmentID);
                parameters.Add("@$JobTitle", employee.JobTitle);
                parameters.Add("@$IdentityNumber", employee.IdentityNumber);
                parameters.Add("@$IdentityDate", employee.IdentityDate);
                parameters.Add("@$IdentityPlace", employee.IdentityPlace);
                parameters.Add("@$PhoneNumber", employee.PhoneNumber);
                parameters.Add("@$TelephoneNumber", employee.TelephoneNumber);
                parameters.Add("@$Email", employee.Email);
                parameters.Add("@$BankAccountNumber", employee.BankAccountNumber);
                parameters.Add("@$BankName", employee.BankName);
                parameters.Add("@$BankBranch", employee.BankBranch);
                parameters.Add("@$IsCustomer", employee.IsCustomer);
                parameters.Add("@$IsSupplier", employee.IsSupplier);
                parameters.Add("@$ModifiedDate", DateTime.Now);
                parameters.Add("@$ModifiedBy", "Đặng Việt Anh");

                // Thực hiện gọi vào DB để chạy câu lệnh UPDATE với tham số đầu vào ở trên
                var rowEffects = mySqlConnection.Execute(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);

                // Xử lý kết quả trả về từ DB
                if (rowEffects > 0)
                {
                    // Trả về dữ liệu cho client
                    return StatusCode(StatusCodes.Status200OK, employeeID);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorCodeMs("e002"));
                }

            }
            catch (MySqlException mySqlException)
            {
                Console.WriteLine(mySqlException.Message);
                // TODO: Sau này có thể bổ sung log lỗi ở đây để khi gặp exception trace lỗi cho dễ
                if (mySqlException.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorCodeMs("e003"));
                }
                return StatusCode(StatusCodes.Status400BadRequest, new ErrorCodeMs("e001"));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult(
                   AmisErrorCode.Exception,
                   "Catched an exception",
                   "Có lỗi xảy ra",
                   "https://openapi.amis.vn/api/v1/employees",
                   HttpContext.TraceIdentifier));
            }
        }
        #endregion
    }
}
