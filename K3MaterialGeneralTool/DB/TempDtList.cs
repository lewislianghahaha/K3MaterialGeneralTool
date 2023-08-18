using System;
using System.Data;
using K3MaterialGeneralTool.Task;

namespace K3MaterialGeneralTool.DB
{
    public class TempDtList
    {
        Search searchDb=new Search();

        /// <summary>
        /// 历史记录临时表
        /// </summary>
        /// <returns></returns>
        public DataTable MakeHistoryTemp()
        {
            var dt=new DataTable();
            for (var i = 0; i < 8; i++)
            {
                var dc = new DataColumn();
                switch (i)
                {
                    //主键
                    case 0:
                        dc.ColumnName = "Fid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //物料编码
                    case 1:
                        dc.ColumnName = "FMaterialCode";
                        dc.DataType = Type.GetType("System.String"); 
                        break;
                    //物料名称
                    case 2:
                        dc.ColumnName = "FMaterialName";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //品牌
                    case 3:
                        dc.ColumnName = "FBi";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //规格型号
                    case 4:
                        dc.ColumnName = "FKui";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //是否成功(0:是 1:否)
                    case 5:
                        dc.ColumnName = "Finishid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //异常原因
                    case 6:
                        dc.ColumnName = "FRemark";
                        dc.DataType=Type.GetType("System.String");
                        break;
                    //导入时间
                    case 7:
                        dc.ColumnName = "ImportDt";
                        dc.DataType = Type.GetType("System.DateTime");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// 根据‘Excel绑定表’动态生成导入临时表
        /// </summary>
        /// <returns></returns>
        public DataTable MakeImportTempDt()
        {
            var coltypge = string.Empty;
            //创建最终结果临时表
            var resultdt=new DataTable();
            //获取‘T_MAT_BindExcelCol’表结构
            var dt = searchDb.Get_SearchExcelTemp().Copy();
            //循环生成列,并将列添加至dt表内
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                //创建新列名
                var dc = new DataColumn {ColumnName = Convert.ToString(dt.Rows[i][0])};
                //根据ExcelColDataType判断是取那个数据类型
                switch (Convert.ToString(dt.Rows[i][1]))
                {
                    case "字符串":
                        coltypge = "System.String"; 
                        break;
                    case "小数":
                        coltypge = "System.Decimal";
                        break;
                    case "整数":
                        coltypge = "System.Int32"; 
                        break;
                    case "日期":
                        coltypge = "System.DateTime"; 
                        break;
                }
                dc.DataType = Type.GetType(coltypge);
                resultdt.Columns.Add(dc);
            }
            return resultdt;
        }

        /// <summary>
        /// 创建绑定记录临时表
        /// </summary>
        /// <returns></returns>
        public DataTable MakeBindTempDt()
        {
            var dt = new DataTable();
            for (var i = 0; i < 4; i++)
            {
                var dc = new DataColumn();
                switch (i)
                {
                    //主键
                    case 0:
                        dc.ColumnName = "Fid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //Excel绑定字段主键ID
                    case 1:
                        dc.ColumnName = "ExcelColId";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //K3绑定字段主键ID
                    case 2:
                        dc.ColumnName = "K3ColId";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //绑定日期
                    case 3:
                        dc.ColumnName = "BindDt";
                        dc.DataType = Type.GetType("System.DateTime"); 
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// 创建EXCEL新字段临时表
        /// </summary>
        /// <returns></returns>
        public DataTable CreateNewExcelColTempdt()
        {
            var dt = new DataTable();
            for (var i = 0; i < 5; i++)
            {
                var dc = new DataColumn();
                switch (i)
                {
                    //主键
                    case 0:
                        dc.ColumnName = "ExcelColId";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //列名
                    case 1:
                        dc.ColumnName = "ExcelCol";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //列数据类型中文描述
                    case 2:
                        dc.ColumnName = "ExcelColDataType";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //是否绑定标记
                    case 3:
                        dc.ColumnName = "Bindid";
                        dc.DataType=Type.GetType("System.Int32");
                        break;
                    //创建时间
                    case 4:
                        dc.ColumnName = "CreateDt";
                        dc.DataType = Type.GetType("System.DateTime");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// 创建K3基础数据临时表
        /// </summary>
        /// <returns></returns>
        public DataTable CreateK3BasicSourceTempdt()
        {
            var dt = new DataTable();
            for (var i = 0; i < 4; i++)
            {
                var dc = new DataColumn();
                switch (i)
                {
                    //类型
                    case 0:
                        dc.ColumnName = "Typeid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //记录K3原来的主键值
                    case 1:
                        dc.ColumnName = "Id";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //名称
                    case 2:
                        dc.ColumnName = "FName";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //创建日期
                    case 3:
                        dc.ColumnName = "CreateDt";
                        dc.DataType = Type.GetType("System.DateTime"); 
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// 创建导入历史记录临时表(结果集)
        /// </summary>
        /// <returns></returns>
        public DataTable CreateHistoryRecordTempdt()
        {
            var dt = new DataTable();
            for (var i = 0; i < 8; i++)
            {
                var dc = new DataColumn();
                switch (i)
                {
                    //主键
                    case 0:
                        dc.ColumnName = "Fid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //物料编码
                    case 1:
                        dc.ColumnName = "FMaterialCode";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //物料名称
                    case 2:
                        dc.ColumnName = "FMaerialName";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //品牌
                    case 3:
                        dc.ColumnName = "FBi";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //规格型号
                    case 4:
                        dc.ColumnName = "FKui";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //是否成功
                    case 5:
                        dc.ColumnName = "Finishid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //异常原因
                    case 6:
                        dc.ColumnName = "FRemark";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //导入时间
                    case 7:
                        dc.ColumnName = "ImportDt";
                        dc.DataType = Type.GetType("System.DateTime");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// 创建生成结果临时表(结果集)
        /// </summary>
        /// <returns></returns>
        public DataTable CreateResultRecordTempdt()
        {
            var dt = new DataTable();
            for (var i = 0; i < 8; i++)
            {
                var dc = new DataColumn();
                switch (i)
                {
                    //主键
                    case 0:
                        dc.ColumnName = "Fid";
                        dc.DataType = Type.GetType("System.Int32");
                        break;
                    //物料编码
                    case 1:
                        dc.ColumnName = "FMaterialCode";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //物料名称
                    case 2:
                        dc.ColumnName = "FMaerialName";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //品牌
                    case 3:
                        dc.ColumnName = "FBi";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //规格型号
                    case 4:
                        dc.ColumnName = "FKui";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //是否成功(0:是,1:否)
                    case 5:
                        dc.ColumnName = "Finishid";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //异常原因
                    case 6:
                        dc.ColumnName = "FRemark";
                        dc.DataType = Type.GetType("System.String");
                        break;
                    //导入时间
                    case 7:
                        dc.ColumnName = "ImportDt";
                        dc.DataType = Type.GetType("System.DateTime");
                        break;
                }
                dt.Columns.Add(dc);
            }
            return dt;
        }

        /// <summary>
        /// 根据typeid动态生成对应的临时表(在生成T_BD_UNITCONVERTRATE时使用)
        /// </summary>
        /// <param name="typeid">类型标记;13:T_BD_UNITCONVERTRATE</param>
        /// <returns></returns>
        public DataTable CreateK3ImportTempDt(int typeid)
        {
            var dt = searchDb.Get_SearchMaterialSourceAndCreateTemp(typeid,0);
            return dt;
        }

    }
}
