using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Misa_Web08_TCDN_AnhDv_Api.Entities;
using Swashbuckle.AspNetCore.Annotations;
using Dapper;
using MySqlConnector;

namespace Misa_Web08_TCDN_AnhDv_Api.Controllers
{
    [Route("api/v1/departments")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        /// <summary>
        /// Chuỗi kết nối đến Database
        /// </summary>        
        private const string mySqlconnectionString = "Server=localhost;Port=3306;Database=misa.web08.tcdn.dva;Uid=root;Pwd=12345678;";

        // private const string  mySqlconnectionString = "Server=3.0.89.182;Port=3306;Database= WDT.2022.PCTUANANH;Uid=dev;Pwd=12345678;";


        /// <summary>
        /// API Lấy toàn bộ danh sách phòng ban
        /// </summary>
        /// <returns>Danh sách phòng ban</returns>
        // add code description for swagger
        [SwaggerOperation(Summary = "Lấy toàn bộ danh sách phòng ban", Description = "Lấy toàn bộ danh sách phòng ban", Tags = new[] { "Departments" })]
        /// <response code="200">Trả về danh sách phòng ban</response>
        /// Created by: TCDN AnhDV (16/09/2022)
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(List<Department>))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAllPositions()
        {
            try
            {
                // Khởi tạo kết nối tới DB MySQL
                string connectionString = mySqlconnectionString;

                var mySqlConnection = new MySqlConnection(connectionString);

                // Chuẩn bị câu lệnh truy vấn
                string getAllDepartmentsCommand = "SELECT * FROM department;";

                // Thực hiện gọi vào DB để chạy câu lệnh truy vấn ở trên
                var departments = mySqlConnection.Query<Department>(getAllDepartmentsCommand);

                // Trả về dữ liệu cho client
                return StatusCode(StatusCodes.Status200OK, departments);
            }
            catch (Exception)
            {
                // TODO: Sau này có thể bổ sung log lỗi ở đây để khi gặp exception trace lỗi cho dễ
                return StatusCode(StatusCodes.Status400BadRequest, "e001");
            }
        }
    }
}
