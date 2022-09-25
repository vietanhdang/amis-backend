using Microsoft.AspNetCore.Mvc;
using Misa_Web08_TCDN_AnhDv_Api.Entities;
using Swashbuckle.AspNetCore.Annotations;
using Dapper;
using MySqlConnector;
using Misa.Web08.TCDN.API.DAO;
using Misa.Web08.TCDN.API.Enum;
using Misa.Web08.TCDN.API.Properties;
using Misa.Web08.TCDN.API.Entities.DTO;

namespace Misa_Web08_TCDN_AnhDv_Api.Controllers
{
    /// <summary>
    /// Các API liên quan đến phòng ban
    /// </summary>
    /// Created by: TCDN AnhDV (16/09/2022)
    [Route("api/v1/departments")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        /// <summary>
        /// API Lấy toàn bộ danh sách phòng ban
        /// </summary>
        /// <returns>Danh sách phòng ban</returns>
        /// <response code="200">Trả về danh sách phòng ban</response>
        /// Created by: TCDN AnhDV (16/09/2022)
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(List<Department>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAllDepartments()
        {
            try
            {
                // Chuẩn bị câu lệnh truy vấn
                string getAllDepartmentsCommand = "SELECT * FROM department;";

                // Thực hiện gọi vào DB để chạy câu lệnh truy vấn ở trên
                var departments = MySqlDataAccessHelper.GetList<Department>(getAllDepartmentsCommand);

                // Trả về dữ liệu cho client
                if (departments != null)
                {
                    return StatusCode(StatusCodes.Status200OK, departments);
                }
                else
                {
                    return StatusCode(StatusCodes.Status204NoContent);
                }
            }
            catch (Exception)
            {
                // Trả về ngoại lệ cho client
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult(
                       AmisCode.Exception,
                       Resource.UserMsg_Exception,
                       Resource.DevMsg_Exception,
                       Resource.MoreInfo_Exception,
                       HttpContext.TraceIdentifier
                   ));
            }
        }
    }
}
