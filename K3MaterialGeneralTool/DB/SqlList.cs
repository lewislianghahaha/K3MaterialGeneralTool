using System;
using NPOI.SS.Formula.Functions;

namespace K3MaterialGeneralTool.DB
{
    public class SqlList
    {
        //根据SQLID返回对应的SQL语句  
        private string _result;

        #region 绑定相关

        /// <summary>
        /// Excel字段绑定记录(在‘绑定’功能显示使用)
        /// </summary>
        /// <returns></returns>
        public string Get_SearchBindExcelCol()
        {
            _result = @"
                          SELECT A.ExcelColId,A.ExcelCol 名称,A.ExcelColDataType 数据类型
                          FROM dbo.T_MAT_BindExcelCol A
                          WHERE A.Bindid=1
                       ";
            return _result;
        }

        /// <summary>
        /// K3字段绑定记录(在'绑定'功能显示及更新时使用)
        /// </summary>
        /// <returns></returns>
        public string Get_SearchBindK3Col(int typeid)
        {
            _result = $@"
                           SELECT A.K3ColId,A.K3Col,A.K3ColTableName,A.K3ColName 名称,A.K3ColDataType 数据类型
                           FROM dbo.T_MAT_BindK3Col A
                           WHERE A.Bindid=1
                           AND A.Typeid='{typeid}'
                           ORDER BY A.Typeid
                       ";
            return _result;
        }

        /// <summary>
        /// 绑定关系
        /// </summary>
        /// <returns></returns>
        public string Get_SearchBind()
        {
            _result = @"
                           SELECT A.Fid,A.ExcelColId,A.K3ColId,B.ExcelCol Excel字段名称,C.K3ColName K3字段名称,C.K3Col K3列名,C.K3ColTableName K3表名,
                                  A.BindDt 绑定日期
                           FROM dbo.T_MAT_BindRecord A
                           INNER JOIN dbo.T_MAT_BindExcelCol B ON A.ExcelColId=B.ExcelColId
                           INNER JOIN dbo.T_MAT_BindK3Col C ON A.K3ColId=C.K3ColId
                       ";
            return _result;
        }

        /// <summary>
        /// 更新绑定字段绑定(构建绑定时使用)
        /// </summary>
        /// <returns></returns>
        public string Update_Bind(int exceid, int k3Id)
        {
            _result = $@"
                            UPDATE dbo.T_MAT_BindExcelCol SET Bindid=0 WHERE ExcelColId='{exceid}'

                            UPDATE dbo.T_MAT_BindK3Col SET Bindid=0 WHERE K3ColId='{k3Id}'
                       ";
            return _result;
        }

        /// <summary>
        /// 更新绑定字段绑定(清除绑定时使用)
        /// </summary>
        /// <param name="exceid"></param>
        /// <param name="k3Id"></param>
        /// <param name="fid"></param>
        /// <returns></returns>
        public string Update_RemoveBind(int exceid,int k3Id,int fid)
        {
            _result = $@"
                            UPDATE dbo.T_MAT_BindExcelCol SET Bindid=1 WHERE ExcelColId='{exceid}'

                            UPDATE dbo.T_MAT_BindK3Col SET Bindid=1 WHERE K3ColId='{k3Id}'

                            DELETE dbo.T_MAT_BindRecord WHERE Fid='{fid}'
                       ";
            return _result;
        }

        #endregion

        #region 基础数据相关

        /// <summary>
        /// 获取K3-‘物料’各下拉列表记录(同步基础数据源时使用)
        /// </summary>
        /// <returns></returns>
        public string Get_SearchK3SourceRecord()
        {
            _result = $@"
                            TRUNCATE TABLE MaterialGeneral.dbo.T_MAT_Source

                            SELECT X.TYPEID,X.ID,X.NAME,X.CreateDt
                            FROM(
                            --品牌
                            SELECT /*A.FID,A.FNUMBER,B.FNUMBER 编号,*/0 TYPEID,B.FENTRYID id,C.FDATAVALUE NAME,GETDATE() CreateDt
                            FROM dbo.T_BAS_ASSISTANTDATA a
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY B ON A.FID=B.FID
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L C ON B.FENTRYID=C.FENTRYID AND C.FLOCALEID=2052
                            WHERE A.FID='57623fb7940f6a'--C.FDATAVALUE LIKE '%威施乐%'

                            UNION ALL

                            --分类
                            SELECT /*A.FID,A.FNUMBER,B.FNUMBER 编号,*/1 TYPEID,B.FENTRYID ID,C.FDATAVALUE NAME,GETDATE() CreateDt
                            FROM dbo.T_BAS_ASSISTANTDATA a
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY B ON A.FID=B.FID
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L C ON B.FENTRYID=C.FENTRYID AND C.FLOCALEID=2052
                            WHERE A.FID='56d5329cd0f4a7'
                            AND B.FDOCUMENTSTATUS='C' --C.FDATAVALUE LIKE '%色母%'

                            UNION ALL

                            --品类
                            SELECT /*A.FID,A.FNUMBER,B.FNUMBER 编号,*/2 TYPEID,B.FENTRYID ID,C.FDATAVALUE NAME,GETDATE() CreateDt
                            FROM dbo.T_BAS_ASSISTANTDATA a
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY B ON A.FID=B.FID
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L C ON B.FENTRYID=C.FENTRYID AND C.FLOCALEID=2052
                            WHERE A.FID='56d5332dd0f4a9'
                            AND B.FDOCUMENTSTATUS='C'--C.FDATAVALUE LIKE '%塑料底漆%'
                            --ORDER BY B.FNUMBER

                            UNION ALL

                            --组份
                            SELECT /*A.FID,A.FNUMBER,B.FNUMBER 编号,*/3 TYPEID,B.FENTRYID ID,C.FDATAVALUE NAME,GETDATE() CreateDt
                            FROM dbo.T_BAS_ASSISTANTDATA a
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY B ON A.FID=B.FID
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L C ON B.FENTRYID=C.FENTRYID AND C.FLOCALEID=2052
                            WHERE A.FID='56d5333bd0f4ab'
                            AND B.FDOCUMENTSTATUS='C' --C.FDATAVALUE LIKE '%1K%'
                            --ORDER BY B.FNUMBER

                            UNION ALL

                            --干燥性
                            SELECT /*A.FID,A.FNUMBER,B.FNUMBER 编号,*/4 TYPEID,B.FENTRYID ID,C.FDATAVALUE NAME,GETDATE() CreateDt
                            FROM dbo.T_BAS_ASSISTANTDATA a
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY B ON A.FID=B.FID
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L C ON B.FENTRYID=C.FENTRYID AND C.FLOCALEID=2052
                            WHERE A.FID='56d53348d0f4ad'
                            AND B.FDOCUMENTSTATUS='C' --C.FDATAVALUE LIKE '%标准%'
                            --ORDER BY B.FNUMBER

                            UNION ALL

                            --常规/订制
                            SELECT /*A.FID,A.FNUMBER,B.FNUMBER 编号,*/5 TYPEID,B.FENTRYID ID,C.FDATAVALUE NAME,GETDATE() CreateDt
                            FROM dbo.T_BAS_ASSISTANTDATA a
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY B ON A.FID=B.FID
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L C ON B.FENTRYID=C.FENTRYID AND C.FLOCALEID=2052
                            WHERE A.FID='56d53359d0f4af'
                            AND B.FDOCUMENTSTATUS='C'-- C.FDATAVALUE LIKE '%订制%'
                            --ORDER BY B.FNUMBER

                            UNION ALL

                            --阻击产品
                            SELECT /*A.FID,A.FNUMBER,B.FNUMBER 编号,*/6 TYPEID,B.FENTRYID ID,C.FDATAVALUE NAME,GETDATE() CreateDt
                            FROM dbo.T_BAS_ASSISTANTDATA a
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY B ON A.FID=B.FID
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L C ON B.FENTRYID=C.FENTRYID AND C.FLOCALEID=2052
                            WHERE A.FID='56d5362fd0f4b3'  --57623fb7940f6a
                            AND B.FDOCUMENTSTATUS='C'-- C.FDATAVALUE LIKE '%阻击产品%'
                            --ORDER BY B.FNUMBER

                            UNION ALL

                            --颜色
                            SELECT /*A.FID,A.FNUMBER,B.FNUMBER 编号,*/ 7 TYPEID,B.FENTRYID ID,C.FDATAVALUE NAME,GETDATE() CreatDt
                            FROM dbo.T_BAS_ASSISTANTDATA a
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY B ON A.FID=B.FID
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L C ON B.FENTRYID=C.FENTRYID AND C.FLOCALEID=2052
                            WHERE A.FID='56d5337fd0f4b1'
                            AND B.FDOCUMENTSTATUS='C' --C.FDATAVALUE LIKE '%白色%'
                            --ORDER BY B.FNUMBER

                            --UNION ALL

                            ----原漆(因数量较大,故根据‘物料分组(辅助)’来限制原漆物料的记录;注:只需以物料分组(辅助)‘原漆半成品’及‘原漆’为条件来查询)
                            --SELECT /*a.F_YTC_ASSISTANT5,*/A.FMATERIALID,A.FNUMBER--,a.FDOCUMENTSTATUS 
                            --FROM dbo.T_BD_MATERIAL A
                            --WHERE A.F_YTC_ASSISTANT5 IN('571f36aa14afdc','571f36b714afde')     --571f36aa14afdc(原漆半成品) 571f36b714afde(原漆)
                            --AND A.FDOCUMENTSTATUS='C'

                            UNION ALL

                            --系数
                            SELECT /*A.FID,A.FNUMBER,B.FNUMBER 编号,*/8 TYPEID,B.FENTRYID ID,C.FDATAVALUE NAME,GETDATE() CreateDt
                            FROM dbo.T_BAS_ASSISTANTDATA a
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY B ON A.FID=B.FID
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L C ON B.FENTRYID=C.FENTRYID AND C.FLOCALEID=2052
                            WHERE A.FID='56d7e144429983'
                            AND B.FDOCUMENTSTATUS='C' --C.FDATAVALUE LIKE '%3系%'
                            --ORDER BY B.FNUMBER

                            UNION ALL

                            --水性油性
                            SELECT /*A.FID,A.FNUMBER,B.FNUMBER 编号,*/9 TYPEID,B.FENTRYID ID,C.FDATAVALUE NAME,GETDATE() CreatDt
                            FROM dbo.T_BAS_ASSISTANTDATA a
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY B ON A.FID=B.FID
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L C ON B.FENTRYID=C.FENTRYID AND C.FLOCALEID=2052
                            WHERE A.FID='56d8e5de429996'
                            AND B.FDOCUMENTSTATUS='C' --C.FDATAVALUE LIKE '%水性%'
                            --ORDER BY B.FNUMBER

                            UNION ALL

                            --原漆半成品属性
                            SELECT /*A.FID,A.FNUMBER,B.FNUMBER 编号,*/10 TYPEID,B.FENTRYID ID,C.FDATAVALUE NAME,GETDATE() CreateDt
                            FROM dbo.T_BAS_ASSISTANTDATA a
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY B ON A.FID=B.FID
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L C ON B.FENTRYID=C.FENTRYID AND C.FLOCALEID=2052
                            WHERE A.FID='56d906f842999c'
                            AND B.FDOCUMENTSTATUS='C'-- C.FDATAVALUE LIKE '%树脂类%'
                            --ORDER BY B.FNUMBER

                            UNION ALL

                            --开票信息
                            SELECT /*A.FID,A.FNUMBER,B.FNUMBER 编号,*/11 TYPEID,B.FENTRYID ID,C.FDATAVALUE NAME,GETDATE() CreateDt
                            FROM dbo.T_BAS_ASSISTANTDATA a
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY B ON A.FID=B.FID
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L C ON B.FENTRYID=C.FENTRYID AND C.FLOCALEID=2052
                            WHERE A.FID='56d7af9f429973'
                            AND B.FDOCUMENTSTATUS='C' --C.FDATAVALUE LIKE '%1K号码漆%'
                            --ORDER BY B.FNUMBER

                            UNION ALL

                            --研发类别
                            SELECT /*A.FID,A.FNUMBER,*/12 TYPEID,B.FENTRYID ID,C.FDATAVALUE NAME,GETDATE() CreateDt
                            FROM dbo.T_BAS_ASSISTANTDATA a
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY B ON A.FID=B.FID
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L C ON B.FENTRYID=C.FENTRYID AND C.FLOCALEID=2052
                            WHERE a.FID='5dd1e57223d58f'
                            
                            UNION ALL                            

                            ---------------------------------------------------------------------------
                            --包装罐(包装箱)
                            SELECT /*a.F_YTC_TEXT 编码,*/13 TYPEID,convert(varchar(100),A.FID) ID,a.F_YTC_TEXT1 NAME,GETDATE() CreateDt 
                            FROM dbo.ytc_t_Cust100010 a --WHERE a.F_YTC_TEXT='T-0001'
                            --ORDER BY A.F_YTC_TEXT

                            UNION ALL

                            --物料分组(辅助)
                            SELECT /*A.FID,A.FNUMBER,B.FNUMBER 编号,*/14 TYPEID,B.FENTRYID ID,C.FDATAVALUE NAME,GETDATE() CreateDt
                            FROM dbo.T_BAS_ASSISTANTDATA a
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY B ON A.FID=B.FID
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L C ON B.FENTRYID=C.FENTRYID AND C.FLOCALEID=2052
                            WHERE A.FID='571f344214afd4'
                            AND B.FDOCUMENTSTATUS='C' --C.FDATAVALUE LIKE '%供应客户原材料%'
                            --ORDER BY B.FNUMBER

                            UNION ALL

                            --物料分组
                            SELECT 15 TYPEID,convert(varchar(100),B.FID) ID,A.FNUMBER+'('+B.FNAME+')' NAME,GETDATE() CreateDt
                            FROM dbo.T_BD_MATERIALGROUP A
                            INNER JOIN dbo.T_BD_MATERIALGROUP_L B ON A.FID=B.FID AND B.FLOCALEID=2052

                            /*SELECT /*A.FNUMBER 父编码,*/15 TYPEID,convert(varchar(100),B.FID) ID,B.FNUMBER+'('+C.FNAME+')' NAME,GETDATE() CreateDt
                            FROM dbo.T_BD_MATERIALGROUP A
                            INNER JOIN dbo.T_BD_MATERIALGROUP B ON A.FID=B.FPARENTID
                            INNER JOIN dbo.T_BD_MATERIALGROUP_L C ON B.FID=C.FID AND C.FLOCALEID=2052
                            WHERE a.FPARENTID=0*/
                            --ORDER BY A.FID

                            UNION ALL

                            --存货类别
                            SELECT /*A.FNUMBER,*/16 TYPEID,convert(varchar(100),A.FCATEGORYID) ID,B.FNAME NAME,GETDATE() CreateDt
                            FROM dbo.T_BD_MATERIALCATEGORY A
                            INNER JOIN T_BD_MATERIALCATEGORY_L B ON A.FCATEGORYID=B.FCATEGORYID AND B.FLOCALEID=2052
                            WHERE A.FDOCUMENTSTATUS='C'
                            --ORDER BY a.FCATEGORYID

                            UNION ALL

                            --默认税率
                            SELECT /*A.FNUMBER,*/17 TYPEID,convert(varchar(100),A.FID) ID,B.FNAME NAME,GETDATE() CreateDt--,A.FTAXRATE
                            FROM dbo.T_BD_TAXRATE A
                            INNER JOIN dbo.T_BD_TAXRATE_L B ON A.FID=B.FID AND B.FLOCALEID=2052
                            --ORDER BY A.FID

                            UNION ALL

                            --基本单位
                            SELECT /*a.FNUMBER,*/18 TYPEID,convert(varchar(100),a.FUNITID) ID,b.FNAME NAME,GETDATE() CreateDt
                            FROM dbo.T_BD_UNIT a
                            INNER JOIN dbo.T_BD_UNIT_L b ON a.FUNITID=b.FUNITID AND b.FLOCALEID=2052
                            WHERE A.FIsBaseUnit='1'
                            --ORDER BY A.FNUMBER

                            )X
                        ";
            return _result;
        }


        /// <summary>
        /// 导入EXCEL时动态生成临时表使用(注:只针对已绑定的字段进行创建)
        /// </summary>
        /// <returns></returns>
        public string Get_SearchExcelTemp()
        {
            _result = @"
                            SELECT A.ExcelCol 名称,A.ExcelColDataType 数据类型
                            FROM dbo.T_MAT_BindExcelCol A
                            where A.Bindid=0
                       ";
            return _result;
        }

        #region 生成新物料相关

        /// <summary>
        /// 根据指定条件查询出“U订货商品分类”ERPcode信息
        /// </summary>
        /// <param name="oneLeverName">一级商品分类</param>
        /// <param name="twoLeverName">二级商品分类</param>
        /// <param name="dtlname">三级商品分类</param>
        /// <returns></returns>
        public string Get_SearchUProdceType(string oneLeverName,string twoLeverName,string dtlname)
        {
            if (oneLeverName != "" && twoLeverName != "" && dtlname == "")
            {
                _result = $@"
                            SELECT B.ErpCode FROM dbo.T_MAT_UProductType A
                            INNER JOIN dbo.T_MAT_UProductTypeDtl B ON A.FId=B.Fid AND b.FParentId=0
                            WHERE A.FName='{oneLeverName}'   --百乐高
                            AND b.FName='{twoLeverName}'     --辅料系列
                        ";
            }
            else if (oneLeverName != "" && twoLeverName != "" && dtlname != "")
            {
                _result = $@"
                            SELECT C.ErpCode FROM dbo.T_MAT_UProductType A
                            INNER JOIN dbo.T_MAT_UProductTypeDtl B ON A.FId=B.Fid AND b.FParentId=0
                            INNER JOIN dbo.T_MAT_UProductTypeDtl C ON B.Dtlid=C.FParentId 
                            WHERE A.FName='{oneLeverName}'   --百乐高
                            AND b.FName='{twoLeverName}'     --辅料系列
                            AND c.FName='{dtlname}'          --PC-底漆类
                        ";
            }

            return _result;
        }

        /// <summary>
        /// 获取原漆K3记录(创建新物料时使用)
        /// </summary>
        /// <param name="fname"></param>
        /// <returns></returns>
        public string Get_SearchK3BinRecord(string fname)
        {
            _result = $@"
                            SELECT A.FMATERIALID
                            FROM dbo.T_BD_MATERIAL A
                            INNER JOIN dbo.T_BD_MATERIAL_L B ON A.FMATERIALID=B.FMATERIALID AND B.FLOCALEID=2052
                            WHERE A.F_YTC_ASSISTANT5 IN('571f36aa14afdc','571f36b714afde')     --571f36aa14afdc(原漆半成品) 571f36b714afde(原漆)
                            AND A.FDOCUMENTSTATUS='C'
                            AND B.FNAME='{fname}'
                        ";
            return _result;
        }

        /// <summary>
        /// 获取基础资料数据源(创建新物料时使用)
        /// </summary>
        /// <param name="typeid">类型ID,(0:品牌 1:分类 2:品类 3:组份 4:干燥性 5:常规/订制 6:阻击产品 7:颜色 8:系数 9:水性油性 10:原漆半成品属性 11:开票信息 12:研发类别
        ///13:包装罐(包装箱) 14:物料分组(辅助) 15:物料分组 16:存货类别 17:默认税率 18:基本单位)</param>
        /// <param name="fname">名称</param>
        /// <returns></returns>
        public string Get_SearchSourceRecord(int typeid, string fname)
        {
            _result = $@"
                            SELECT A.Id 
                            FROM dbo.T_MAT_Source A
                            WHERE A.Typeid='{typeid}'
                            AND A.FName='{fname}'
                        ";
            return _result;
        }

        #region 生成所需相关SQL(主要包括生成主键及相关表对应内容)

        /// <summary>
        /// 根据‘品牌’及‘规格型号’查找出TOP 1的记录
        /// </summary>
        /// <param name="bin">品牌名称</param>
        /// <param name="kui">规格型号</param>
        /// <returns></returns>
        public string Get_SearchTop1MaterialRecord(string bin,string kui)
        {
            _result = $@"
                            SELECT TOP 1 A.FMATERIALID
                            FROM dbo.T_BD_MATERIAL A
                            INNER JOIN dbo.T_BD_MATERIAL_L B ON A.FMATERIALID=B.FMATERIALID AND B.FLOCALEID=2052
                            WHERE B.FSPECIFICATION='{kui}'
                            AND A.F_YTC_ASSISTANT7=(
                                                        SELECT B.FENTRYID id
                                                        FROM dbo.T_BAS_ASSISTANTDATA a
                                                        INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY B ON A.FID=B.FID
                                                        INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L C ON B.FENTRYID=C.FENTRYID AND C.FLOCALEID=2052
                                                        WHERE A.FID='57623fb7940f6a'--C.FDATAVALUE LIKE '%威施乐%'
							                            AND C.FDATAVALUE='{bin}'
                                                    )
                            ORDER BY a.FCREATEDATE
                        ";
            return _result;
        }

        /// <summary>
        /// 根据typeid获取各表的主键值(一共14个)
        /// </summary>
        /// <param name="typeid">类型标记;0:T_BD_MATERIAL 1:T_BD_MATERIAL_L 2:t_BD_MaterialBase 3:t_BD_MaterialStock 4:t_BD_MaterialSale 
        ///                      5:t_bd_MaterialPurchase 6:t_BD_MaterialPlan 7:t_BD_MaterialProduce 8:t_BD_MaterialAuxPty 9:t_BD_MaterialInvPty 
        ///                      10:t_bd_MaterialSubcon 11:T_BD_MATERIALQUALITY 12:T_BD_UNITCONVERTRATE 13:T_MAT_ImportHistoryRecord_Key</param>
        /// <returns></returns>
        public string Get_MakeDtidKey(int typeid)
        {
            switch (typeid)
            {
                //T_BD_MATERIAL
                case 0:
                    _result = @"
                            DECLARE
	                            @id INT;
                            BEGIN
	                            INSERT INTO dbo.Z_BAS_ITEM( Column1 )
	                            VALUES  (1)

	                            SELECT @id=Id FROM dbo.Z_BAS_ITEM

	                            DELETE FROM dbo.Z_BAS_ITEM

	                            SELECT @id
                            END
                       ";
                    break;
                //T_BD_MATERIAL_L
                case 1:
                    _result = @"
                            DECLARE
	                            @id INT;
                            BEGIN
	                            INSERT INTO dbo.Z_BD_MATERIAL_L( Column1 )
	                            VALUES  (1)

	                            SELECT @id=Id FROM dbo.Z_BD_MATERIAL_L

	                            DELETE FROM dbo.Z_BD_MATERIAL_L

	                            SELECT @id
                            END
                       ";
                    break;
                //t_BD_MaterialBase
                case 2:
                    _result = @"
                            DECLARE
	                            @id INT;
                            BEGIN
	                            INSERT INTO dbo.Z_BD_MATERIALBASE( Column1 )
	                            VALUES  (1)

	                            SELECT @id=Id FROM dbo.Z_BD_MATERIALBASE

	                            DELETE FROM dbo.Z_BD_MATERIALBASE

	                            SELECT @id
                            END
                       ";
                    break;
                //t_BD_MaterialStock
                case 3:
                    _result = @"
                            DECLARE
	                            @id INT;
                            BEGIN
	                            INSERT INTO dbo.Z_BD_MATERIALSTOCK( Column1 )
	                            VALUES  (1)

	                            SELECT @id=Id FROM dbo.Z_BD_MATERIALSTOCK

	                            DELETE FROM dbo.Z_BD_MATERIALSTOCK

	                            SELECT @id
                            END
                       ";
                    break;
                //t_BD_MaterialSale
                case 4:
                    _result = @"
                            DECLARE
	                            @id INT;
                            BEGIN
	                            INSERT INTO dbo.Z_BD_MATERIALSALE( Column1 )
	                            VALUES  (1)

	                            SELECT @id=Id FROM dbo.Z_BD_MATERIALSALE

	                            DELETE FROM dbo.Z_BD_MATERIALSALE

	                            SELECT @id
                            END
                       ";
                    break;
                //t_bd_MaterialPurchase
                case 5:
                    _result = @"
                            DECLARE
	                            @id INT;
                            BEGIN
	                            INSERT INTO dbo.Z_BD_MATERIALPURCHASE( Column1 )
	                            VALUES  (1)

	                            SELECT @id=Id FROM dbo.Z_BD_MATERIALPURCHASE

	                            DELETE FROM dbo.Z_BD_MATERIALPURCHASE

	                            SELECT @id
                            END
                       ";
                    break;
                //t_BD_MaterialPlan
                case 6:
                    _result = @"
                            DECLARE
	                            @id INT;
                            BEGIN
	                            INSERT INTO dbo.Z_BD_MATERIALPLAN( Column1 )
	                            VALUES  (1)

	                            SELECT @id=Id FROM dbo.Z_BD_MATERIALPLAN

	                            DELETE FROM dbo.Z_BD_MATERIALPLAN

	                            SELECT @id
                            END
                       ";
                    break;
                //t_BD_MaterialProduce
                case 7:
                    _result = @"
                            DECLARE
	                            @id INT;
                            BEGIN
	                            INSERT INTO dbo.Z_BD_MATERIALPRODUCE( Column1 )
	                            VALUES  (1)

	                            SELECT @id=Id FROM dbo.Z_BD_MATERIALPRODUCE

	                            DELETE FROM dbo.Z_BD_MATERIALPRODUCE

	                            SELECT @id
                            END
                       ";
                    break;
                //t_BD_MaterialAuxPty
                case 8:
                    _result = @"
                            DECLARE
	                            @id INT;
                            BEGIN
	                            INSERT INTO dbo.Z_BD_MATERIALAUXPTY( Column1 )
	                            VALUES  (1)

	                            SELECT @id=Id FROM dbo.Z_BD_MATERIALAUXPTY

	                            DELETE FROM dbo.Z_BD_MATERIALAUXPTY

	                            SELECT @id
                            END
                       ";
                    break;
                //t_BD_MaterialInvPty
                case 9:
                    _result = @"
                            DECLARE
	                            @id INT;
                            BEGIN
	                            INSERT INTO dbo.Z_BD_MATERIALINVPTY( Column1 )
	                            VALUES  (1)

	                            SELECT @id=Id FROM dbo.Z_BD_MATERIALINVPTY

	                            DELETE FROM dbo.Z_BD_MATERIALINVPTY

	                            SELECT @id
                            END
                       ";
                    break;
                //t_bd_MaterialSubcon
                case 10:
                    _result = @"
                            DECLARE
	                            @id INT;
                            BEGIN
	                            INSERT INTO dbo.Z_BD_MATERIALSUBCON( Column1 )
	                            VALUES  (1)

	                            SELECT @id=Id FROM dbo.Z_BD_MATERIALSUBCON

	                            DELETE FROM dbo.Z_BD_MATERIALSUBCON

	                            SELECT @id
                            END
                       ";
                    break;
                //T_BD_MATERIALQUALITY
                case 11:
                    _result = @"
                            DECLARE
	                            @id INT;
                            BEGIN
	                            INSERT INTO dbo.Z_BD_MATERIALQUALITY( Column1 )
	                            VALUES  (1)

	                            SELECT @id=Id FROM dbo.Z_BD_MATERIALQUALITY

	                            DELETE FROM dbo.Z_BD_MATERIALQUALITY

	                            SELECT @id
                            END
                       ";
                    break;
                //T_BD_UNITCONVERTRATE
                case 12:
                    _result = @"
                            DECLARE
	                            @id INT;
                            BEGIN
	                            INSERT INTO dbo.Z_BD_UNITCONVERTRATE( Column1 )
	                            VALUES  (1)

	                            SELECT @id=Id FROM dbo.Z_BD_UNITCONVERTRATE

	                            DELETE FROM dbo.Z_BD_UNITCONVERTRATE

	                            SELECT @id
                            END
                       ";
                    break;
                //T_MAT_ImportHistoryRecord_Key
                case 13:
                    _result = @"
                            DECLARE
	                            @id INT;
                            BEGIN
	                            INSERT INTO dbo.T_MAT_ImportHistoryRecord_Key( Column1 )
	                            VALUES  (1)

	                            SELECT @id=Id FROM dbo.T_MAT_ImportHistoryRecord_Key

	                            DELETE FROM dbo.T_MAT_ImportHistoryRecord_Key

	                            SELECT @id
                            END
                       ";
                    break;
            }
            return _result;
        }

        /// <summary>
        /// 作用:1)根据fmaterialid查询出数据源 2)用于动态生成临时表(最后更新及插入使用)此点只适合T_BD_UNITCONVERTRATE使用
        /// </summary>
        /// <param name="typeid">类型标记;0:T_BD_MATERIAL 1:T_BD_MATERIAL_L 2:t_BD_MaterialBase 3:t_BD_MaterialStock 4:t_BD_MaterialSale 
        ///                      5:t_bd_MaterialPurchase 6:t_BD_MaterialPlan 7:t_BD_MaterialProduce 8:t_BD_MaterialAuxPty 9:t_BD_MaterialInvPty 
        ///                      10:t_bd_MaterialSubcon 11:T_BD_MATERIALQUALITY 12:T_BD_UNITCONVERTRATE</param>
        /// <param name="fmaterialid"></param>
        /// <returns></returns>
        public string Get_SearchMaterialSource(int typeid,int fmaterialid)
        {
            switch (typeid)
            {
                //T_BD_MATERIAL
                case 0:
                    _result = $@"
                            SELECT FMATERIALID ,FNUMBER ,FOLDNUMBER ,FMNEMONICCODE ,FMASTERID ,
                                   FMATERIALGROUP ,FCREATEORGID ,FUSEORGID ,FCREATORID ,FCREATEDATE ,
	                               FMODIFIERID ,FMODIFYDATE ,FDOCUMENTSTATUS ,FFORBIDSTATUS ,FAPPROVERID ,
                                   FAPPROVEDATE ,FFORBIDDERID ,FFORBIDDATE ,FIMAGE ,FPLMMATERIALID ,
                                   FMATERIALSRC ,FIMAGEFILESERVER ,FIMGSTORAGETYPE ,F_YTC_ASSISTANT ,F_YTC_ASSISTANT1 ,
                                   F_YTC_ASSISTANT11 ,F_YTC_ASSISTANT111 ,F_YTC_ASSISTANT1111 ,F_YTC_ASSISTANT11111 ,
                                   F_YTC_ASSISTANT111111 ,F_YTC_BASE ,F_YTC_ASSISTANT2 ,F_YTC_ASSISTANT3 ,F_YTC_ASSISTANT31 ,
                                   F_YTC_DECIMAL ,F_YTC_BASE1 ,F_YTC_DECIMAL1 ,F_YTC_DECIMAL2 ,F_YTC_TEXT ,
                                   F_YTC_REMARK ,F_YTC_ASSISTANT4 ,F_YTC_ASSISTANT5 ,F_YTC_IMAGE ,FISSALESBYNET ,
                                   F_YTC_ASSISTANT6 ,F_YTC_ASSISTANT7 ,F_YTC_REMARK1 ,F_YTC_INTEGER ,F_YTC_DECIMAL3 ,
                                   F_YTC_DECIMAL4 ,F_YTC_DECIMAL5 ,F_YTC_DECIMAL6 ,F_YTC_TEXT1 ,F_YTC_TEXT2,
                                   F_YTC_BASE2 ,F_YTC_BASE3 ,F_YTC_TEXT9 ,F_YTC_DECIMAL7 ,F_YTC_DECIMAL8 ,
                                   F_YTC_TEXT8 ,F_YTC_TEXT7 ,F_YTC_TEXT6 ,F_YTC_TEXT5 ,F_YTC_ASSISTANT8 ,
                                   F_YTC_TEXT4 ,F_YTC_TEXT3 ,F_YTC_TEXT10 ,F_YTC_TEXT11 ,F_YTC_REMARK2 ,
                                   F_YTC_REMARK3
                            FROM dbo.T_BD_MATERIAL
                            WHERE FMATERIALID='{fmaterialid}'
                        ";
                    break;
                //T_BD_MATERIAL_L
                case 1:
                    _result = $@"
                            SELECT A.FPKID ,A.FMATERIALID ,A.FLOCALEID ,A.FNAME ,A.FSPECIFICATION ,
                                   A.FDESCRIPTION 
                            FROM dbo.T_BD_MATERIAL_L A
                            INNER JOIN T_BD_MATERIAL B ON A.FMATERIALID = B.FMATERIALID 
                            WHERE B.FMATERIALID='{fmaterialid}'
                        ";
                    break;
                //T_BD_MATERIALBASE
                case 2:
                    _result = $@"
                                    SELECT A.FENTRYID ,A.FMATERIALID ,A.FERPCLSID ,A.FCATEGORYID ,
                                           A.FBASEUNITID ,A.FISPURCHASE ,A.FISSALE ,A.FISINVENTORY ,
                                           A.FISPRODUCE ,A.FISSUBCONTRACT ,A.FISPICK ,A.FISASSET ,
                                           A.FISREALTIMEACCOUT ,A.FTAXTYPE ,A.FTYPEID ,A.FTAXRATEID ,
                                           A.FISVMIBUSINESS ,A.FGROSSWEIGHT ,A.FNETWEIGHT ,A.FWEIGHTUNITID ,
                                           A.FLENGTH ,A.FWIDTH ,A.FHEIGHT,A.FVOLUME ,A.FVOLUMEUNITID ,
                                           A.FBARCODE ,A.FCONFIGTYPE 
                                    FROM dbo.T_BD_MATERIALBASE A
                                    INNER JOIN T_BD_MATERIAL ON A.FMATERIALID = T_BD_MATERIAL.FMATERIALID 
                                    WHERE T_BD_MATERIAL.FMATERIALID = '{fmaterialid}';
                                ";
                    break;
                //T_BD_MATERIALSTOCK
                case 3:
                    _result = $@"
                                SELECT A.FENTRYID ,A.FMATERIALID ,A.FSTOREUNITID ,A.FAUXUNITID ,
                                        A.FISSTOCKLIMIT ,A.FSTOCKID ,A.FISSPLIMIT ,A.FSTOCKPLACEID ,
                                        A.FSTOCKERID ,A.FDELIVERYLEADTIME ,A.FISWORKDAY ,A.FISLOCKSTOCK ,
                                        A.FISAUTOUNLOCKSTOCK ,A.FAUTOUNLOCKSTOCKDAYS ,A.FISBATCHMANAGE ,A.FBATCHRULEID ,
                                        A.FISKFPERIOD ,A.FEXPUNIT ,A.FEXPPERIOD ,A.FISEXPPARTOFLOT ,
	                                    A.FONLINELIFE ,A.FSTOREURNUM ,A.FSTOREURNOM ,A.FISCYCLECOUNTING ,
                                        A.FISMUSTCOUNTING ,A.FBATCHLEVEL ,A.FCURRENCYID ,A.FREFCOST ,
                                        A.FCOUNTCYCLE ,A.FCOUNTDAY ,A.FISSNMANAGE ,A.FSNCODERULE ,
                                        A.FSNUNIT ,A.FSNMANAGETYPE ,A.FISAUTOCREATEDCSN ,A.FSNCREATETIME ,
                                        A.FSAFESTOCK ,A.FREORDERGOOD ,A.FMINSTOCK ,A.FMAXSTOCK ,
                                        A.FUNITCONVERTDIR ,A.FISENABLEMINSTOCK ,A.FISENABLESAFESTOCK ,A.FISENABLEREORDER ,
                                        A.FISENABLEMAXSTOCK ,A.FECONREORDERQTY ,A.FISSNPRDTRACY 
                                FROM dbo.T_BD_MATERIALSTOCK A
                                INNER JOIN T_BD_MATERIAL ON A.FMATERIALID = T_BD_MATERIAL.FMATERIALID 
                                WHERE T_BD_MATERIAL.FMATERIALID = '{fmaterialid}'
                                ";
                    break;
                //T_BD_MATERIALSALE
                case 4:
                    _result = $@"
                                    SELECT A.FENTRYID ,A.FMATERIALID ,A.FSALEUNITID ,A.FSALEPRICEUNITID ,
                                           A.FORDERQTY ,A.FMINQTY ,A.FMAXQTY ,A.FOVERSENDPERCENT ,
                                           A.FISATPCHECK ,A.FISINVOICE ,A.FISOVERSEND ,A.FISRETURN ,
                                           A.FISRETURNPART ,A.FISRETURNCHECK ,A.FSALEURNUM ,A.FSALEURNOM ,
                                           A.FSALEPRICEURNUM ,A.FSALEPRICEURNOM ,A.FOUTSTOCKLMTH ,A.FOUTSTOCKLMTL ,
                                           A.FAGENTSALREDUCERATE ,A.FALLOWPUBLISH ,A.FGOODSTYPEID ,A.FISAFTERSALE ,
                                           A.FISPRODUCTFILES ,A.FISWARRANTED ,A.FWARRANTYUNITID ,A.FWARRANTY ,
                                           A.FOUTLMTUNIT ,A.FTAXCATEGORYCODEID 
                                    FROM dbo.T_BD_MATERIALSALE A
                                    INNER JOIN T_BD_MATERIAL ON A.FMATERIALID = T_BD_MATERIAL.FMATERIALID 
                                    WHERE T_BD_MATERIAL.FMATERIALID = '{fmaterialid}';
                                ";
                    break;
                //T_BD_MATERIALPURCHASE
                case 5:
                    _result = $@"
                                    SELECT A.FENTRYID ,A.FMATERIALID ,A.FPURCHASEUNITID ,A.FPURCHASEPRICEUNITID ,
                                           A.FPURCHASERID ,A.FPURCHASEGROUPID ,A.FDEFAULTVENDORID ,A.FISVENDORQUALIFICATION ,
                                           A.FISSOURCECONTROL ,A.FISPR ,A.FPURCHASETIMES ,A.FMINPOAMOUNT ,
                                           A.FISEXCESSRECEIVE ,A.FRECEIVEMAXSCALE ,A.FRECEIVEMINSCALE ,A.FRECEIVEADVANCEDAYS ,
                                           A.FRECEIVEDELAYDAYS ,A.FTAXCODE ,A.FPURURNUM ,A.FPURURNOM ,
                                           A.FPURPRICEURNUM ,A.FPURPRICEURNOM ,A.FISQUOTA ,A.FQUOTATYPE ,
                                           A.FMINSPLITQTY ,A.FAGENTPURPLUSRATE ,A.FCHARGEID ,A.FISLIMITPRICE ,
                                           A.FBASEMINSPLITQTY ,A.FISVMIBUSINESS ,A.FISRETURNMATERIAL ,A.FENABLESL ,
                                           A.FPURCHASEORGID ,A.FDEFBARCODERULEID ,A.FMINPACKCOUNT ,A.FPRINTCOUNT
                                    FROM dbo.T_BD_MATERIALPURCHASE A
                                    INNER JOIN T_BD_MATERIAL ON A.FMATERIALID = T_BD_MATERIAL.FMATERIALID 
                                    WHERE T_BD_MATERIAL.FMATERIALID = '{fmaterialid}'
                                ";
                    break;
                //T_BD_MATERIALPLAN
                case 6:
                    _result = $@"
                                    SELECT A.FENTRYID ,A.FMATERIALID ,A.FPLANNINGSTRATEGY ,A.FORDERPOLICY ,
                                           A.FPLANWORKSHOP ,A.FFIXLEADTIMETYPE ,A.FFIXLEADTIME ,A.FVARLEADTIMETYPE ,
                                           A.FVARLEADTIME ,A.FCHECKLEADTIMETYPE ,A.FCHECKLEADTIME ,A.FORDERINTERVALTIMETYPE ,
                                           A.FORDERINTERVALTIME ,A.FMAXPOQTY ,A.FMINPOQTY ,A.FINCREASEQTY ,A.FEOQ ,
                                           A.FVARLEADTIMELOTSIZE ,FBASEVARLEADTIMELOTSIZE ,FPLANINTERVALSDAYS ,FPLANBATCHSPLITQTY ,
                                           A.FREQUESTTIMEZONE ,FPLANTIMEZONE ,FPLANERID ,FISMRPCOMREQ ,FISMADETOPUR ,
	                                       A.FRESERVETYPE ,A.FCANLEADDAYS ,A.FLEADEXTENDDAY ,A.FCANDELAYDAYS ,
                                           A.FDELAYEXTENDDAY ,A.FPLANOFFSETTIMETYPE ,A.FPLANOFFSETTIME ,A.FPLANGROUPID ,
                                           A.FTIMEFACTORID ,A.FQTYFACTORID ,A.FSUPPLYSOURCEID ,A.FMFGPOLICYID ,
                                           A.FPLANMODE ,A.FALLOWPARTAHEAD ,A.FALLOWPARTDELAY ,A.FPLANSAFESTOCKQTY 
                                    FROM dbo.T_BD_MATERIALPLAN A
                                    INNER JOIN T_BD_MATERIAL ON A.FMATERIALID = T_BD_MATERIAL.FMATERIALID 
                                    WHERE T_BD_MATERIAL.FMATERIALID = '{fmaterialid}'
                                ";
                    break;
                //T_BD_MATERIALPRODUCE
                case 7:
                    _result = $@"
                                    SELECT A.FENTRYID ,A.FMATERIALID ,A.FWORKSHOPID ,A.FPRODUCEUNITID ,A.FBOMUNITID ,
                                           A.FUSETYPE ,A.FFIXLOSS ,A.FLOSSPERCENT ,A.FPROCESSID ,A.FISMAINPRD ,
                                           A.FISCOBY ,A.FDEFAULTROUTING ,A.FPERUNITSTANDHOUR ,A.FISSUETYPE ,A.FBKFLTIME ,
                                           A.FPICKSTOCKID ,A.FPICKBINID ,A.FISOVERISSUE ,A.FOVERISSUEPERCENT ,A.FISKITTING ,
                                           A.FFINISHRECEIPTOVERRATE ,A.FFINISHRECEIPTSHORTRATE ,A.FPRDURNUM ,A.FPRDURNOM ,
                                           A.FBOMURNUM ,A.FBOMURNOM ,A.FISCOMPLETESET ,A.FOVERCONTROLMODE ,A.FMINISSUEQTY ,
                                           A.FSTDLABORPREPARETIME ,A.FSTDLABORPROCESSTIME ,A.FSTDMACHINEPREPARETIME ,A.FSTDMACHINEPROCESSTIME ,
                                           A.FCONSUMVOLATITITY ,A.FISPRODUCTLINE ,A.FPRODUCEBILLTYPE ,A.FORGTRUSTBILLTYPE ,A.FISMINISSUEQTY ,
                                           A.FISECN ,A.FMINISSUEUNITID ,A.FMDLID ,A.FMDLMATERIALID 
                                    FROM dbo.T_BD_MATERIALPRODUCE A
                                    INNER JOIN T_BD_MATERIAL ON A.FMATERIALID = T_BD_MATERIAL.FMATERIALID 
                                    WHERE T_BD_MATERIAL.FMATERIALID = '{fmaterialid}'
                                ";
                    break;
                //T_BD_MATERIALAUXPTY
                case 8:
                    _result = $@"
                                    SELECT A.FENTRYID ,A.FMATERIALID ,A.FAUXPROPERTYID ,A.FISENABLE ,
                                           A.FISCOMCONTROL ,A.FISMUSTINPUT ,A.FISAFFECTPRICE ,A.FISAFFECTPLAN ,
                                           A.FISAFFECTCOST ,A.FVALUETYPE ,A.FVALUESET 
                                    FROM dbo.T_BD_MATERIALAUXPTY A
                                    INNER JOIN T_BD_MATERIAL ON A.FMATERIALID = T_BD_MATERIAL.FMATERIALID 
                                    WHERE T_BD_MATERIAL.FMATERIALID = '{fmaterialid}'
                                ";
                    break;
                //T_BD_MATERIALINVPTY
                case 9:
                    _result = $@"
                                    SELECT A.FENTRYID ,A.FMATERIALID ,A.FINVPTYID ,A.FISENABLE ,
                                           A.FISAFFECTPRICE ,A.FISAFFECTPLAN ,A.FISAFFECTCOST 
                                    FROM  dbo.T_BD_MATERIALINVPTY A
                                    INNER JOIN T_BD_MATERIAL ON A.FMATERIALID = T_BD_MATERIAL.FMATERIALID 
                                    WHERE T_BD_MATERIAL.FMATERIALID = '{fmaterialid}';
                                ";
                    break;
                //T_BD_MATERIALSUBCON
                case 10:
                    _result = $@"
                                    SELECT A.FENTRYID ,A.FMATERIALID ,A.FSUBCONUNITID ,A.FSUBCONPRICEUNITID ,
                                           A.FSUBCONURNUM ,A.FSUBCONURNOM ,A.FSUBCONPRICEURNUM ,
                                           A.FSUBCONPRICEURNOM ,A.FSUBBILLTYPE 
                                    FROM dbo.T_BD_MATERIALSUBCON A
                                    INNER JOIN T_BD_MATERIAL ON A.FMATERIALID = T_BD_MATERIAL.FMATERIALID 
                                    WHERE T_BD_MATERIAL.FMATERIALID = '{fmaterialid}';
                                ";
                    break;
                //T_BD_MATERIALQUALITY
                case 11:
                    _result = $@"
                                    SELECT A.FENTRYID ,A.FMATERIALID ,A.FCHECKPRODUCT ,
                                           A.FCHECKINCOMING ,A.FINCSAMPSCHEMEID ,A.FINCQCSCHEMEID ,A.FCHECKSTOCK ,
                                           A.FENABLECYCLISTQCSTK ,A.FSTOCKCYCLE ,A.FENABLECYCLISTQCSTKEW ,
                                           A.FEWLEADDAY ,A.FCHECKRETURN ,A.FCHECKDELIVERY ,A.FINSPECTGROUPID ,
                                           A.FINSPECTORID
                                    FROM dbo.T_BD_MATERIALQUALITY A
                                    INNER JOIN T_BD_MATERIAL ON A.FMATERIALID = T_BD_MATERIAL.FMATERIALID 
                                    WHERE T_BD_MATERIAL.FMATERIALID = '{fmaterialid}'
                                ";
                    break;
                //T_BD_UNITCONVERTRATE
                case 12:
                    _result = $@"
                                    SELECT FUNITCONVERTRATEID , FMASTERID ,FBILLNO ,
                                           FFORMID ,FMATERIALID ,FCURRENTUNITID ,
                                           FDESTUNITID ,FCONVERTTYPE ,FCONVERTNUMERATOR ,
                                           FCONVERTDENOMINATOR ,FCREATEORGID ,FUSEORGID ,
                                           FCREATORID ,FCREATEDATE ,FMODIFIERID ,
                                           FMODIFYDATE ,FAPPROVERID ,FAPPROVEDATE ,
                                           FFORBIDDERID ,FFORBIDDATE ,FDOCUMENTSTATUS ,
                                           FFORBIDSTATUS ,FUNITID 
                                    FROM dbo.T_BD_UNITCONVERTRATE
                                    WHERE 1=0    --只获取其表结构不获取内容
                                ";
                    break;
            }
            return _result;
        }

        /// <summary>
        /// 创建(更新)T_BAS_BILLCODES(编码规则最大编码表)中的相关记录
        /// 注:传输过来的物料编码只截取前8位（0~7）并且传过来的值必须为HAHAHAHA{{{{{0}}} 这种格式
        /// </summary>
        /// <param name="fmaterialnumber"></param>
        /// <returns></returns>
        public string Get_MakeUnitKey(string fmaterialnumber)
        {
            _result = $@"
                            IF NOT EXISTS (SELECT 1 FROM T_BAS_BILLCODES WHERE (FRULEID = 'eeb39bd33e0441d789661e1a7e8944f0' AND FBYVALUE = N'{fmaterialnumber}'))
                            BEGIN
                            INSERT INTO T_BAS_BILLCODES SELECT ISNULL(max(fcodeid), 0) + 1, 'eeb39bd33e0441d789661e1a7e8944f0', N'{fmaterialnumber}', 1.0000000000 FROM T_BAS_BILLCODES
                            END
                            ELSE
                            BEGIN
                            UPDATE T_BAS_BILLCODES SET FNUMMAX = (FNUMMAX + 1.0000000000) WHERE (FRULEID = 'eeb39bd33e0441d789661e1a7e8944f0' AND FBYVALUE = N'{fmaterialnumber}')
                            END;
                        ";
            return _result;
        }

        /// <summary>
        /// 根据物料编码在‘编码索引表’内查询出FNUMMAX值,以此构建‘物料单位换算’中FBILLNO的组成部份
        /// 注:传输过来的物料编码只截取前8位（0~7） 并且传过来的值必须为HAHAHAHA{{{{{0}}} 这种格式
        /// </summary>
        /// <param name="fmaterialnumber"></param>
        /// <returns>返回的结果为0000这种格式;如"0001"</returns>
        public string SearchUnitMaxKey(string fmaterialnumber)
        {
            _result = $@"
                            SELECT right(cast(power(10,4) as varchar)+CAST(a.FNUMMAX AS int),4) FNUMMAX
                            FROM dbo.T_BAS_BILLCODES A
                            WHERE A.FRULEID='eeb39bd33e0441d789661e1a7e8944f0'
                            AND A.FBYVALUE='{fmaterialnumber}'
                        ";
            return _result;
        }

        /// <summary>
        /// 根据新物料ID,将相关表进行删除
        /// </summary>
        /// <param name="fmaterialid"></param>
        /// <returns></returns>
        public string DelNewMaterialRecord(int fmaterialid)
        {
            _result = $@"
                             delete from dbo.T_BD_MATERIAL where fmaterialid='{fmaterialid}'
                             delete from dbo.T_BD_MATERIAL_L where fmaterialid='{fmaterialid}'
                             delete from dbo.t_BD_MaterialBase where fmaterialid='{fmaterialid}'
                             delete from dbo.t_BD_MaterialStock where fmaterialid='{fmaterialid}'
                             delete from dbo.t_BD_MaterialSale where fmaterialid='{fmaterialid}'
                             delete from dbo.t_bd_MaterialPurchase where fmaterialid='{fmaterialid}'
                             delete from dbo.t_BD_MaterialPlan where fmaterialid='{fmaterialid}'
                             delete from dbo.t_BD_MaterialProduce where fmaterialid='{fmaterialid}'
                             delete from dbo.t_BD_MaterialAuxPty where fmaterialid='{fmaterialid}'
                             delete from dbo.t_BD_MaterialInvPty where fmaterialid='{fmaterialid}'
                             delete from dbo.t_bd_MaterialSubcon where fmaterialid='{fmaterialid}'
                             delete from dbo.T_BD_MATERIALQUALITY where fmaterialid='{fmaterialid}'
                        ";
            return _result;
        }

        /// <summary>
        /// 初始化获取罐箱相关明细(创建新记录时使用)
        /// </summary>
        /// <returns></returns>
        public string GetGuanXuanDtl()
        {
            _result = $@"
                           SELECT a.F_YTC_TEXT 编码,a.F_YTC_TEXT1 名称,A.F_YTC_DECIMAL 罐重,
	                            A.F_YTC_DECIMAL1 长,A.F_YTC_DECIMAL2 宽,A.F_YTC_DECIMAL3 高,A.F_YTC_DECIMAL4 体积,
	                            A.F_YTC_DECIMAL5 箱重
                           FROM dbo.ytc_t_Cust100010 a 
                        ";

            return _result;
        }

        /// <summary>
        /// 根据导入过来的DT,查询并整理;若发现‘物料编码’已在DB内存在,即删除
        /// </summary>
        /// <returns></returns>
        public string SearchMaterialFnumber(string fnumber)
        {
            _result = $@"
                            SELECT FMATERIALID FROM dbo.T_BD_MATERIAL WHERE FNUMBER IN ({fnumber})
                        ";

            return _result;
        }

        #endregion

        #endregion

        #endregion

        #region 历史记录表相关

        /// <summary>
        /// 获取历史记录
        /// </summary>
        /// <param name="sdt">开始日期</param>
        /// <param name="edt">结束日期</param>
        /// <param name="fmaterialname">物料名称</param>
        /// <param name="fkui">规格型号</param>
        /// <param name="fbi">品牌</param>
        /// <param name="typeid">是否完成;0:是 1:否</param>
        /// <returns></returns>
        public string Get_SearchHistoryRecord(string sdt,string edt,string fmaterialname,string fkui,string fbi,int typeid)
        {
                _result = $@"
                            SELECT A.FID,A.FMaterialCode 物料编码,A.FMaerialName 物料名称,A.FBi 品牌,A.FKui 规格型号,
                                   CASE A.Finishid WHEN 0 THEN '是' ELSE '否' END 是否成功,
                                   A.FRemark 异常原因,A.ImportDt 导入时间
                            FROM dbo.T_MAT_ImportHistoryRecord A
                            WHERE (CONVERT(varchar(100), A.ImportDt, 23)) >='{sdt}'
							AND (CONVERT(varchar(100), A.ImportDt, 23)) <='{edt}'
                            AND (A.FMaerialName like '%{fmaterialname}%' or '{fmaterialname}' is null)
                            AND (A.FKui LIKE '%{fkui}%' or '{fkui}' is null)
                            AND (A.FBi like '%{fbi}%' or '{fbi}' is null)
                            AND A.Finishid ='{typeid}'
                            ORDER BY A.ImportDt
                            ";

            return _result;
        }

        #endregion
    }
}
