using System.Data.SqlClient;

namespace K3MaterialGeneralTool.Task
{
    public class ConDb
    {
        /// <summary>
        /// 获取K3数据连接
        /// </summary>
        /// <returns></returns>
        public SqlConnection GetK3CloudConn()
        {
            var conn=new Conn();
            var sqlcon=new SqlConnection(conn.GetConnectionString(0));
            return sqlcon;
        }

        /// <summary>
        /// 获取Material连接
        /// </summary>
        /// <returns></returns>
        public SqlConnection GetMaterialConn()
        {
            var conn = new Conn();
            var sqlcon=new SqlConnection(conn.GetConnectionString(1));
            return sqlcon;
        }
    }
}
