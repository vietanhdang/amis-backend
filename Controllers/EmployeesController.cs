using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Misa.Web08.TCDN.API.Entities.DTO;
using Misa_Web08_TCDN_AnhDv_Api.Entities;
using Misa.Web08.TCDN.API.Enum;
using Misa_Web08_TCDN_AnhDv_Api.Entities.DTO;
using MySqlConnector;
using Swashbuckle.AspNetCore.Annotations;
using Misa.Web08.TCDN.API.Entities;
using System.ComponentModel.DataAnnotations;
using Misa.Web08.TCDN.API.DAO;
using Misa.Web08.TCDN.API.Properties;

namespace Misa_Web08_TCDN_AnhDv_Api.Controllers
{
    /// <summary>
    /// Các API liên quan đến thêm sửa xóa nhân viên
    /// </summary>
    /// Created by: TCDN AnhDV (16/09/2022)
    [Route("api/v1/employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {

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
                // Chuẩn bị tên stored procedure
                string storedProcedureName = "Proc_Employee_GetMaxEmployeeCode";

                // Thực hiện gọi vào DB để chạy stored procedure ở trên
                string maxEmployeeCode = MySqlDataAccessHelper.Get<string>(storedProcedureName, System.Data.CommandType.StoredProcedure);

                // Mã nhân viên mới = "NV-" + Giá trị nhân viên lớn nhất đã + 1 trong procedure

                string newEmployeeCode = $"NV-{maxEmployeeCode}";

                // Trả về dữ liệu cho client
                return StatusCode(StatusCodes.Status200OK, newEmployeeCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult(
                    AmisCode.Exception,
                    Resource.UserMsg_Exception,
                    Resource.DevMsg_Exception,
                    Resource.MoreInfo_Exception,
                    HttpContext.TraceIdentifier)
                );
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


                // Chuẩn bị tên Stored procedure
                string storedProcedureName = "Proc_Employee_GetEmployeeByID";

                // Chuẩn bị tham số đầu vào cho stored procedure
                var parameters = new DynamicParameters();
                parameters.Add("@v_EmployeeID", employeeID);

                // Thực hiện gọi vào DB để chạy stored procedure với tham số đầu vào ở trên
                var employee = MySqlDataAccessHelper.Get<Employee>(storedProcedureName, System.Data.CommandType.StoredProcedure, parameters);

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
                    AmisCode.Exception,
                    Resource.UserMsg_Exception,
                    Resource.DevMsg_Exception,
                    Resource.MoreInfo_Exception,
                    HttpContext.TraceIdentifier
                ));
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
                var employees = MySqlDataAccessHelper.GetMultiple(storedProcedureName, System.Data.CommandType.StoredProcedure, parameters, (employee) => employee.Read<Employee>(), (total) => total.Read<long>().FirstOrDefault());

                if (employees != null)
                {
                    // Xử lý kết quả trả về từ DB
                    var pagingData = new PagingData<Employee>()
                    {
                        Data = (List<Employee>)employees[0],
                        TotalCount = (long)employees[1],
                    };

                    // Trả về kết quả cho client nếu có dữ liệu
                    return StatusCode(StatusCodes.Status200OK, pagingData);
                }
                else
                {
                    // nếu không có dữ liệu thì trả về 204 No Content
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception ex)
            {
                // Nếu có lỗi thì trả về 500
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult(
                    AmisCode.Exception,
                    Resource.UserMsg_Exception,
                    Resource.DevMsg_Exception,
                    Resource.MoreInfo_Exception,
                    HttpContext.TraceIdentifier
                ));
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

                // Chuẩn bị câu lệnh Prepared Statement
                string storedProcedureName = "Proc_Employee_DeleteEmployeeByID";

                // Chuẩn bị tham số đầu vào cho câu lệnh DELETE
                var parameters = new DynamicParameters();
                parameters.Add("@v_EmployeeID", employeeID);

                // Thực hiện gọi vào DB để chạy câu lệnh DELETE với tham số đầu vào ở trên
                int numberOfAffectedRows = MySqlDataAccessHelper.Execute(storedProcedureName, System.Data.CommandType.StoredProcedure, parameters);

                // Xử lý kết quả trả về từ DB
                if (numberOfAffectedRows > 0)
                {
                    // Trả về dữ liệu cho client
                    return StatusCode(StatusCodes.Status200OK, employeeID);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult(
                         AmisCode.Delete,
                         Resource.UserMsg_Delete_Failed,
                         Resource.DevMsg_Delete_Failed,
                         Resource.MoreInfo_Exception,
                         HttpContext.TraceIdentifier
                     ));
                }
            }
            catch (Exception exception)
            {
                // Nếu có lỗi exception thì trả về 500
                Console.WriteLine(exception.Message);

                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult(
                    AmisCode.Exception,
                    Resource.UserMsg_Exception,
                    Resource.DevMsg_Exception,
                    Resource.MoreInfo_Exception,
                    HttpContext.TraceIdentifier
                ));
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
                // Validate dữ liệu
                List<string> errorMessages = new List<string>();

                if (!employee.IsValid(out errorMessages))
                {
                    // Trả về lỗi cho client nếu các trường dữ liệu không hợp lệ
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult(
                        AmisCode.Validate,
                        errorMessages.ToList(),
                        Resource.DevMsg_Validate_Failed,
                        Resource.MoreInfo_Exception,
                        HttpContext.TraceIdentifier
                    ));
                }

                // Chuẩn bị tên Stored procedure
                string storedProcedureName = "Proc_Employee_InsertEmployee";

                // Chuẩn bị tham số đầu vào cho stored procedure
                var parameters = new DynamicParameters();
                var employeeID = Guid.NewGuid();

                // lấy ra các property của đối tượng employee
                var properties = employee.GetType().GetProperties();

                foreach (var property in properties)
                {
                    // Lấy ra tên của property
                    var propertyName = property.Name;

                    // Lấy ra giá trị của property
                    var propertyValue = property.GetValue(employee);

                    // Thêm tham số đầu vào cho stored procedure
                    parameters.Add($"@v_{propertyName}", propertyValue);
                }

                // set lại giá trị cho EmployeeID
                parameters.Add("@v_EmployeeID", employeeID);
                // set giá trị cho CreatedDate
                parameters.Add("@v_CreatedDate", DateTime.Now);
                // set giá trị cho ModifiedDate
                parameters.Add("@v_ModifiedDate", DateTime.Now);
                // set giá trị cho CreatedBy
                parameters.Add("@v_CreatedBy", "Đặng Việt Anh");
                // set giá trị cho ModifiedBy
                parameters.Add("@v_ModifiedBy", "Đặng Việt Anh");
                // Thực hiện gọi vào DB để chạy stored procedure với tham số đầu vào ở trên
                var rowEffects = MySqlDataAccessHelper.Execute(storedProcedureName, System.Data.CommandType.StoredProcedure, parameters);

                if (rowEffects > 0)
                {
                    return StatusCode(StatusCodes.Status201Created, employeeID);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult(
                          AmisCode.Insert,
                          Resource.UserMsg_Insert_Failed,
                          Resource.DevMsg_Insert_Failed,
                          Resource.MoreInfo_Exception,
                          HttpContext.TraceIdentifier
                      ));
                }

            }
            catch (MySqlException mySqlException)
            {
                Console.WriteLine(mySqlException.Message);

                // trả về lỗi trùng mã cho client
                if (mySqlException.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult(
                         AmisCode.DuplicateCode,
                         Resource.UserMsg_Insert_Duplicate,
                         Resource.DevMsg_Insert_Duplicate,
                         Resource.MoreInfo_Exception,
                         HttpContext.TraceIdentifier
                     ));
                }

                return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult(
                         AmisCode.Insert,
                         Resource.UserMsg_Insert_Failed,
                         Resource.DevMsg_Insert_Failed,
                         Resource.MoreInfo_Exception,
                         HttpContext.TraceIdentifier
                     ));
            }
            catch (Exception exception)
            {
                // trả về lỗi không xác định cho client
                Console.WriteLine(exception.Message);

                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult(
                    AmisCode.Exception,
                    Resource.UserMsg_Exception,
                    Resource.DevMsg_Exception,
                    Resource.MoreInfo_Exception,
                    HttpContext.TraceIdentifier
                ));
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
                // Validate dữ liệu
                List<string> errorMessages = new List<string>();

                if (!employee.IsValid(out errorMessages))
                {
                    // Trả về lỗi cho client nếu các trường dữ liệu không hợp lệ
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult(
                        AmisCode.Validate,
                        errorMessages.ToList(),
                        Resource.DevMsg_Validate_Failed,
                        Resource.MoreInfo_Exception,
                        HttpContext.TraceIdentifier
                    ));
                }

                // Chuẩn bị tên Stored procedure
                string storedProcedureName = "Proc_Employee_EditEmployee";

                // Chuẩn bị tham số đầu vào cho câu lệnh UPDATE
                var parameters = new DynamicParameters();

                // lấy ra các property của đối tượng employee
                var properties = employee.GetType().GetProperties();

                foreach (var property in properties)
                {
                    // Lấy ra tên của property
                    var propertyName = property.Name;

                    // Lấy ra giá trị của property
                    var propertyValue = property.GetValue(employee);

                    if (propertyName == "CreatedDate" || propertyName == "CreatedBy")
                    {
                        continue;
                    }

                    // Thêm tham số đầu vào cho stored procedure
                    parameters.Add($"@v_{propertyName}", propertyValue);
                }

                // set lại giá trị cho EmployeeID
                parameters.Add("@v_EmployeeID", employeeID);

                // set lại giá trị cho ModifiedDate
                parameters.Add("@v_ModifiedDate", DateTime.Now);

                // set giá trị cho ModifiedBy
                parameters.Add("@v_ModifiedBy", "Đặng Việt Anh");

                // Thực hiện gọi vào DB để chạy câu lệnh UPDATE với tham số đầu vào ở trên
                var rowEffects = MySqlDataAccessHelper.Execute(storedProcedureName, System.Data.CommandType.StoredProcedure, parameters);

                // Xử lý kết quả trả về từ DB
                if (rowEffects > 0)
                {
                    return StatusCode(StatusCodes.Status200OK, employeeID);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult(
                         AmisCode.Update,
                         Resource.UserMsg_Update_Failed,
                         Resource.DevMsg_Update_Failed,
                         Resource.MoreInfo_Exception,
                         HttpContext.TraceIdentifier
                     ));
                }
            }
            catch (MySqlException mySqlException)
            {
                Console.WriteLine(mySqlException.Message);

                if (mySqlException.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult(
                         AmisCode.DuplicateCode,
                         Resource.UserMsg_Insert_Duplicate,
                         Resource.DevMsg_Insert_Duplicate,
                         Resource.MoreInfo_Exception,
                         HttpContext.TraceIdentifier
                     ));
                }
                return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult(
                          AmisCode.Update,
                          Resource.UserMsg_Update_Failed,
                          Resource.DevMsg_Update_Failed,
                          Resource.MoreInfo_Exception,
                          HttpContext.TraceIdentifier
                      ));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult(
                    AmisCode.Exception,
                    Resource.UserMsg_Exception,
                    Resource.DevMsg_Exception,
                    Resource.MoreInfo_Exception,
                    HttpContext.TraceIdentifier
                ));
            }
        }
        #endregion
    }
}
