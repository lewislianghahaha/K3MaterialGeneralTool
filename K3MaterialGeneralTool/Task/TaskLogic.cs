using System.Data;

namespace K3MaterialGeneralTool.Task
{
    public class TaskLogic
    {
        Search searchDb=new Search();
        InsertGenerate generate = new InsertGenerate();
        Update update = new Update();

        #region 变量定义
        private int _exceid;                     //Excel绑定ID主键
        private int _k3Id;                       //K3绑定ID主键
        private int _fid;                        //绑定主键ID
        private DataTable _resultTable;          //返回DT类型
        private bool _resultMark;                //返回是否成功标记
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
            _resultMark = update.UpdateRemoveBindRecord(_exceid,_k3Id,_fid);
        }

        /// <summary>
        /// 创建EXCEL新绑定字段使用
        /// </summary>
        /// <param name="colname">新字段名称</param>
        /// <param name="coldatatypename">新字段数据类型名称</param>
        public bool InsertExcelNewCol(string colname,string coldatatypename)
        {
            return generate.InsertExcelNewCol(colname,coldatatypename);
        }

    }
}
