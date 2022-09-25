using System.Data;
using Dapper;
using MySqlConnector;
using static Dapper.SqlMapper;

namespace Misa.Web08.TCDN.API.DAO
{
    /// <summary>
    /// Lớp này dùng để thực hiện các thao tác với database
    /// </summary>
    /// Created by: TCDN AnhDV (24/09/2022)
    public class MySqlDataAccessHelper
    {
        /// <summary>
        /// Khởi tạo kết nối đến database
        /// </summary>
        /// <returns>Kết nối đến database</returns>
        /// Created by: TCDN AnhDV (24/09/2022)
        public static MySqlConnection GetConnection()
        {
            // Lấy chuỗi kết nối từ file appsettings.json
            var connectionString = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("DefaultConnection");
            // Khởi tạo đối tượng kết nối
            var connection = new MySqlConnection(connectionString);
            // Mở kết nối
            connection.Open();
            // Trả về kết nối
            return connection;
        }

        /// <summary>
        /// Lấy ra một bản ghi (có thể dùng procedure)
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu trả về</typeparam>
        /// <param name="sql">Câu lệnh sql hoặc tên procedure</param>
        /// <param name="parameters">Tham số truyền vào</param>
        /// <returns>Chi tiết bản ghi</returns>
        /// Created by: TCDN AnhDV (24/09/2022)
        public static T Get<T>(string sql, CommandType commandType = CommandType.Text, DynamicParameters? parameters = null)
        {
            // Khởi tạo đối tượng kết nối
            using (var connection = GetConnection())
            {
                // Thực hiện lấy dữ liệu
                var result = connection.QueryFirstOrDefault<T>(sql, param: parameters, commandType: commandType);
                // Trả về kết quả
                return result;
            }
        }

        /// <summary>
        /// Lấy ra danh sách bản ghi (có thể dùng procedure)
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu trả về</typeparam>
        /// <param name="sql">Câu lệnh sql hoặc tên procedure</param>
        /// <param name="parameters">Tham số truyền vào</param>
        /// <returns>Danh sách bản ghi</returns>
        /// Created by: TCDN AnhDV (24/09/2022)
        public static IEnumerable<T> GetList<T>(string sql, CommandType commandType = CommandType.Text, DynamicParameters? parameters = null)
        {
            // Khởi tạo đối tượng kết nối
            using (var connection = GetConnection())
            {
                // Thực hiện lấy dữ liệu
                var result = connection.Query<T>(sql, param: parameters, commandType: commandType);
                // Trả về kết quả
                return result;
            }
        }

        /// <summary>
        /// Trả về số bản ghi thực hiện được
        /// </summary>
        /// <param name="sql">Câu lệnh sql hoặc tên procedure</param>
        /// <param name="parameters">Tham số truyền vào</param>
        /// <returns>Số bản ghi thực hiện được</returns>
        /// Created by: TCDN AnhDV (24/09/2022)
        public static int Execute(string sql, CommandType commandType = CommandType.Text, DynamicParameters? parameters = null)
        {
            // Khởi tạo đối tượng kết nối
            using (var connection = GetConnection())
            {
                // Thực hiện lấy dữ liệu
                var result = connection.Execute(sql, param: parameters, commandType: commandType);
                // Trả về kết quả
                return result;
            }
        }

        /// <summary>
        /// Trả về nhiều object
        /// </summary>
        /// <param name="sql">Câu lệnh sql hoặc tên procedure</param>
        /// <param name="parameters">Tham số truyền vào</param>
        /// <param name="commandType">Kiểu câu lệnh</param>
        /// <param name="readerFuncs">Các hàm đọc dữ liệu</param>
        /// <returns>Danh sách bản ghi</returns>
        /// Created by: TCDN AnhDV (24/09/2022)
        public static List<object> GetMultiple(string sql, CommandType commandType = CommandType.Text, DynamicParameters? parameters = null, params Func<GridReader, object>[] readerFuncs)
        {
            // Khởi tạo danh sách trả về
            var list = new List<object>();

            // Khởi tạo đối tượng kết nối
            using (var connection = GetConnection())
            {
                // Thực hiện lấy dữ liệu
                var result = connection.QueryMultiple(sql, param: parameters, commandType: commandType);

                // Lặp qua các hàm đọc dữ liệu
                foreach (var readerFunc in readerFuncs)
                {
                    // Thực hiện đọc dữ liệu
                    var data = readerFunc(result);
                    // Thêm vào danh sách trả về
                    list.Add(data);
                }
            }
            // Trả về kết quả
            return list;
        }
    }
}
