using K3MaterialGeneralTool.DB;

namespace K3MaterialGeneralTool.Task
{
    //更新
    public class Update
    {
        SqlList sqlList=new SqlList();
        Search search=new Search();

        private string _sqlscript = string.Empty;

        /// <summary>
        /// 更新T_MAT_BindExcelCol T_MAT_BindK3Col表中的BindId字段
        /// </summary>
        /// <param name="exceid"></param>
        /// <param name="k3Id"></param>
        public void UpdateBindRecord(int exceid,int k3Id)
        {
            _sqlscript = sqlList.Update_Bind(exceid, k3Id);
            search.UseSqlSearchIntoDt(1,_sqlscript);
        }


        
    }
}
