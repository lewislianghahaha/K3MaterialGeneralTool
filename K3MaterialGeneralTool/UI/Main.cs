using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using K3MaterialGeneralTool.Task;

namespace K3MaterialGeneralTool.UI
{
    public partial class Main : Form
    {
        TaskLogic task=new TaskLogic();
        Load load = new Load();

        #region 变量参数
        //保存查询出来的GridView记录
        private DataTable _dtl;
        //记录当前页数(GridView页面跳转使用)
        private int _pageCurrent = 1;
        //记录计算出来的总页数(GridView页面跳转使用)
        private int _totalpagecount;
        //记录初始化标记(GridView页面跳转 初始化时使用)
        private bool _pageChange;
        #endregion

        public Main()
        {
            InitializeComponent();
            OnInitialize();
            OnRegisterEvents();
            OnShowList();
        }

        private void OnRegisterEvents()
        {
            tmBindCol.Click += TmBindCol_Click;
            tmUpdate.Click += TmUpdate_Click;
            ////////////////新物料导入生成////////////////
            tmimportexcel.Click += Tmimportexcel_Click;
            tmGenerate.Click += TmGenerate_Click;

            ///////////////查询建档记录///////////////////
            btnSearch.Click += BtnSearch_Click;
            bnMoveFirstItem.Click += BnMoveFirstItem_Click;
            bnMovePreviousItem.Click += BnMovePreviousItem_Click;
            bnMoveNextItem.Click += BnMoveNextItem_Click;
            bnMoveLastItem.Click += BnMoveLastItem_Click;
            bnPositionItem.Leave += BnPositionItem_Leave;
            tmshowrows.DropDownClosed += Tmshowrows_DropDownClosed;
            panel5.Visible = false;
        }

        /// <summary>
        /// 初始化相关设置
        /// </summary>
        private void OnInitialize()
        {
            //设置TabControl控件默认显示页
            tbhistory.SelectedIndex = 1;
        }

        /// <summary>
        /// 初始化下拉列表(查询历史记录使用)
        /// </summary>
        private void OnShowList()
        {
            var dt=new DataTable();

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
            for (var j = 0; j < 2; j++)
            {
                var dr = dt.NewRow();

                switch (j)
                {
                    case 0:
                        dr[0] = "0";
                        dr[1] = "是";
                        break;
                    case 1:
                        dr[0] = "1";
                        dr[1] = "否";
                        break;
                }
                dt.Rows.Add(dr);
            }

            comlist.DataSource = dt.Copy();
            comlist.DisplayMember = "Name"; //设置显示值
            comlist.ValueMember = "Id";    //设置默认值内码
        }

        /// <summary>
        /// 绑定功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmBindCol_Click(object sender, EventArgs e)
        {
            try
            {
                var binColFrm = new BinColFrm {StartPosition = FormStartPosition.CenterScreen};
                binColFrm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 同步基础资料功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                //子线程调用
                new Thread(InsertK3SourceStart).Start();
                load.StartPosition = FormStartPosition.CenterScreen;
                load.ShowDialog();

                if (!task.ResultMark) throw new Exception("同步异常,请联系管理员");
                {
                    MessageBox.Show($"同步成功,请点击继续", $"提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        ///子线程使用(重:用于监视功能调用情况,当完成时进行关闭LoadForm)-同步基础资料使用
        /// </summary>
        private void InsertK3SourceStart()
        {
            //绑定
            task.InsertK3SourceRecord();

            //当完成后将Load子窗体关闭
            this.Invoke((ThreadStart)(() =>
            {
                load.Close();
            }));
        }

        #region 新物料导入生成

        /// <summary>
        /// 导入EXCEL数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmimportexcel_Click(object sender, EventArgs e)
        {
            try
            {
                var openFileDialog = new OpenFileDialog { Filter = $"Xlsx文件|*.xlsx" };
                if (openFileDialog.ShowDialog() != DialogResult.OK) return;

                task.FileAddress = openFileDialog.FileName;

                //子线程调用
                new Thread(ImportExcelStart).Start();
                load.StartPosition = FormStartPosition.CenterScreen;
                load.ShowDialog();
                
                if(task.ResultTable.Rows.Count==0)throw new Exception("导入异常,请联系管理员");
                {
                    MessageBox.Show($"导入成功,请点击继续",$"提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
                    //将‘导入EXCEL’数据按钮设置为不可用
                    tmimportexcel.Enabled = false;
                    //将记录赋值到GridView内
                    gvdtl.DataSource = task.ResultTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 生成K3新物料
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                if(gvdtl.RowCount==0) throw new Exception("检测到没有内容进行生成,请先进行导入Excel再继续");
                var clickMessage = $"准备生成,\n 注:此次生成的结果会记录在‘查询建档历史记录’内 \n 是否继续?";
                if (MessageBox.Show(clickMessage, $"提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    //通过gvdtl获取内容并转化至DataTable
                    task.ImportGridviewdt = (DataTable) gvdtl.DataSource;

                    //子线程调用
                    new Thread(GenerateRecordStart).Start();
                    load.StartPosition = FormStartPosition.CenterScreen;
                    load.ShowDialog();

                    //返回结果并执行相关提示
                    if(!task.ResultMark) throw new Exception("生成异常,请联系管理员");
                    {
                        //将返回结果DT传送至'生成结果'窗体,并进行显示
                        var showGenerateRecordFrm = new ShowGenerateRecordFrm { StartPosition = FormStartPosition.CenterScreen };
                        showGenerateRecordFrm.ShowDialog();

                        //当‘生成结果’窗体退出后,1)将gvdtl内容清空 2)将‘导入EXCEL’数据按钮设置为可用
                        var dt = (DataTable) gvdtl.DataSource;
                        dt.Rows.Clear();
                        dt.Columns.Clear();
                        gvdtl.DataSource = dt;

                        tmimportexcel.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                var dt = (DataTable)gvdtl.DataSource;
                dt.Rows.Clear();
                dt.Columns.Clear();
                gvdtl.DataSource = dt;
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 子线程使用(重:用于监视功能调用情况,当完成时进行关闭LoadForm)-导入EXCEL使用
        /// </summary>
        private void ImportExcelStart()
        {
            //导入EXCEL
            task.ImportExcelToDt();

            //当完成后将Load子窗体关闭
            this.Invoke((ThreadStart)(() =>
            {
                load.Close();
            }));
        }

        /// <summary>
        /// 子线程使用(重:用于监视功能调用情况,当完成时进行关闭LoadForm)-生成使用
        /// </summary>
        private void GenerateRecordStart()
        {
            //生成
            task.GenerateRecord();

            //当完成后将Load子窗体关闭
            this.Invoke((ThreadStart)(() =>
            {
                load.Close();
            }));
        }

        #endregion

        #region 查询历史记录

        /// <summary>
        /// 根据所选的选择条件刷新GridView
        /// </summary>
        private void OnSearchHistory()
        {
            //获取所选择的'开始'日期
            var sdt = dtstart.Value.ToString("yyyy-MM-dd");
            //获取所选择的'结束'日期
            var edt = dtend.Value.ToString("yyyy-MM-dd");
            //获取‘物料名称’记录
            var materialname = txtmaterialname.Text;
            //获取‘规格型号’记录
            var kui = txtkui.Text;
            //获取‘品牌’记录
            var bin = txtbin.Text;
            //获取下拉列表所选值
            var dvordertylelist = (DataRowView)comlist.Items[comlist.SelectedIndex];
            var typeId = Convert.ToInt32(dvordertylelist["Id"]);

            //将各变量值赋给task变量
            task.Sdt = sdt;
            task.Edt = edt;
            task.Fmaterialname = materialname;
            task.Fkui = kui;
            task.Fbi = bin;
            task.Finishid = typeId;

            //子线程调用
            new Thread(SearchHistoryStart).Start();
            load.StartPosition = FormStartPosition.CenterScreen;
            load.ShowDialog();

            if (task.ResultTable.Rows.Count > 0)
            {
                _dtl = task.ResultTable;
                panel5.Visible = true;
                //初始化下拉框所选择的默认值
                tmshowrows.SelectedItem = Convert.ToInt32(tmshowrows.SelectedItem) == 0
                    ? (object)"10"
                    : Convert.ToInt32(tmshowrows.SelectedItem);
                //定义初始化标记
                _pageChange = _pageCurrent <= 1;
                //GridView分页
                GridViewPageChange();
            }
            //注:当为空记录时,不显示跳转页;只需将临时表赋值至GridView内
            else
            {
                gvhistorydtl.DataSource = task.ResultTable;
                panel5.Visible = false;
            }
            //控制GridView单元格显示方式
            ControlHistoryGridViewisShow();
        }

        /// <summary>
        /// 查询历史记录功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                //根据所选的选择条件刷新GridView
                OnSearchHistory();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        ///子线程使用(重:用于监视功能调用情况,当完成时进行关闭LoadForm)-查询历史记录使用
        /// </summary>
        private void SearchHistoryStart()
        {
            //查询历史记录
            task.SearchHistoryRecord();

            //当完成后将Load子窗体关闭
            this.Invoke((ThreadStart)(() =>
            {
                load.Close();
            }));
        }

        /// <summary>
        /// 控制GridView单元格显示方式
        /// </summary>
        private void ControlHistoryGridViewisShow()
        {
            //注:当没有值时,若还设置某一行Row不显示的话,就会出现异常
            if (gvhistorydtl?.RowCount >= 0)
                gvhistorydtl.Columns[0].Visible = false;
        }

        /// <summary>
        /// 首页按钮(GridView页面跳转时使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BnMoveFirstItem_Click(object sender, EventArgs e)
        {
            try
            {
                //1)将当前页变量PageCurrent=1; 2)并将“首页” 及 “上一页”按钮设置为不可用 将“下一页” “末页”按设置为可用
                _pageCurrent = 1;
                bnMoveFirstItem.Enabled = false;
                bnMovePreviousItem.Enabled = false;

                bnMoveNextItem.Enabled = true;
                bnMoveLastItem.Enabled = true;
                GridViewPageChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 上一页(GridView页面跳转时使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BnMovePreviousItem_Click(object sender, EventArgs e)
        {
            try
            {
                //1)将PageCurrent自减 2)将“下一页” “末页”按钮设置为可用
                _pageCurrent--;
                bnMoveNextItem.Enabled = true;
                bnMoveLastItem.Enabled = true;
                //判断若PageCurrent=1的话,就将“首页” “上一页”按钮设置为不可用
                if (_pageCurrent == 1)
                {
                    bnMoveFirstItem.Enabled = false;
                    bnMovePreviousItem.Enabled = false;
                }
                GridViewPageChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 下一页按钮(GridView页面跳转时使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BnMoveNextItem_Click(object sender, EventArgs e)
        {
            try
            {
                //1)将PageCurrent自增 2)将“首页” “上一页”按钮设置为可用
                _pageCurrent++;
                bnMoveFirstItem.Enabled = true;
                bnMovePreviousItem.Enabled = true;
                //判断若PageCurrent与“总页数”一致的话,就将“下一页” “末页”按钮设置为不可用
                if (_pageCurrent == _totalpagecount)
                {
                    bnMoveNextItem.Enabled = false;
                    bnMoveLastItem.Enabled = false;
                }
                GridViewPageChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 末页按钮(GridView页面跳转使用)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BnMoveLastItem_Click(object sender, EventArgs e)
        {
            try
            {
                //1)将“总页数”赋值给PageCurrent 2)将“下一页” “末页”按钮设置为不可用 并将 “上一页” “首页”按钮设置为可用
                _pageCurrent = _totalpagecount;
                bnMoveNextItem.Enabled = false;
                bnMoveLastItem.Enabled = false;

                bnMovePreviousItem.Enabled = true;
                bnMoveFirstItem.Enabled = true;

                GridViewPageChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 跳转页文本框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BnPositionItem_Leave(object sender, EventArgs e)
        {
            try
            {
                //判断所输入的跳转数必须为整数
                if (!Regex.IsMatch(bnPositionItem.Text, @"^-?[1-9]\d*$|^0$")) throw new Exception("请输入整数再继续");
                //判断所输入的跳转数不能大于总页数
                if (Convert.ToInt32(bnPositionItem.Text) > _totalpagecount) throw new Exception("所输入的页数不能超出总页数,请修改后继续");
                //判断若所填跳转数为0时跳出异常
                if (Convert.ToInt32(bnPositionItem.Text) == 0) throw new Exception("请输入大于0的整数再继续");

                //将所填的跳转页赋值至“当前页”变量内
                _pageCurrent = Convert.ToInt32(bnPositionItem.Text);
                //根据所输入的页数动态控制四个方向键是否可用
                //若为第1页，就将“首页” “上一页”按钮设置为不可用 将“下一页” “末页”设置为可用
                if (_pageCurrent == 1)
                {
                    bnMoveFirstItem.Enabled = false;
                    bnMovePreviousItem.Enabled = false;

                    bnMoveNextItem.Enabled = true;
                    bnMoveLastItem.Enabled = true;
                }
                //若为末页,就将"下一页" “末页”按钮设置为不可用 将“上一页” “首页”设置为可用
                else if (_pageCurrent == _totalpagecount)
                {
                    bnMoveNextItem.Enabled = false;
                    bnMoveLastItem.Enabled = false;

                    bnMovePreviousItem.Enabled = true;
                    bnMoveFirstItem.Enabled = true;
                }
                //否则四个按钮都可用
                else
                {
                    bnMoveFirstItem.Enabled = true;
                    bnMovePreviousItem.Enabled = true;
                    bnMoveNextItem.Enabled = true;
                    bnMoveLastItem.Enabled = true;
                }
                GridViewPageChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                bnPositionItem.Text = Convert.ToString(_pageCurrent);
            }
        }

        /// <summary>
        /// 每页显示行数 下拉框关闭时执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmshowrows_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                //每次选择新的“每页显示行数”，都要 1)将_pageChange标记设为true(即执行初始化方法) 2)将“当前页”初始化为1
                _pageChange = true;
                _pageCurrent = 1;
                //将“上一页” “首页”设置为不可用
                bnMovePreviousItem.Enabled = false;
                bnMoveFirstItem.Enabled = false;
                GridViewPageChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// GridView分页功能
        /// </summary>
        private void GridViewPageChange()
        {
            try
            {
                //获取查询的总行数
                var dtltotalrows = _dtl.Rows.Count;
                //获取“每页显示行数”所选择的行数
                var pageCount = Convert.ToInt32(tmshowrows.SelectedItem);
                //计算出总页数
                _totalpagecount = dtltotalrows % pageCount == 0 ? dtltotalrows / pageCount : dtltotalrows / pageCount + 1;
                //赋值"总页数"项
                bnCountItem.Text = $"/ {_totalpagecount} 页";

                //初始化BindingNavigator控件内的各子控件 及 对应初始化信息
                if (_pageChange)
                {
                    bnPositionItem.Text = Convert.ToString(1);                       //初始化填充跳转页为1
                    tmshowrows.Enabled = true;                                      //每页显示行数（下拉框）  

                    //初始化时判断;若“总页数”=1，四个按钮不可用；若>1,“下一页” “末页”按钮可用
                    if (_totalpagecount == 1)
                    {
                        bnMoveFirstItem.Enabled = false;                            //'首页'按钮
                        bnMovePreviousItem.Enabled = false;                         //'上一页'按钮
                        bnMoveNextItem.Enabled = false;                             //'下一页'按钮
                        bnMoveLastItem.Enabled = false;                             //'末页'按钮
                        bnPositionItem.Enabled = false;                             //跳转页文本框
                    }
                    else
                    {
                        bnMoveNextItem.Enabled = true;
                        bnMoveLastItem.Enabled = true;
                        bnPositionItem.Enabled = true;                             //跳转页文本框
                    }
                    _pageChange = false;
                }

                //显示_dtl的查询总行数
                tstotalrow.Text = $"共 {_dtl.Rows.Count} 行";

                //根据“当前页” 及 “固定行数” 计算出新的行数记录并进行赋值
                //计算进行循环的起始行
                var startrow = (_pageCurrent - 1) * pageCount;
                //计算进行循环的结束行
                var endrow = _pageCurrent == _totalpagecount ? dtltotalrows : _pageCurrent * pageCount;
                //复制 查询的DT的列信息（不包括行）至临时表内
                var tempdt = _dtl.Clone();
                //循环将所需的_dtl的行记录复制至临时表内
                for (var i = startrow; i < endrow; i++)
                {
                    tempdt.ImportRow(_dtl.Rows[i]);
                }

                //最后将刷新的DT重新赋值给GridView
                gvhistorydtl.DataSource = tempdt;
                //将“当前页”赋值给"跳转页"文本框内
                bnPositionItem.Text = Convert.ToString(_pageCurrent);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion
    }
}
