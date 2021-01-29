using System;
using System.Data;
using System.Data.SqlClient;
using K3MaterialGeneralTool.DB;

namespace K3MaterialGeneralTool.Task
{
    //查询
    public class Search
    {
        SqlList sqlList=new SqlList();
        ConDb conDb=new ConDb();

        private string _sqlscript = string.Empty;

        /// <summary>
        /// 根据SQL语句查询得出对应的DT(公共方法)
        /// </summary>
        /// <param name="type">0:获取K3-CLOUD数据库 1:获取MaterialGeneral数据库</param>
        /// <param name="sqlscript">SQL语句</param>
        /// <returns></returns>
        public DataTable UseSqlSearchIntoDt(int type, string sqlscript)
        {
            var resultdt = new DataTable();

            try
            {
                var sqlcon = type == 0 ? conDb.GetK3CloudConn() : conDb.GetMaterialConn();
                var sqlDataAdapter = new SqlDataAdapter(sqlscript, sqlcon);
                sqlDataAdapter.Fill(resultdt);
            }
            catch (Exception)
            {
                resultdt.Rows.Clear();
                resultdt.Columns.Clear();
            }
            return resultdt;
        }

        /// <summary>
        /// 获取Excel绑定表记录(导入EXCEL时动态生成临时表使用)
        /// </summary>
        /// <returns></returns>
        public DataTable SearchExcelBindRecord()
        {
            _sqlscript = sqlList.Get_SearchBindExcelCol();
            return UseSqlSearchIntoDt(1,_sqlscript);
        }

        /// <summary>
        /// K3字段绑定记录(在'绑定'功能显示及更新时使用)
        /// </summary>
        /// <returns></returns>
        public DataTable SearchK3BindRecord(int typeid)
        {
            _sqlscript = sqlList.Get_SearchBindK3Col(typeid);
            return UseSqlSearchIntoDt(1, _sqlscript);
        }

        /// <summary>
        /// 绑定关系
        /// </summary>
        /// <returns></returns>
        public DataTable SearchBind()
        {
            _sqlscript = sqlList.Get_SearchBind();
            return UseSqlSearchIntoDt(1, _sqlscript);
        }

        /// <summary>
        /// 查询历史记录
        /// </summary>
        /// <returns></returns>
        public DataTable SearchHistoryRecord(DateTime sdt, DateTime edt, string fmaterialname, string fkui, string fbi)
        {
            _sqlscript = sqlList.Get_SearchHistoryRecord(sdt,edt,fmaterialname,fkui,fbi);
            return UseSqlSearchIntoDt(1, _sqlscript);
        }



    }
}
