using System.Data;

namespace K3MaterialGeneralTool
{
    public class GlobalClasscs
    {

        /// <summary>
        /// 记录各初始化的数据表
        /// </summary>
        public struct ResultDt
        {
            public DataTable Resultdt;            //获取生成后的结果集
        }

        public static ResultDt RDt;
    }
}
