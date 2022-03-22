using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using K3MaterialGeneralTool.DB;

namespace K3MaterialGeneralTool.Task
{
    //生成新物料 插入记录
    public class InsertGenerate
    {
        #region  参数定义
        private int _newmaterialid = 0;                //记录新fmaterialid
        private int _salesunit = 0;                    //记录旧记录-销售.销售单位(FSALEUNITID);生成‘单位换算’表时使用
        private DataTable _guandt;                     //记录初始化的罐箱记录;新建时需要使用
        private string _errmessage = string.Empty;     //记录异常信息
        #endregion

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
                ImportDtToDb(1,"T_MAT_BindRecord", bintempdt);
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
                ImportDtToDb(1,"T_MAT_BindExcelCol", createtempdt);
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
                ImportDtToDb(1,"T_MAT_Source", createk3Tempdt);
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

            try
            {
                //初始化获取罐箱相关内容
                _guandt = search.GetGuanXuanDtl();

                //定义历史记录表(结果集)临时表
                _resultdt = tempDtList.CreateHistoryRecordTempdt();
                //获取绑定记录表
                var binddt = search.SearchBind();
                //若根据'物料编码'在DB内查找到有相关记录,就先记录,在最后完成生成后对此samematerialidlist相关的记录进行删除(已取消 change date:20211122)
                //现已修改为当出现这种情况时,就跳过不进行插入,并在结果集内提示原因(change date:20211122)
                var samematerialiddt = search.SearchImportIdAndDel(importdt);

                //循环读取importdt内的记录
                foreach (DataRow rows in importdt.Rows)
                {
                    //根据‘品牌’及‘规格型号’获取K3物料中TOP 1的Fmaterialid(注:若此没有记录,即马上跳出当前循环)
                    var searchdt = search.SearchTop1MaterialRecord(Convert.ToString(rows[0]),Convert.ToString(rows[4]));

                    if (searchdt.Rows.Count == 0)
                    {
                        CreateGenerateRecord(rows, 1, $@"没有在K3中找到品牌:'{Convert.ToString(rows[0])}'及规格型号:'{Convert.ToString(rows[4])}'的相关记录,故不能自动生成新物料,请在K3进行手动创建");
                        continue;
                    }
                    //检测若循环的物料在samematerialiddt临时表内获在,即表示已在K3里内存在,不能进行插入,并需在结果集内体现
                    var dtlrow = samematerialiddt.Select("FNUMBER='"+Convert.ToString(rows[1])+"'");
                    if (dtlrow.Length>0)
                    {
                        CreateGenerateRecord(rows,1,$"检测到要导入的物料编码:'{Convert.ToString(rows[1])}'已在K3内存在,故不能自动生成新物料,请检查后再执行");
                        continue;
                    }


                    //若searchdt有值,即将fmaterial赋值给oldmaterialid
                    var oldmaterialid = Convert.ToInt32(searchdt.Rows[0][0]);

                    //创建EXCEL临时表并将rows记录插入其中
                    var exceltempdt = ImportExcelTempdt(rows);

                    //若找到oldmaterialid相关值,即获取关于此oldmaterialid的相关表格信息
                    //循环从0~11;分别针对不同表进行生成KEY 插入内容等相关操作
                    for (var i = 0; i < 12; i++)
                    {
                        //根据循环的i值及oldmaterialid获取数据源
                        var materialdt = search.Get_SearchMaterialSourceAndCreateTemp(i, oldmaterialid);

                        //判断若materialdt返回的行数为0,即跳出当前循环至下一个循环
                        if(materialdt.Rows.Count==0) continue;

                        //根据materialdt动态生成对应表格的临时表
                        var tempdt = materialdt.Clone();
   
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

                        //当dtname="T_BD_MATERIALSALE"时,获取materialdt.rows[0][2]的值并赋给salesunit变量;‘单位换算’表使用
                        if (dtname == "t_BD_MaterialSale")
                        {
                            _salesunit = Convert.ToInt32(materialdt.Rows[0][2]);
                        }

                        //将materialdt tempdt keyid放至方法内进行选择插入,并返回bool,若为false,即跳转至CreateGenerateRecord()并break(跳出所有循环),loopmark为false
                        if (!MakeRecordToDb(i,materialdt, tempdt, exceltempdt, dtname, binddt))
                        {
                            CreateGenerateRecord(rows,1,$@"物料编码:'{Convert.ToString(rows[1])}'不能成功插入,原因:插入'{dtname}'表时出现'{_errmessage}'异常,请联系管理员");
                            loopmark = false;
                            break;
                        }
                    }

                    //对‘单位换算’表进行记录插入(注:需loopmark不为false时才能继续)
                    if (!loopmark)
                    {
                        //当检测到loopmark为false时,即使用keyid为条件,将已插入的表格进行删除
                        search.DelNewMaterialRecord(_newmaterialid);
                        //change date:20220322 修正当执行完删除操作后,就会loopmark初始化,将false更新为True值
                        loopmark = true;
                        continue;
                    }
                    //执行‘单位换算’表相关内容插入
                    //获取excel来源的'物料编码' '净重'及'规格型号'
                    var excelmatcode = Convert.ToString(exceltempdt.Rows[0]["物料编码"]);
                    var nkg = Convert.ToDecimal(exceltempdt.Rows[0]["净重"]);
                    var kui = Convert.ToString(exceltempdt.Rows[0]["规格型号"]);
                    MakeUnitRecordToDb(_salesunit, excelmatcode, nkg,kui);
                    //最后若成功生成,并将rows插入至CreateGenerateRecord()内
                    CreateGenerateRecord(rows,0,"");
                }

                //1)将_resultdt插入T_MAT_ImportHistoryRecord表内 2) 最后将结果集添加至GlobalClasscs.RDt.Resultdt内
                ImportDtToDb(1,"T_MAT_ImportHistoryRecord", _resultdt);
                GlobalClasscs.RDt.Resultdt = _resultdt.Copy();
                //若samematerialidlist有值,即执行删除操作(已取消;change date:20211122)
                //if (samematerialiddt.Rows.Count > 0) DelSameRecord(samematerialiddt);
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        /// <summary>
        /// 将包含相同‘物料编码’的旧fmaterialid相关记录删除(已取消 change date:20211122)
        /// </summary>
        /// <param name="deldt"></param>
        private void DelSameRecord(DataTable deldt)
        {
            //将已存在的fmaterialid删除
            foreach (DataRow rows in deldt.Rows)
            {
                search.DelNewMaterialRecord(Convert.ToInt32(rows[0]));
            }
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
            var newrow = tempdt.NewRow();
            for (var i = 0; i < tempdt.Columns.Count; i++)
            {
                newrow[i] = rows[i];
            }
            tempdt.Rows.Add(newrow);
            return tempdt;
        }

        /// <summary>
        /// 执行‘单位换算’表相关内容插入
        /// </summary>
        /// <param name="salesunit">旧记录销售单位ID</param>
        /// <param name="excelmatcode">excel物料编码</param>
        /// <param name="nkg">净重-来源excel内容</param>
        /// <param name="kui">规格型号-来源excel内容</param>
        private void MakeUnitRecordToDb(int salesunit,string excelmatcode,decimal nkg,string kui)
        {
            var fenzi = new decimal();
            //获取‘单位换算’临时表
            var tempdt = tempDtList.CreateK3ImportTempDt(12);

            //循环将相关值插入至临时表(固定插入两行)
            for (var i = 0; i < 2; i++)
            {
                //获取‘单位换算’新主键值
                var unitid = search.MakeDtidKey(12);

                //将‘物料编码’截图前8位+{{{{{0}}}结合
                var materialcode = excelmatcode.Substring(0, 7);
                var checkvalue = materialcode + "{{{{{0}}}";

                //创建(更新)T_BAS_BILLCODES(编码规则最大编码表)中的相关记录-FBILLNO字段使用
                search.Get_MakeUnitKey(checkvalue);

                //根据物料编码在‘编码索引表’内查询出FNUMMAX值,以此构建‘物料单位换算’中FBILLNO的组成部份
                var nummax = search.SearchUnitMaxKey(checkvalue);

                //将‘物料编码’截图前8位+‘编码索引表’内查询出FNUMMAX值,整合成新的FBILLNO值
                var fbillno = materialcode + nummax;

                //根据salesunit判断并获取FCURRENTUNITID最终值
                //注:1)i=0时,再进行判断,当salesunit=10099(升)或100157(罐)就统称为罐,其余的情况直接赋值
                var currentunitid = salesunit == 10099 || salesunit == 100157 ? 100157 : salesunit;

                //获取‘规格型号’内的数值,作为‘分子’的值
                var str = Regex.Replace(kui, @"[^\d.\d]", "");
                //如果是数字，则转换为decimal类型
                fenzi = Regex.IsMatch(str, @"^[+-]?\d*[.]?\d*$") ? decimal.Parse(str) : 0;

                //插入相关值至对应项内(共23项)
                var newrow = tempdt.NewRow();
                newrow[0] = unitid;                                              //FUNITCONVERTRATEID
                newrow[1] = _newmaterialid;                                      //FMASTERID
                newrow[2] = fbillno;                                             //FBILLNO
                newrow[3] = "BD_MATERIALUNITCONVERT";                            //FFORMID
                newrow[4] = _newmaterialid;                                      //FMATERIALID
                newrow[5] = i == 0 ? currentunitid : 10095;                      //FCURRENTUNITID(单位)
                newrow[6] = 10099;                                               //FDESTUNITID(基本单位;固定:升10099)
                newrow[7] = 0;                                                   //FCONVERTTYPE
                newrow[8] = fenzi;                                               //FCONVERTNUMERATOR(分子)
                newrow[9] = i == 0 ? 1 : nkg;                                    //FCONVERTDENOMINATOR(换算关系)
                newrow[10] = 1;                                                  //FCREATEORGID
                newrow[11] = 1;                                                  //FUSEORGID
                newrow[12] = 380167;                                             //FCREATORID(创建者)  380167(信息管理科) 100005(阮明达)
                newrow[13] = DateTime.Now.Date;                                  //FCREATEDATE
                newrow[14] = 380167;                                             //FMODIFIERID(修改者) 380167(信息管理科)
                newrow[15] = DateTime.Now.Date;                                  //FMODIFYDATE
                newrow[16] = 380167;                                             //FAPPROVERID(审核者) 380167(信息管理科)
                newrow[17] = DateTime.Now.Date;                                  //FAPPROVEDATE
                newrow[18] = 0;                                                  //FFORBIDDERID
                newrow[19] = DateTime.Now.Date;                                  //FFORBIDDATE
                newrow[20] = "A";                                                //FDOCUMENTSTATUS
                newrow[21] = "";                                                 //FFORBIDSTATUS
                newrow[22] = -1;                                                 //FUNITID

                tempdt.Rows.Add(newrow);
            }
            //最后将记录插入至T_BD_UNITCONVERTRATE数据表内
            ImportDtToDb(0,"T_BD_UNITCONVERTRATE", tempdt);
        }

        /// <summary>
        /// 创建收集新记录集及插入至对应数据库(重)
        /// </summary>
        /// <param name="tabid">表格循环ID</param>
        /// <param name="mdt">根据旧materialid获取的数据集</param>
        /// <param name="tempdt">对应的临时表(最后插入使用)</param>
        /// <param name="exceltempdt">EXCEL导入的临时表(包含循环行的内容)</param>
        /// <param name="dtname">对应表名称</param>
        /// <param name="binddt">绑定记录表</param>
        /// <returns></returns>
        private bool MakeRecordToDb(int tabid,DataTable mdt, DataTable tempdt,DataTable exceltempdt,string dtname,DataTable binddt)
        {
            var result = true;
            //定义K3列名
            var k3Colname = string.Empty;

            try
            {
                //根据EXCEL-‘包装罐’查询相关记录
                var guanrows = _guandt.Select("名称='" + Convert.ToString(exceltempdt.Rows[0][5]) + "'");
                //根据EXCEL-‘包装箱’查询相关记录
                var xuanrows = _guandt.Select("名称='" + Convert.ToString(exceltempdt.Rows[0][6]) + "'");

                //循环mdt数据源-行
                for (var i = 0; i < mdt.Rows.Count; i++)
                {
                    //根据循环的tabid值获取对应K3表的KEY主键值
                    var keyid = search.MakeDtidKey(tabid);
                    //若tabid值为0，即获取的KEY值为newfmaterialid
                    if (tabid == 0) _newmaterialid = keyid;

                    //对tempdt创建行,注:因为mdt与tempdt数据结构一致,故列也是一致
                    var newrow = tempdt.NewRow();
                    //循环mdt数据源-列
                    for (var j = 0; j < mdt.Columns.Count; j++)
                    {
                        //获取K3列名
                        k3Colname = mdt.Columns[j].ColumnName;
                        //若dtname为'T_BD_MATERIAL'时,J=0即将keyid赋给newrow[j];若为其余11个表,即需要当j=0 1时,
                        //分别将keyid(本表新主键) newmaterialid(对应T_BD_MATERIAL新主键)进行赋值
                        if (dtname == "T_BD_MATERIAL")
                        {
                            switch (j)
                            {
                                case 0:
                                    newrow[j] = keyid;
                                    continue;
                                case 4:
                                    newrow[j] = keyid;
                                    continue;
                            }
                        }
                        else if(dtname != "T_BD_MATERIAL")
                        {
                            switch (j)
                            {
                                case 0:
                                    newrow[j] = keyid;
                                    continue;
                                case 1:
                                    newrow[j] = _newmaterialid;
                                    continue;
                            }
                        }

                        //使用k3Colname及k3Tablename查询是否有绑定关系;若有就获取对应的值;若没有将使用旧记录赋值给对应的项内
                        //根据k3Colname及k3Tablename查在bindt内找出对应的‘Excel字段名称’
                        var dtlrows = binddt.Select("K3列名='" + k3Colname + "' and K3表名='" + dtname + "'");

                        if (dtlrows.Length > 0)
                        {
                            //若dtlrows有值,即将其赋给excelcolname变量内
                            var excelcolname = Convert.ToString(dtlrows[0][3]);
                            //根据excelcolname找到excedt对应列的内容
                            var excelcolvalue = Convert.ToString(exceltempdt.Rows[0][$"{excelcolname}"]);
                            //根据excelcolname 及 excelcolvalue进行逻辑判断,并将结果返回至result变量内(重)
                            var colvalue = CheckAndGetColValue(excelcolname, excelcolvalue);
                            
                            //按不同绑定字段情况进行数据类型转换(注:主要将不为string的数据类型进行转换)
                            if (k3Colname == "FMATERIALGROUP" || k3Colname == "FCATEGORYID" || k3Colname == "FTAXRATEID" ||
                                k3Colname == "FBASEUNITID" || k3Colname == "FSTOREUNITID" || k3Colname == "FSALEUNITID" || k3Colname == "FSALEPRICEUNITID")
                            {
                                newrow[j] = colvalue == "" ? (object)DBNull.Value : Convert.ToInt32(colvalue);
                            }
                            else if (k3Colname == "F_YTC_BASE" || k3Colname == "F_YTC_BASE2" || k3Colname == "F_YTC_BASE3")
                            {
                                newrow[j] = colvalue == "" ? 0 : Convert.ToInt32(colvalue);
                            }
                            else if (k3Colname=="F_YTC_DECIMAL" || k3Colname=="FNETWEIGHT")
                            {
                                newrow[j] = colvalue == "" ? 0 : Convert.ToDecimal(colvalue);
                            }
                            else
                            {
                                newrow[j] = colvalue;
                            }
                        }
                        //执行没有绑定的字段数据对接
                        else
                        {
                            //若K3字段不包含 FCREATORID FCREATEDATE FMODIFYDATE FDOCUMENTSTATUS,就将旧记录赋给临时表对应的项内
                            switch (k3Colname)
                            {
                                case "FCREATORID":      //创建者ID 380167(信息管理科) 100005(阮明达)
                                case "FMODIFIERID":     //修改者ID 380167(信息管理科)
                                    newrow[j] = 380167;  
                                    break;
                                case "FCREATEDATE":     //创建日期
                                case "FMODIFYDATE":     //修改日期
                                    newrow[j] = DateTime.Now.Date;
                                    break;
                                case "FAPPROVERID":     //审核人
                                    newrow[j] = 0;
                                    break;
                                case "FAPPROVEDATE":    //审核日期
                                    newrow[j] = DBNull.Value;
                                    break;
                                case "FDOCUMENTSTATUS":
                                case "FFORBIDSTATUS":
                                    newrow[j] = "A";     //单据状态:创建
                                    break;
                                case "F_YTC_REMARK":     //基本.包装规格(组成规则:名称-规格型号-包装系数值+罐/箱)
                                    newrow[j] = Convert.ToString(exceltempdt.Rows[0][2]) + "-" + Convert.ToString(exceltempdt.Rows[0][4]) + "-" + Convert.ToString(exceltempdt.Rows[0][3]) + "罐/箱";
                                    break;
                                case "F_YTC_REMARK1":    //基本.排序(方法:若mdt.rows[i][j]有值,就将原来的信息进行截取并与‘物料编码’进行合拼)
                                    if (mdt.Rows[i][j].ToString() == "")
                                    {
                                        newrow[j] = "";
                                    }
                                    else
                                    {
                                        //获取原来记录最后一个,的下标值(注:+1是为了包含,)
                                        var endid = Convert.ToString(mdt.Rows[i][j]).LastIndexOf(",", StringComparison.Ordinal) + 1;
                                        newrow[j] = Convert.ToString(mdt.Rows[i][j]).Substring(0,endid) + Convert.ToString(exceltempdt.Rows[0][1]);
                                    }
                                    break;
                                case "F_YTC_TEXT1":      //标签名称
                                    newrow[j] = GetMaterialNameAndCode(0,Convert.ToString(exceltempdt.Rows[0][2]));
                                    break;
                                case "F_YTC_TEXT2":      //标签代码
                                    newrow[j] = GetMaterialNameAndCode(1, Convert.ToString(exceltempdt.Rows[0][2]));
                                    break;
                                //将系数 干燥性 颜色 U订货商品编码清空
                                case "F_YTC_ASSISTANT2":      //系数
                                case "F_YTC_ASSISTANT111":    //干燥性
                                case "F_YTC_ASSISTANT111111": //颜色
                                case "F_YTC_TEXT8":           //U订货商品编码
                                    newrow[j] = "";
                                    break;
                                case "F_YTC_DECIMAL7":        //U订货计价规格
                                    //根据EXCEL内获取的‘规格型号’信息;若包含套的,此项为1;反之,取‘规格型号’内的数值
                                    var kui = Convert.ToString(exceltempdt.Rows[0][4]);

                                    if (kui.Contains("套"))
                                    {
                                        newrow[j] = 1;
                                    }
                                    else
                                    {
                                        //获取‘规格型号’内的数值
                                        var str = Regex.Replace(kui, @"[^\d.\d]", "");
                                        //如果是数字，则转换为decimal类型,否则为0
                                        newrow[j] = Regex.IsMatch(str, @"^[+-]?\d*[.]?\d*$") ? decimal.Parse(str) : 0;
                                    }
                                    break;
                                //add date:20210301
                                case "FGROSSWEIGHT":       //罐重
                                    newrow[j] = guanrows.Length == 0 ? 0 : Convert.ToDecimal(guanrows[0][2]);
                                    break;
                                case "FLENGTH":            //长
                                    newrow[j] = xuanrows.Length == 0 ? 0 : Convert.ToDecimal(xuanrows[0][3]);
                                    break;
                                case "FWIDTH":             //宽
                                    newrow[j] = xuanrows.Length == 0 ? 0 : Convert.ToDecimal(xuanrows[0][4]);
                                    break;
                                case "FHEIGHT":            //高
                                    newrow[j] = xuanrows.Length == 0 ? 0 : Convert.ToDecimal(xuanrows[0][5]);
                                    break;
                                case "FVOLUME":            //体积
                                    newrow[j] = xuanrows.Length == 0 ? 0 : Convert.ToDecimal(xuanrows[0][6]);
                                    break;
                                case "F_YTC_DECIMAL2":     //箱重
                                    newrow[j] = xuanrows.Length == 0 ? 0 : Convert.ToDecimal(xuanrows[0][7]);
                                    break;

                                //检测若mdt.rows[i][j]为空值,若对应的列名包含F_YTC_TEXT F_YTC_REMARK F_YTC_ASSISTANT的就返回"",若包含F_YTC_DECIMAL F_YTC_BASE的就返回0;若正常即直接赋值
                                default:
                                //change date:20220318 在T_BD_MATERIAL表,若循环到  F_YTC_DECIMAL3(包装系数) [48] 即将F_YTC_DECIMAL(罐/箱) [34]记录进行赋值
                                    if (dtname == "T_BD_MATERIAL" && j == 48)
                                    {
                                        newrow[j] = Convert.ToDecimal(newrow[34]);
                                    }
                                //若为空的值就根据指定字段设置为 "" 或 0
                                    else
                                    {
                                        if (mdt.Rows[i][j].ToString() == "")
                                        {
                                            if (k3Colname.Contains("F_YTC_TEXT") || k3Colname.Contains("F_YTC_REMARK") || k3Colname.Contains("F_YTC_ASSISTANT"))
                                            {
                                                newrow[j] = "";
                                            }
                                            else if (k3Colname.Contains("F_YTC_DECIMAL") || k3Colname.Contains("F_YTC_BASE"))
                                            {
                                                newrow[j] = 0;
                                            }
                                        }
                                        else
                                        {
                                            newrow[j] = mdt.Rows[i][j];
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                    tempdt.Rows.Add(newrow);
                }

                //最后将dtname及对应的内容进行插入
                ImportDtToDb(0,dtname,tempdt);
            }
            catch (Exception ex)
            {
                _errmessage = "字段:"+k3Colname+ex.Message;
                result = false;
            }

            return result;
        }

        /// <summary>
        /// 根据excelcolname 及 excelcolvalue进行逻辑判断,并将结果返回(重)
        /// </summary>
        /// <param name="excecolname">Excel绑定列名</param>
        /// <param name="excecolvalue">Excel绑定列名对应内容</param>
        /// <returns></returns>
        private string CheckAndGetColValue(string excecolname,string excecolvalue)
        {
            string result;

            //根据excecolname跳转至不同的方法进行获取相关ID值,若不用查找对应ID值的列,即将excecolvalue赋给result变量
            //若excecolname包含'原漆',就跳转至SearchK3BinRecord()方法
            if (excecolname == "原漆物料名称")
            {
                var dt = search.SearchK3BinRecord(excecolvalue);
                result = dt.Rows.Count == 1 ? Convert.ToString(dt.Rows[0][0]) : "";
            }
            //若excecolname包含0:品牌 1:分类 2:品类 3:组份 4:干燥性 5:常规/订制 6:阻击产品 7:颜色 8:系数 9:水性油性 10:原漆半成品属性 11:开票信息 12:研发类别
            //13:包装罐(包装箱) 14:物料分组(辅助) 15:物料分组 16:存货类别 17:默认税率 18:基本单位 19:化学品分类,就跳转至SearchSourceRecord()方法
            else if (excecolname == "品牌" || excecolname =="分类" || excecolname =="品类" ||excecolname =="组份" || 
                    excecolname == "干燥性" || excecolname =="工业项目名称(常规/定制)" || excecolname =="是否为阻击产品" || excecolname == "颜色" || 
                    excecolname =="系数" || excecolname =="水性油性" || excecolname =="原漆半成品属性" || excecolname == "开票信息" || 
                    excecolname =="研发类别" || excecolname =="包装罐" || excecolname =="包装箱" || excecolname == "物料分组(辅助)" || 
                    excecolname =="物料分组" || excecolname=="存货类别" || excecolname=="默认税率" || excecolname== "基本单位" || excecolname== "化学品分类")
            {
                var typeid = 0;

                #region 根据不同的列名获取不同的typeid值
                if (excecolname =="品牌")
                {
                    typeid = 0;
                }
                else if (excecolname =="分类")
                {
                    typeid = 1;
                }
                else if (excecolname =="品类")
                {
                    typeid = 2;
                }
                else if (excecolname =="组份")
                {
                    typeid = 3;
                }
                else if (excecolname =="干燥性")
                {
                    typeid = 4;
                }
                else if (excecolname =="工业项目名称(常规/定制)")
                {
                    typeid = 5;
                }
                else if (excecolname =="是否为阻击产品")
                {
                    typeid = 6;
                }
                else if (excecolname =="颜色")
                {
                    typeid = 7;
                }
                else if (excecolname =="系数")
                {
                    typeid = 8;
                }
                else if (excecolname =="水性油性")
                {
                    typeid = 9;
                }
                else if (excecolname =="原漆半成品属性")
                {
                    typeid = 10;
                }
                else if (excecolname =="开票信息")
                {
                    typeid = 11;
                }
                else if (excecolname =="研发类别")
                {
                    typeid = 12;
                }
                else if (excecolname =="包装罐")
                {
                    typeid = 13;
                }
                else if (excecolname =="包装箱")
                {
                    typeid = 13;
                }
                else if (excecolname =="物料分组(辅助)")
                {
                    typeid = 14;
                }
                else if (excecolname =="物料分组")
                {
                    typeid = 15;
                }
                else if (excecolname =="存货类别")
                {
                    typeid = 16;
                }
                else if (excecolname =="默认税率")
                {
                    typeid = 17;
                }
                else if (excecolname =="基本单位")
                {
                    typeid = 18;
                }
                else if (excecolname == "化学品分类")
                {
                    typeid = 19;
                }
                #endregion

                var dt = search.SearchSourceRecord(typeid, excecolvalue);
                result = dt.Rows.Count == 1 ? Convert.ToString(dt.Rows[0][0]) : "";
            }
            //若excecolname包含'U订货商品分类',即调用XXX方法;注:若返回结果多于1行,即返回null值至result变量
            else if (excecolname =="U订货商品分类")
            {
                //定义各商品分类变量
                var oneLeverName = string.Empty;   //一级商品分类
                var twoLeverName = string.Empty;   //二级商品分类
                var dtlname = string.Empty;        //三级商品分类

                //将excecolvalue进行以‘_’分拆
                var typenamelist = excecolvalue.Split('_');
                //判断若typenamelist<2,即返回null
                if (typenamelist.Length < 2)
                {
                    result = "";
                }
                else
                {
                    //根据typenamelist.Length,获取不同的值至不同的变量内
                    if (typenamelist.Length == 2)
                    {
                        oneLeverName = Convert.ToString(typenamelist[0]);
                        twoLeverName = Convert.ToString(typenamelist[1]);
                    }
                    else if (typenamelist.Length > 2)
                    {
                        oneLeverName = Convert.ToString(typenamelist[0]);
                        twoLeverName = Convert.ToString(typenamelist[1]);
                        dtlname = Convert.ToString(typenamelist[2]);
                    }
                    

                    //将值赋给SearchUProdceType(),并返回相关值;注:若返回的行数超过1行,即返回null值
                    var dt = search.SearchUProdceType(oneLeverName, twoLeverName, dtlname);
                    result = dt.Rows.Count > 1 ? "" : Convert.ToString(dt.Rows[0][0]);
                }
            }
            //若excecolname不包含上面所指三种情况,即将excecolvalue值赋给result变量
            else
            {
                result = excecolvalue;
            }

            return result;
        }

        /// <summary>
        /// 创建并记录生成时的各行记录的情况,并将这些记录插入至_resultdt(生成结果记录集)内
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

        /// <summary>
        /// 根据物料名称获取‘标签名称(代码)’
        /// </summary>
        /// <param name="typeid">0:获取标签名称 1:获取标签代码</param>
        /// <param name="materialname"></param>
        /// <returns></returns>
        private string GetMaterialNameAndCode(int typeid,string materialname)
        {
            string result;
            var startid = 0;  //截取字符串起始位置
            var endid = 0;   //截取字符串结束位置

            //检测materialname是否包含中文
            var check = Regex.IsMatch(materialname, @"[\u4e00-\u9fa5]");

            //获取标签名称
            if (typeid == 0)
            {
                //若不包含中文即返回空值
                if (!check)
                {
                    result = "";
                }
                else
                {
                    //检测到字符串出现首个中文,并以此对应下标为最终截取大小,从而获取不包含中文的值
                    //用的是ASCII码判断,在ASCII码表中,英文的范围是0-127,而汉字则是大于127,根据这个范围可以判断;然后获取之前的字符
                    for (var i = 0; i < materialname.Length; i++)
                    {
                        if ((int) materialname[i] <= 127) continue;
                        startid = i;
                        break;
                    }
                    //从有中文的位置开始至结束
                    result = materialname.Substring(startid);
                }
            }
            //获取标签代码
            else
            {
                if (!check)
                {
                    result = materialname;
                }
                else
                {
                    //检测到字符串出现首个中文,并以此对应下标为最终截取大小,从而获取不包含中文的值
                    //用的是ASCII码判断,在ASCII码表中,英文的范围是0-127,而汉字则是大于127,根据这个范围可以判断;然后获取之前的字符
                    for (var i = 0; i < materialname.Length; i++)
                    {
                        if ((int)materialname[i] <= 127) continue;
                        endid = i;
                        break;
                    }
                    //从0开始至有首个中文位置的前一位
                    result = materialname.Substring(0,endid);
                }
            }
            return result;
        }

        #endregion

        /// <summary>
        /// 针对指定表进行数据插入
        /// </summary>
        /// <param name="connid">连接类型ID; 0:K3 1:Material数据库</param>
        /// <param name="tableName">表名</param>
        /// <param name="dt">包含数据的临时表</param>
        private void ImportDtToDb(int connid,string tableName, DataTable dt)
        {
            try
            {
                var conn = new Conn();
                var sqlcon = conn.GetConnectionString(connid == 0 ? 0 : 1);

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
