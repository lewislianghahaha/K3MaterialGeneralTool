using System;
using System.Windows.Forms;

namespace K3MaterialGeneralTool.UI
{
    public partial class Main : Form
    {
        BinColFrm binColFrm=new BinColFrm();

        public Main()
        {
            InitializeComponent();
            OnInitialize();
            OnRegisterEvents();
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
        /// 绑定功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TmBindCol_Click(object sender, EventArgs e)
        {
            try
            {
                binColFrm.StartPosition=FormStartPosition.CenterScreen;
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

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region  新物料导入生成

        /// <summary>
        /// 导入EXCEL数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tmimportexcel_Click(object sender, EventArgs e)
        {
            try
            {

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

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region 查询历史记录

        /// <summary>
        /// 查询历名记录功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, $"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        #endregion
    }
}
