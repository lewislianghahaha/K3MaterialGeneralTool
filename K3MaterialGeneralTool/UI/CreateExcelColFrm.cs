using System;
using System.Data;
using System.Windows.Forms;
using K3MaterialGeneralTool.Task;

namespace K3MaterialGeneralTool.UI
{
    public partial class CreateExcelColFrm : Form
    {
        TaskLogic task=new TaskLogic();

        public CreateExcelColFrm()
        {
            InitializeComponent();
            OnRegisterEvents();
            OnInitialize();
        }

        private void OnRegisterEvents()
        {
            tmSave.Click += TmSave_Click;
            tmclose.Click += Tmclose_Click;
        }

        /// <summary>
        /// 初始化相关记录
        /// </summary>
        private void OnInitialize()
        {
            //初始化下拉列表
            OnShowSelectTypeList();
        }

        /// <summary>
        /// 初始化查询下拉列表
        /// </summary>
        private void OnShowSelectTypeList()
        {
            var dt = new DataTable();
            //创建表头
            for (var i = 0; i < 2; i++)
            {
                var dc = new DataColumn();
                switch (i)
                {
                    case 0:
                        dc.ColumnName = "Id";
                        break;
                    case 1:
                        dc.ColumnName = "Name";
                        break;
                }
                dt.Columns.Add(dc);
            }
            //创建行内容
            for (var j = 0; j < 4; j++)
            {
                var dr = dt.NewRow();

                switch (j)
                {
                    case 0:
                        dr[0] = "0";
                        dr[1] = "日期";
                        break;
                    case 1:
                        dr[0] = "1";
                        dr[1] = "整数";
                        break;
                    case 2:
                        dr[0] = "2";
                        dr[1] = "小数";
                        break;
                    case 3:
                        dr[0] ="3";
                        dr[1] = "字符串";
                        break;
                }
                dt.Rows.Add(dr);
            }

            comtypelist.DataSource = dt;
            comtypelist.DisplayMember = "Name"; //设置显示值
            comtypelist.ValueMember = "Id";    //设置默认值内码
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmSave_Click(object sender, EventArgs e)
        {
            try
            {
                //判断若'Excel字段名称'没有填,不能继续
                if(string.IsNullOrEmpty(txtname.Text))throw new Exception("检测到Excel字段名称没有填写,请填写后继续");
                //获取所选下拉列表值
                var dvtypelist = (DataRowView)comtypelist.Items[comtypelist.SelectedIndex];
                var colDataTypeName = Convert.ToString(dvtypelist["Name"]);

                //执行新增
                if(!task.InsertExcelNewCol(txtname.Text,colDataTypeName))throw new Exception("绑定异常,请联系管理员");
                else
                {
                    MessageBox.Show($"创建Excel新字段成功,请点击继续", $"提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmclose_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
