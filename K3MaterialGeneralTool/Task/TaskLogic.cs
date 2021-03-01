using System;
using System.Data;
using System.Security.Cryptography.X509Certificates;

namespace K3MaterialGeneralTool.Task
{
    public class TaskLogic
    {
        Search searchDb=new Search();
        InsertGenerate generate = new InsertGenerate();
        Update update = new Update();
        Import import=new Import();

        #region 变量定义

        #region 绑定相关变量
        private int _exceid;                     //Excel绑定ID主键
        private int _k3Id;                       //K3绑定ID主键
        private int _fid;                        //绑定主键ID
        #endregion

        #region 历史记录使用
        private string _sdt;                   //开始日期
        private string _edt;                   //结束日期
        private string _fmaterialname;           //物料名称
        private string _fkui;                    //规格型号
        private string _fbi;                     //品牌
        #endregion

        #region Excel导入
        private string _fileAddress;             //文件地址
        #endregion

        #region 生成

        private DataTable _importGridviewdt;     //从EXCEL导入的GridViewDT        
        private DataTable _importdt;             //从EXCEL导入的DT(检测是否已在DB存在使用)
        #endregion

        #region 返回变量
        private DataTable _resultTable;          //返回DT类型
        private bool _resultMark;                //返回是否成功标记
        #endregion

        #endregion

        #region Set(获取外部值)
        /// <summary>
        /// Excel绑定ID主键
        /// </summary>
        public int Exceid { set { _exceid = value; } }
        /// <summary>
        /// K3绑定ID主键
        /// </summary>
        public int K3Id { set { _k3Id = value; } }
        /// <summary>
        /// 绑定主键ID
        /// </summary>
        public int Fid { set { _fid = value; } }


        /// <summary>
        /// 开始日期
        /// </summary>
        public string Sdt { set { _sdt = value; } }
        /// <summary>
        /// 结束日期
        /// </summary>
        public string Edt { set { _edt = value; } }
        /// <summary>
        /// 物料名称
        /// </summary>
        public string Fmaterialname { set { _fmaterialname = value; } }
        /// <summary>
        /// 规格型号
        /// </summary>
        public string Fkui { set { _fkui = value; } }
        /// <summary>
        /// 品牌
        /// </summary>
        public string Fbi { set { _fbi = value; } }
        /// <summary>
        /// 文件地址
        /// </summary>
        public string FileAddress { set { _fileAddress = value; } }
        /// <summary>
        /// 从EXCEL导入的GridViewDT
        /// </summary>
        public DataTable ImportGridviewdt { set { _importGridviewdt = value; } }
        /// <summary>
        /// 从EXCEL导入的DT(检测是否已在DB存在使用)
        /// </summary>
        public DataTable Importdt { set { _importdt = value; } }
        #endregion

        #region Get(返回值至外部)
        /// <summary>
        ///返回DataTable至主窗体
        /// </summary>
        public DataTable ResultTable => _resultTable;

        /// <summary>
        /// 返回结果标记
        /// </summary>
        public bool ResultMark => _resultMark;
        #endregion

        #region 绑定功能相关
        /// <summary>
        /// 获取Excel绑定表记录
        /// </summary>
        /// <returns></returns>
        public DataTable SearchExcelBindRecord()
        {
            return searchDb.SearchExcelBindRecord().Copy();
        }

        /// <summary>
        /// K3字段绑定记录(在'绑定'功能显示及更新时使用)
        /// </summary>
        /// <returns></returns>
        public DataTable SearchK3BindRecord(int typeid)
        {
            return searchDb.SearchK3BindRecord(typeid).Copy();
        }

        /// <summary>
        /// 绑定关系记录
        /// </summary>
        /// <returns></returns>
        public DataTable SearchBind()
        {
            return searchDb.SearchBind().Copy();
        }

        /// <summary>
        /// 将数据插入至T_MAT_BindRecord表内并执行更新
        /// </summary>
        /// <returns></returns>
        public void InsertBindRecord()
        {
            _resultMark = generate.InsertBindRecord(_exceid, _k3Id);
        }

        /// <summary>
        /// 解除绑定
        /// </summary>
        public void UpdateRemoveBindRecord()
        {
            _resultMark = update.UpdateRemoveBindRecord(_exceid, _k3Id, _fid);
        }

        /// <summary>
        /// 创建EXCEL新绑定字段使用
        /// </summary>
        /// <param name="colname">新字段名称</param>
        /// <param name="coldatatypename">新字段数据类型名称</param>
        public bool InsertExcelNewCol(string colname, string coldatatypename)
        {
            return generate.InsertExcelNewCol(colname, coldatatypename);
        }
        #endregion

        #region  主窗体功能相关
        /// <summary>
        /// 同步K3基础资料
        /// </summary>
        /// <returns></returns>
        public void InsertK3SourceRecord()
        {
            _resultMark = generate.InsertK3SourceRecord();
        }

        /// <summary>
        /// 查询历史记录
        /// </summary>
        public void SearchHistoryRecord()
        {
            _resultTable = searchDb.SearchHistoryRecord(_sdt, _edt, _fmaterialname, _fkui, _fbi);
        }

        /// <summary>
        /// EXCEL导入
        /// </summary>
        public void ImportExcelToDt()
        {
            _resultTable = import.ImportExcelToDt(_fileAddress);
        }

        /// <summary>
        /// 生成
        /// </summary>
        /// <returns></returns>
        public bool GenerateRecord()
        {
           return _resultMark = generate.GenerateAndCreateK3NewMaterialRecord(_importGridviewdt);
        }

        /// <summary>
        /// 根据导入过来的DT,查询并整理;若发现‘物料编码’已在DB内存在,即删除
        /// </summary>
        public void SearchImportIdAndDel()
        {
            searchDb.SearchImportIdAndDel(_importdt);
        }

        #endregion

    }
}
