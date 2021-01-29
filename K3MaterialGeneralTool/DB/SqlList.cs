﻿using System;
using NPOI.SS.Formula.Functions;

namespace K3MaterialGeneralTool.DB
{
    public class SqlList
    {
        //根据SQLID返回对应的SQL语句  
        private string _result;

        #region 绑定相关

        /// <summary>
        /// Excel字段绑定记录(导入EXCEL时动态生成临时表使用)
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
                           SELECT A.Fid,A.ExcelColId,A.K3ColId,B.ExcelCol Excel字段名称,C.K3ColName K3字段名称,A.BindDt 绑定日期
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
                            /*SELECT /*A.FID,A.FNUMBER,*/12 TYPEID,B.FENTRYID ID,C.FDATAVALUE NAME,GETDATE() CreateDt
                            FROM dbo.T_BAS_ASSISTANTDATA a
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY B ON A.FID=B.FID
                            INNER JOIN dbo.T_BAS_ASSISTANTDATAENTRY_L C ON B.FENTRYID=C.FENTRYID AND C.FLOCALEID=2052
                            WHERE a.FID='5dd1e57223d58f'*/

                            ---------------------------------------------------------------------------
                            --包装罐(包装箱)
                            /*SELECT /*a.F_YTC_TEXT 编码,*/13 TYPEID,a.FID ID,a.F_YTC_TEXT1 NAME,GETDATE() CreateDt 
                            FROM dbo.ytc_t_Cust100010 a --WHERE a.F_YTC_TEXT='T-0001'
                            ORDER BY A.F_YTC_TEXT*/

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
                            SELECT /*A.FNUMBER 父编码,*/15 TYPEID,convert(varchar(100),B.FID) ID,B.FNUMBER+'('+C.FNAME+')' NAME,GETDATE() CreateDt
                            FROM dbo.T_BD_MATERIALGROUP A
                            INNER JOIN dbo.T_BD_MATERIALGROUP B ON A.FID=B.FPARENTID
                            INNER JOIN dbo.T_BD_MATERIALGROUP_L C ON B.FID=C.FID AND C.FLOCALEID=2052
                            WHERE a.FPARENTID=0
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
                            WHERE B.FNAME='{fname}'
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
        /// <returns></returns>
        public string Get_SearchHistoryRecord(DateTime sdt,DateTime edt,string fmaterialname,string fkui,string fbi)
        {
                _result = $@"
                            SELECT A.FMaterialCode 物料编码,A.FMaerialName 物料名称,A.FBi 品牌,A.FKui 规格型号,
                                   CASE A.Finishid WHEN 0 THEN '是' ELSE '否' END 是否成功,
                                   A.FRemark 异常原因,A.ImportDt 导入时间
                            FROM dbo.T_MAT_ImportHistoryRecord A
                            WHERE (CONVERT(varchar(100), A.ImportDt, 23) >=CONVERT(VARCHAR(100),'{sdt}',23)
							AND (CONVERT(varchar(100), A.ImportDt, 23) <=CONVERT(VARCHAR(100),'{edt}',23)
                            AND (A.FMaerialName like '%{fmaterialname}%' or '{fmaterialname}' is null)
                            AND (A.FKui LIKE '%{fkui}%' or '{fkui}' is null)
                            AND (A.FBi like '%{fbi}%' or '{fbi}' is null)
                            ORDER BY A.ImportDt
                       ";

            return _result;
        }

        #endregion
    }
}
