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
        /// 获取Excel绑定表记录()
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

        /// <summary>
        /// 导入EXCEL时动态生成临时表使用
        /// </summary>
        /// <returns></returns>
        public DataTable Get_SearchExcelTemp()
        {
            _sqlscript = sqlList.Get_SearchExcelTemp();
            return UseSqlSearchIntoDt(1,_sqlscript);
        }

        /// <summary>
        /// 1)根据fmaterialid查询出数据源 2)用于动态生成临时表(最后更新及插入使用)
        /// </summary>
        /// <param name="typeid">类型标记;0:T_BD_MATERIAL 1:T_BD_MATERIAL_L 2:t_BD_MaterialBase 3:t_BD_MaterialStock 4:t_BD_MaterialSale 
        ///                      5:t_bd_MaterialPurchase 6:t_BD_MaterialPlan 7:t_BD_MaterialProduce 8:t_BD_MaterialAuxPty 9:t_BD_MaterialInvPty 
        ///                      10:t_bd_MaterialSubcon 11:T_BD_MATERIALQUALITY 12:T_BD_UNITCONVERTRATE</param>
        /// <param name="fmaterialid"></param>
        /// <returns></returns>
        public DataTable Get_SearchMaterialSourceAndCreateTemp(int typeid, int fmaterialid)
        {
            _sqlscript = sqlList.Get_SearchMaterialSource(typeid, fmaterialid);
            return UseSqlSearchIntoDt(0,_sqlscript);
        }

    }
}
