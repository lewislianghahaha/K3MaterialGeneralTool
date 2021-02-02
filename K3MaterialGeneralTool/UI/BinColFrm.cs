using System;
using System.Data;
using System.Threading;
using System.Windows.Forms;
using K3MaterialGeneralTool.Task;

namespace K3MaterialGeneralTool.UI
{
    public partial class BinColFrm : Form
    {
        Load load=new Load();
        TaskLogic task=new TaskLogic();

        public BinColFrm()
        {
            InitializeComponent();
            OnRegisterEvents();
            OnInitialize();
        }

        /// <summary>
        /// 初始化构建各按钮事件
        /// </summary>
        private void OnRegisterEvents()
        {
            tmBind.Click += TmBind_Click;
            tmremovebind.Click += Tmremovebind_Click;
            tmCreateExcelCol.Click += TmCreateExcelCol_Click;
            tmclose.Click += Tmclose_Click;
            comtypelist.SelectedIndexChanged += Comtypelist_SelectedIndexChanged;
        }

        /// <summary>
        /// 初始化相关信息
        /// </summary>
        private void OnInitialize()
        {
            //初始化查询下拉列表
            OnShowSelectTypeList();
            //初始化‘EXCEL’绑定字段列表
            OnSearchExcelBindRecord();
            //初始化‘K3’绑定字段列表
            OnSearchK3BindRecord();
            //初始化绑定关系记录
            OnSearchBindRecord();
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
            for (var j = 0; j < 3; j++)
            {
                var dr = dt.NewRow();

                switch (j)
                {
                    case 0:
                        dr[0] = "0";
                        dr[1] = "基本";
                        break;
                    case 1:
                        dr[0] = "1";
                        dr[1] = "库存";
                        break;
                    case 2:
                        dr[0] = "2";
                        dr[1] = "销售";
                        break;
                }
                dt.Rows.Add(dr);
            }

            comtypelist.DataSource = dt;
            comtypelist.DisplayMember = "Name"; //设置显示值
            comtypelist.ValueMember = "Id";    //设置默认值内码
        }

        /// <summary>
        /// ‘EXCEL’绑定字段列表(初始化及刷新时使用)
        /// </summary>
        private void OnSearchExcelBindRecord()
        {
            gvexcelBind.DataSource = task.SearchExcelBindRecord();
            //控制某些字段不显示
            ConvtrolExcelGridViewColShow();
        }

        /// <summary>
        /// ‘K3’绑定字段列表(初始化及刷新时使用)
        /// </summary>
        private void OnSearchK3BindRecord()
        {

            //根据所选择的‘类型’下拉列表获取对应数据源
            var dvtypelist = (DataRowView)comtypelist.Items[comtypelist.SelectedIndex];
            var typeid = Convert.ToInt32(dvtypelist["Id"]);
            gvk3bind.DataSource = task.SearchK3BindRecord(typeid);
            //控制某些字段不显示
            ControlK3GridViewisColShow();
        }

        /// <summary>
        /// 绑定关系记录(初始化及刷新时使用)
        /// </summary>
        private void OnSearchBindRecord()
        {
            gvbind.DataSource = task.SearchBind();
            //控制某些字段不显示
            ControlBindGridViewColShow();
        }

        /// <summary>
        /// 下拉列表ID发生变化时使用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Comtypelist_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                //刷新K3绑定字段GridView
                OnSearchK3BindRecord();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 绑定功能(先执行插入 后执行更新)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmBind_Click(object sender, EventArgs e)
        {
            try
            {
                //判断:必须选择行
                if(gvexcelBind.SelectedRows.Count==0 || gvk3bind.SelectedRows.Count==0)throw new Exception("检测到没有选择Excel或K3绑定字段,请选择后继续");
                //判断:若选择超过1行作出异常提示
                if (gvexcelBind.SelectedRows.Count > 1) throw new Exception($"检测到'Excel绑定字段'选择多于一行记录,请重新只选择一行进行绑定");
                if(gvk3bind.SelectedRows.Count > 1) throw new Exception($"检测到'K3绑定字段'选择多于一行记录,请重新只选择一行进行绑定");
                //判断若两者的数据类型不相同,即不能进行绑定
                if (Convert.ToString(gvexcelBind.SelectedRows[0].Cells[2].Value) != Convert.ToString(gvk3bind.SelectedRows[0].Cells[4].Value))
                    throw new Exception("检测到进行绑定的字段数据类型不一致,请重新进行选择");


                //将获取两者所选的行记录[0]ID值
                var excelColId = Convert.ToInt32(gvexcelBind.SelectedRows[0].Cells[0].Value);
                var k3ColId = Convert.ToInt32(gvk3bind.SelectedRows[0].Cells[0].Value);
                //将获取两者所选的行记录‘名称’
                var excelcolname = Convert.ToString(gvexcelBind.SelectedRows[0].Cells[1].Value);
                var k3Colname = Convert.ToString(gvk3bind.SelectedRows[0].Cells[3].Value);

                var clickMessage = $"您所选择需要进行绑定的字段为: \n Excel:'{excelcolname}',K3:'{k3Colname}' \n 是否继续?";
                if (MessageBox.Show(clickMessage, $"提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    task.Exceid = excelColId;
                    task.K3Id = k3ColId;

                    //子线程调用
                    new Thread(BindStart).Start();
                    load.StartPosition = FormStartPosition.CenterScreen;
                    load.ShowDialog();

                    if(!task.ResultMark) throw new Exception("绑定异常,请联系管理员");
                    {
                        MessageBox.Show($"绑定成功,请点击继续", $"提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //绑定完成后对三个GridView都进行数据刷新
                        OnSearchExcelBindRecord();
                        OnSearchK3BindRecord();
                        OnSearchBindRecord();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 清除绑定功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmremovebind_Click(object sender, EventArgs e)
        {
            try
            {
                //判断:‘绑定结果’GridView是否有选择行
                if(gvbind.SelectedRows.Count==0)throw new Exception("检测到没有'绑定'选择字段,请选择后继续");
                //判断:'绑定结果'GridView只能选择一行进行解除绑定
                if(gvbind.SelectedRows.Count>1)throw new Exception("检测到选择多于一行记录,请重新只选择一行进行绑定");

                //将获取所选的行记录[0]ID值
                var fid = Convert.ToInt32(gvbind.SelectedRows[0].Cells[0].Value);
                var excelColId = Convert.ToInt32(gvbind.SelectedRows[0].Cells[1].Value);
                var k3ColId = Convert.ToInt32(gvbind.SelectedRows[0].Cells[2].Value);

                var clickMessage = $"注意:当解除绑定后,导入时将不会对原来的K3字段进行更新 \n 是否继续?";
                if (MessageBox.Show(clickMessage, $"提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    task.Exceid = excelColId;
                    task.K3Id = k3ColId;
                    task.Fid = fid;

                    //子线程调用
                    new Thread(RemoveBindStart).Start();
                    load.StartPosition = FormStartPosition.CenterScreen;
                    load.ShowDialog();

                    if (!task.ResultMark) throw new Exception("解除绑定异常,请联系管理员");
                    {
                        MessageBox.Show($"解除绑定成功,请点击继续", $"提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //绑定完成后对三个GridView都进行数据刷新
                        OnSearchExcelBindRecord();
                        OnSearchK3BindRecord();
                        OnSearchBindRecord();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 新建EXCEL字段功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmCreateExcelCol_Click(object sender, EventArgs e)
        {
            try
            {
                var createExcelColFrm = new CreateExcelColFrm {StartPosition = FormStartPosition.CenterScreen};
                createExcelColFrm.ShowDialog();
                //返回后刷新Excel绑定GridView
                OnSearchExcelBindRecord();
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
            this.Close();
        }

        /// <summary>
        /// 控制ExcelGridView单元格显示方式-EXCEL绑定字段
        /// </summary>
        private void ConvtrolExcelGridViewColShow()
        {
            //注:当没有值时,若还设置某一行Row不显示的话,就会出现异常
            if (gvexcelBind?.Rows.Count >= 0)
                gvexcelBind.Columns[0].Visible = false;
        }

        /// <summary>
        /// 控制K3GridView单元格显示方式-K3绑定字段
        /// </summary>
        private void ControlK3GridViewisColShow()
        {
            //注:当没有值时,若还设置某一行Row不显示的话,就会出现异常
            if (gvk3bind?.Rows.Count >= 0)
            {
                gvk3bind.Columns[0].Visible = false;
                gvk3bind.Columns[1].Visible = false;
                gvk3bind.Columns[2].Visible = false;
            }

        }

        /// <summary>
        /// 控制绑定GridView单元格显示方式-绑定关系
        /// </summary>
        private void ControlBindGridViewColShow()
        {
            //注:当没有值时,若还设置某一行Row不显示的话,就会出现异常
            if (gvbind?.Rows.Count >= 0)
            {
                gvbind.Columns[0].Visible = false;
                gvbind.Columns[1].Visible = false;
                gvbind.Columns[2].Visible = false;
                gvbind.Columns[5].Visible = false;
                gvbind.Columns[6].Visible = false;
            }
        }

        /// <summary>
        ///子线程使用(重:用于监视功能调用情况,当完成时进行关闭LoadForm)-绑定时使用
        /// </summary>
        private void BindStart()
        {
            //绑定
            task.InsertBindRecord();

            //当完成后将Load子窗体关闭
            this.Invoke((ThreadStart)(() =>
            {
                load.Close();
            }));
        }

        /// <summary>
        /// 子线程使用(重:用于监视功能调用情况,当完成时进行关闭LoadForm)-解除绑定时使用
        /// </summary>
        private void RemoveBindStart()
        {
            //解除绑定
            task.UpdateRemoveBindRecord();

            //当完成后将Load子窗体关闭
            this.Invoke((ThreadStart)(() =>
            {
                load.Close();
            }));
        }
    }
}
