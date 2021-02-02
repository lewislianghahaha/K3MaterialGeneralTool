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
        //记录生成时的记录集(注:最后将此记录集插入至GlobalClasscs.RDt.Resultdt内)
        private DataTable _resultdt;   

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
                result = update.UpdateBindRecord(exceid,k3Id);
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

        #region 生成K3新物料相关

        /// <summary>
        /// 生成K3新物料
        /// </summary>
        /// <param name="importdt"></param>
        /// <returns></returns>
        public bool GenerateAndCreateK3NewMaterialRecord(DataTable importdt)
        {
            var result = true;
            var loopmark = true;        //子循环使用(检测各表是否正常生成,若在循环返回的结果为false,即不能执行‘单位找算’相关代码)
            var dtname = string.Empty; //记录各表名信息
            var newmaterialid = 0;    //记录新fmaterialid

            try
            {
                //定义历史记录表(结果集)临时表
                _resultdt = tempDtList.CreateHistoryRecordTempdt();
                //获取绑定记录表
                var binddt = search.SearchBind();

                //循环读取importdt内的记录
                foreach (DataRow rows in importdt.Rows)
                {
                    //根据‘品牌’及‘规格型号’获取K3物料中TOP 1的Fmaterialid(注:若此没有记录,即马上跳出当前循环)
                    var oldmaterialid = search.SearchTop1MaterialRecord(Convert.ToString(rows[0]),Convert.ToString(rows[4]));
                    if (oldmaterialid == 0)
                    {
                        CreateGenerateRecord(rows, 1, $@"没有在K3中找到品牌:'{Convert.ToString(rows[0])}'及规格型号:'{Convert.ToString(rows[4])}'的相关记录,
                                                         故不能自动生成新物料,请在K3进行手动创建");
                        continue;
                    }
                    //获取EXCEL临时表并将rows记录插入
                    var exceltempdt = ImportExcelTempdt(rows);

                    //若找到oldmaterialid相关值,即获取关于此oldmaterialid的相关表格信息
                    //循环从0~12;分别针对不同表进行生成KEY 插入内容等相关操作
                    for (var i = 0; i < 12; i++)
                    {
                        //根据循环的i值及oldmaterialid获取数据源
                        var materialdt = search.Get_SearchMaterialSourceAndCreateTemp(i, oldmaterialid);
                        //根据循环的i值动态生成对应的临时表
                        var tempdt = tempDtList.CreateK3ImportTempDt(i, oldmaterialid);
                        //根据循环的i值获取对应K3表的KEY主键值
                        var keyid = search.MakeDtidKey(i);
                        //若i值为0，即获取获取的KEY值为fmaterialid
                        if (i == 0) newmaterialid = keyid;
                        
                        #region 根据循环id获取对应表信息
                            //0:T_BD_MATERIAL 1:T_BD_MATERIAL_L 2:t_BD_MaterialBase 3:t_BD_MaterialStock 4:t_BD_MaterialSale
                            //5:t_bd_MaterialPurchase 6:t_BD_MaterialPlan 7:t_BD_MaterialProduce 8:t_BD_MaterialAuxPty 9:t_BD_MaterialInvPty 
                            //10:t_bd_MaterialSubcon 11:T_BD_MATERIALQUALITY
                        switch (i)
                        {
                            case 0:
                                dtname = "T_BD_MATERIAL";
                                break;
                            case 1:
                                dtname = "T_BD_MATERIAL_L";
                                break;
                            case 2:
                                dtname = "t_BD_MaterialBase";
                                break;
                            case 3:
                                dtname = "t_BD_MaterialStock";
                                break;
                            case 4:
                                dtname = "t_BD_MaterialSale";
                                break;
                            case 5:
                                dtname = "t_bd_MaterialPurchase";
                                break;
                            case 6:
                                dtname = "t_BD_MaterialPlan";
                                break;
                            case 7:
                                dtname = "t_BD_MaterialProduce";
                                break;
                            case 8:
                                dtname = "t_BD_MaterialAuxPty";
                                break;
                            case 9:
                                dtname = "t_BD_MaterialInvPty";
                                break;
                            case 10:
                                dtname = "t_bd_MaterialSubcon";
                                break;
                            case 11:
                                dtname = "T_BD_MATERIALQUALITY";
                                break;
                        }
                        #endregion

                        //将materialdt tempdt keyid放至方法内进行选择插入,并返回bool,若为false,即跳转至CreateGenerateRecord()并break,loopmark为false
                        if (!MakeRecordToDb(materialdt, tempdt, keyid, exceltempdt, dtname, binddt))
                        {
                            CreateGenerateRecord(rows,1,$@"物料编码:'{Convert.ToString(rows[1])}'不能成功插入,原因:插入'{dtname}'表时出现异常,请联系管理员");
                            loopmark = false;
                            break;
                        }
                    }

                    //对‘单位换算’表进行记录插入(注:需loopmark不为false时才能继续)
                    if (!loopmark)
                    {
                        //当检测到loopmark为false时,即使用keyid为条件,将已插入的表格进行删除
                        search.DelNewMaterialRecord(newmaterialid);
                        continue;
                    }
                    //todo:执行‘单位换算’表相关内容插入
                    {
                        
                    }
                    //最后若成功生成,并将rows插入至CreateGenerateRecord()内
                    CreateGenerateRecord(rows,0,"");
                }

                //1)将_resultdt插入T_MAT_ImportHistoryRecord表内 2) 最后将结果集添加至GlobalClasscs.RDt.Resultdt内
                ImportDtToDb("T_MAT_ImportHistoryRecord", _resultdt);
                GlobalClasscs.RDt.Resultdt = _resultdt.Copy();
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// 将循环的行记录插入至EXCEL临时表内
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        private DataTable ImportExcelTempdt(DataRow rows)
        {
            //创建EXCEL临时表
            var tempdt = tempDtList.MakeImportTempDt();
            //循环将rows记录插入至tempdt对应的项内
            for (var i = 0; i < tempdt.Columns.Count; i++)
            {
                var newrow = tempdt.NewRow();
                newrow[i] = rows[i];
                tempdt.Rows.Add(newrow);
            }

            return tempdt;
        }

        /// <summary>
        /// 创建收集新记录集及插入至对应数据库(重)
        /// </summary>
        /// <param name="mdt">根据旧materialid获取的数据集</param>
        /// <param name="tempdt">对应的临时表(最后插入使用)</param>
        /// <param name="keyid">对应表新主键ID</param>
        /// <param name="exceltempdt">EXCEL导入的临时表(包含循环行的内容)</param>
        /// <param name="dtname">对应表名称</param>
        /// <param name="binddt">绑定记录表</param>
        /// <returns></returns>
        private bool MakeRecordToDb(DataTable mdt, DataTable tempdt, int keyid, DataTable exceltempdt,string dtname,DataTable binddt)
        {
            var result = true;

            try
            {
                //
                

                //最后将dtname及对应的内容进行插入
                ImportDtToDb(dtname,tempdt);
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }



        /// <summary>
        /// /// 创建并记录生成时的各行记录的情况,并将这些记录插入至_resultdt(生成结果记录集)内
        /// 注:_resultdt此临时表最终会插入至1)T_MAT_ImportHistoryRecord 2)GlobalClasscs.RDt.Resultdt
        /// 注:在中途调用此方法时,表示不能继续,需continue跳出当前循环,到下一条
        /// </summary>
        /// <param name="rows">循环获取的行数</param>
        /// <param name="finishid">是否完成标记(0:是 1:否)</param>
        /// <param name="errorremark">异常信息 注:当成功完成插入时,此变量为空</param>
        private void CreateGenerateRecord(DataRow rows,int finishid,string errorremark)
        {
            //创建历史记录表(结果集)临时表
            var tempdt = tempDtList.CreateHistoryRecordTempdt();

            var newrow = tempdt.NewRow();
            newrow[0] = CreateHistoryKey();        //fid
            newrow[1] = rows[1];                   //物料编码
            newrow[2] = rows[2];                   //物料名称
            newrow[3] = rows[0];                   //品牌
            newrow[4] = rows[4];                   //规格型号
            newrow[5] = finishid;                  //是否成功标记
            newrow[6] = errorremark;               //异常原因
            newrow[7] = DateTime.Now.Date;         //导入时间
            tempdt.Rows.Add(newrow);

            //最后将dt临时表与_resultdt进行合并
            _resultdt.Merge(tempdt);
        }

        /// <summary>
        /// 创建并获取T_MAT_ImportHistoryRecord表的主键值
        /// </summary>
        /// <returns></returns>
        private int CreateHistoryKey()
        {
            return search.MakeDtidKey(13);
        }

        #endregion

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
