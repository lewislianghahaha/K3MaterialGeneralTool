using System;
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
        /// 绑定:更新T_MAT_BindExcelCol T_MAT_BindK3Col表中的BindId字段
        /// </summary>
        /// <param name="exceid"></param>
        /// <param name="k3Id"></param>
        public bool UpdateBindRecord(int exceid,int k3Id)
        {
            _sqlscript = sqlList.Update_Bind(exceid, k3Id);
            return search.Generdt(1,_sqlscript);
        }

        /// <summary>
        /// 解除绑定 对表T_MAT_BindRecord进行删除操作;对T_MAT_BindExcelCol T_MAT_BindK3Col进行更新操作
        /// </summary>
        /// <param name="k3Id"></param>
        /// <param name="fid"></param>
        /// <param name="exceid"></param>
        public bool UpdateRemoveBindRecord(int exceid,int k3Id,int fid)
        {
            var result = true;
            try
            {
                _sqlscript = sqlList.Update_RemoveBind(exceid, k3Id, fid);
                result=search.Generdt(1, _sqlscript);
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }


    }
}
