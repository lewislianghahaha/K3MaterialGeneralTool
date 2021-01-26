using System;
using System.Data;

namespace K3MaterialGeneralTool.DB
{
    public class TempDtList
    {
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



    }
}
