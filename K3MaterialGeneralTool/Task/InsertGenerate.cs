using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using K3MaterialGeneralTool.DB;

namespace K3MaterialGeneralTool.Task
{
    //生成新物料 插入记录
    public class InsertGenerate
    {
        SqlList sqlList = new SqlList();
        Search search=new Search();
        TempDtList tempDtList=new TempDtList();
        Update update=new Update();

        private string _sqlscript = string.Empty;

        /// <summary>
        /// 将数据插入至T_MAT_BindRecord表内
        /// </summary>
        /// <param name="exceid"></param>
        /// <param name="k3Id"></param>
        /// <returns></returns>
        public bool InsertBindRecord(int exceid, int k3Id)
        {
            var result = true;
            try
            {
                //获取绑定记录临时表
                var bintempdt = tempDtList.MakeBindTempDt();
                //将相关记录插入至临时表
                var newrow = bintempdt.NewRow();
                newrow[1] = exceid;             //ExcelColId
                newrow[2] = k3Id;               //K3ColId
                newrow[3] = DateTime.Now.Date;  //BindDt
                bintempdt.Rows.Add(newrow);
                //执行插入操作
                ImportDtToDb("T_MAT_BindRecord", bintempdt);
                //完成后执行对T_MAT_BindExcelCol T_MAT_BindK3Col表中的BindId字段更新
                update.UpdateBindRecord(exceid,k3Id);
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 创建EXCEL新绑定字段使用
        /// </summary>
        /// <param name="colname">新字段名称</param>
        /// <param name="coldatatypename">新字段数据类型名称</param>
        /// <returns></returns>
        public bool InsertExcelNewCol(string colname,string coldatatypename)
        {
            var result = true;
            try
            {
                //获取绑定记录临时表
                var createtempdt = tempDtList.CreateNewExcelColTempdt();
                //将相关记录插入至临时表
                var newrow = createtempdt.NewRow();
                newrow[1] = colname;             //列名
                newrow[2] = coldatatypename;     //列数据类型中文描述
                newrow[3] = 1;                   //是否绑定标记(0:是 1:否)
                newrow[4] = DateTime.Now.Date;   //创建日期
                createtempdt.Rows.Add(newrow);
                //执行插入操作
                ImportDtToDb("T_MAT_BindExcelCol", createtempdt);
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 同步K3基础资料
        /// </summary>
        /// <returns></returns>
        public bool InsertK3SourceRecord()
        {
            var result = true;
            try
            {
                //获取‘导入K3基础资料’临时表
                var createk3Tempdt = tempDtList.CreateK3BasicSourceTempdt();
                //获取‘K3基础资料’数据源,并将数据源赋值至createk3Tempdt内
                 _sqlscript = sqlList.Get_SearchK3SourceRecord();
                var sourcedt = search.UseSqlSearchIntoDt(0, _sqlscript);
                //循环将数据插入至createk3Tempdt临时表内
                foreach (DataRow rows in sourcedt.Rows)
                {
                    var newrow = createk3Tempdt.NewRow();
                    newrow[0] = rows[0];        //Typeid
                    newrow[1] = rows[1];        //Id
                    newrow[2] = rows[2];        //FName
                    newrow[3] = rows[3];        //CreateDt
                    createk3Tempdt.Rows.Add(newrow);
                }
                //执行插入操作
                ImportDtToDb("T_MAT_Source", createk3Tempdt);
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 针对指定表进行数据插入
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dt">包含数据的临时表</param>
        private void ImportDtToDb(string tableName, DataTable dt)
        {
            try
            {
                var conn = new Conn();
                var sqlcon = conn.GetConnectionString(1);
                // sqlcon.Open(); 若返回一个SqlConnection的话,必须要显式打开 
                //注:1)要插入的DataTable内的字段数据类型必须要与数据库内的一致;并且要按数据表内的字段顺序 2)SqlBulkCopy类只提供将数据写入到数据库内
                using (var sqlBulkCopy = new SqlBulkCopy(sqlcon))
                {
                    sqlBulkCopy.BatchSize = 1000;                    //表示以1000行 为一个批次进行插入
                    sqlBulkCopy.DestinationTableName = tableName;  //数据库中对应的表名
                    sqlBulkCopy.NotifyAfter = dt.Rows.Count;      //赋值DataTable的行数
                    sqlBulkCopy.WriteToServer(dt);               //数据导入数据库
                    sqlBulkCopy.Close();                        //关闭连接 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



    }
}
