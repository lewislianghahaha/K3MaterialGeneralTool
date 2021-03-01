using System;
using System.Data;
using System.Data.SqlClient;
using K3MaterialGeneralTool.DB;
using NPOI.SS.Formula.Functions;

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
        /// 按照指定的SQL语句执行记录并返回执行结果（true 或 false）
        /// </summary>
        /// <param name="type">0:获取K3-CLOUD数据库 1:获取MaterialGeneral数据库</param>
        /// <param name="sqlscript"></param>
        /// <returns></returns>
        public bool Generdt(int type, string sqlscript)
        {
            var result = true;
            try
            {
                var sqlcon = type == 0 ? conDb.GetK3CloudConn() : conDb.GetMaterialConn();
                using (sqlcon)
                {
                    sqlcon.Open();
                    var sqlCommand=new SqlCommand(sqlscript,sqlcon);
                    sqlCommand.ExecuteNonQuery();
                    sqlcon.Close();
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
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
        public DataTable SearchHistoryRecord(string sdt, string edt, string fmaterialname, string fkui, string fbi)
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
        /// 根据导入过来的DT,查询并整理;若发现‘物料编码’已在DB内存在,即删除
        /// </summary>
        public DataTable SearchImportIdAndDel(DataTable importdt)
        {
            var numberlist = string.Empty;

            //循环获取import.rows[1]中的'物料编码'并放到T_BD_MATERIAL.FNUMBER进行查询,若有,即获取fmaterial并执行删除操作
            foreach (DataRow rows in importdt.Rows)
            {
                if (numberlist == "")
                {
                    numberlist = "'" + Convert.ToString(rows[1]) + "'";
                }
                else
                {
                    numberlist += ',' + "'" + Convert.ToString(rows[1]) + "'";
                }
            }

            //将numberlist进行查询,若存在即删除
            var deldt = UseSqlSearchIntoDt(0, sqlList.SearchMaterialFnumber(numberlist));

            return deldt;
        }


        #region 生成时所需相关方法

        /// <summary>
        /// 根据指定条件查询出“U订货商品分类”ERPcode信息
        /// </summary>
        /// <param name="oneLeverName">一级商品分类</param>
        /// <param name="twoLeverName">二级商品分类</param>
        /// <param name="dtlname">三级商品分类</param>
        /// <returns></returns>
        public DataTable SearchUProdceType(string oneLeverName, string twoLeverName, string dtlname)
        {
            _sqlscript = sqlList.Get_SearchUProdceType(oneLeverName,twoLeverName,dtlname);
            return UseSqlSearchIntoDt(1,_sqlscript);
        }

        /// <summary>
        /// 获取原漆K3记录(创建新物料时使用)
        /// </summary>
        /// <param name="fname"></param>
        /// <returns></returns>
        public DataTable SearchK3BinRecord(string fname)
        {
            _sqlscript = sqlList.Get_SearchK3BinRecord(fname);
            return UseSqlSearchIntoDt(0, _sqlscript);
        }

        /// <summary>
        /// 获取基础资料数据源(创建新物料时使用)
        /// </summary>
        /// <param name="typeid">类型ID,(0:品牌 1:分类 2:品类 3:组份 4:干燥性 5:常规/订制 6:阻击产品 7:颜色 8:系数 9:水性油性 10:原漆半成品属性 11:开票信息 12:研发类别
        ///13:包装罐(包装箱) 14:物料分组(辅助) 15:物料分组 16:存货类别 17:默认税率 18:基本单位)</param>
        /// <param name="fname">名称</param>
        /// <returns>近回ID值</returns>
        public DataTable SearchSourceRecord(int typeid, string fname)
        {
            _sqlscript = sqlList.Get_SearchSourceRecord(typeid, fname);
            return UseSqlSearchIntoDt(1, _sqlscript);
        }

        /// <summary>
        /// 根据‘品牌’及‘规格型号’查找出TOP 1的记录
        /// </summary>
        /// <param name="bin">品牌名称</param>
        /// <param name="kui">规格型号</param>
        /// <returns></returns>
        public DataTable SearchTop1MaterialRecord(string bin, string kui)
        {
            _sqlscript = sqlList.Get_SearchTop1MaterialRecord(bin, kui);
            return UseSqlSearchIntoDt(0, _sqlscript);
        }

        /// <summary>
        /// 根据typeid获取各表的主键值(一共14个)
        /// </summary>
        /// <param name="typeid">类型标记;0:T_BD_MATERIAL 1:T_BD_MATERIAL_L 2:t_BD_MaterialBase 3:t_BD_MaterialStock 4:t_BD_MaterialSale 
        ///                      5:t_bd_MaterialPurchase 6:t_BD_MaterialPlan 7:t_BD_MaterialProduce 8:t_BD_MaterialAuxPty 9:t_BD_MaterialInvPty 
        ///                      10:t_bd_MaterialSubcon 11:T_BD_MATERIALQUALITY 12:T_BD_UNITCONVERTRATE 13:T_MAT_ImportHistoryRecord_Key</param>
        /// <returns></returns>
        public int MakeDtidKey(int typeid)
        {
            //定义连接数据库ID
            var conid = typeid == 13 ? 1 : 0;
            _sqlscript = sqlList.Get_MakeDtidKey(typeid);
            return Convert.ToInt32(UseSqlSearchIntoDt(conid,_sqlscript).Rows[0][0]);
        }

        /// <summary>
        /// 1)根据fmaterialid查询出数据源 2)用于动态生成临时表(最后更新及插入使用)此点只适合T_BD_UNITCONVERTRATE使用
        /// </summary>
        /// <param name="typeid">类型标记;0:T_BD_MATERIAL 1:T_BD_MATERIAL_L 2:t_BD_MaterialBase 3:t_BD_MaterialStock 4:t_BD_MaterialSale 
        ///                      5:t_bd_MaterialPurchase 6:t_BD_MaterialPlan 7:t_BD_MaterialProduce 8:t_BD_MaterialAuxPty 9:t_BD_MaterialInvPty 
        ///                      10:t_bd_MaterialSubcon 11:T_BD_MATERIALQUALITY 12:T_BD_UNITCONVERTRATE</param>
        /// <param name="fmaterialid"></param>
        /// <returns></returns>
        public DataTable Get_SearchMaterialSourceAndCreateTemp(int typeid, int fmaterialid)
        {
            _sqlscript = sqlList.Get_SearchMaterialSource(typeid, fmaterialid);
            return UseSqlSearchIntoDt(0, _sqlscript);
        }

        /// <summary>
        /// 创建(更新)T_BAS_BILLCODES(编码规则最大编码表)中的相关记录
        /// </summary>
        /// <param name="fmaterialnumber"></param>
        /// <returns></returns>
        public bool Get_MakeUnitKey(string fmaterialnumber)
        {
            _sqlscript = sqlList.Get_MakeUnitKey(fmaterialnumber);
            return Generdt(0, _sqlscript);
        }

        /// <summary>
        /// 根据物料编码在‘编码索引表’内查询出FNUMMAX值,以此构建‘物料单位换算’中FBILLNO的组成部份
        /// 注:传输过来的物料编码只截取前8位（0~7） 并且传过来的值必须为HAHAHAHA{{{{{0}}} 这种格式
        /// </summary>
        /// <param name="fmaterialnumber"></param>
        /// <returns></returns>
        public string SearchUnitMaxKey(string fmaterialnumber)
        {
            _sqlscript = sqlList.SearchUnitMaxKey(fmaterialnumber);
            return Convert.ToString(UseSqlSearchIntoDt(0, _sqlscript).Rows[0][0]);
        }

        /// <summary>
        /// 根据新物料ID,将相关表进行删除
        /// </summary>
        /// <returns></returns>
        public bool DelNewMaterialRecord(int fmaterialid)
        {
            _sqlscript = sqlList.DelNewMaterialRecord(fmaterialid);
            return Generdt(0, _sqlscript);
        }

        /// <summary>
        /// 初始化获取罐箱相关明细(创建新记录时使用)
        /// </summary>
        /// <returns></returns>
        public DataTable GetGuanXuanDtl()
        {
            _sqlscript = sqlList.GetGuanXuanDtl();
            return UseSqlSearchIntoDt(0, _sqlscript);
        }

        #endregion
    }
}
