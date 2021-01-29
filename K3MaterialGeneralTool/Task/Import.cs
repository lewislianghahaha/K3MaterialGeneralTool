using System;
using System.Data;
using K3MaterialGeneralTool.DB;

namespace K3MaterialGeneralTool.Task
{
    //导入Excel
    public class Import
    {
        TempDtList tempDtList=new TempDtList();

        #region Excel模板导入

        /// <summary>
        /// EXcel导入
        /// </summary>
        /// <param name="fileAddress"></param>
        /// <returns></returns>
        public DataTable ImportExcelToDt(string fileAddress)
        {
            var dt = new DataTable();
            try
            {
                //使用NPOI技术进行导入EXCEL至DATATABLE
                var importExcelDt = OpenExcelToDataTable(fileAddress);
                //将从EXCEL过来的记录集为空的行清除
                dt = RemoveEmptyRows(importExcelDt);
            }
            catch (Exception)
            {
                dt.Columns.Clear();
                dt.Rows.Clear();
            }
            return dt;
        }



        #endregion
    }
}
