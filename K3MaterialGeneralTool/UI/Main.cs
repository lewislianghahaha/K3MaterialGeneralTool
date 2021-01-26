using System.Windows.Forms;

namespace K3MaterialGeneralTool.UI
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            OnInitialize();
            OnRegisterEvents();
        }

        /// <summary>
        /// 初始化相关设置
        /// </summary>
        private void OnInitialize()
        {
            
        }

        private void OnRegisterEvents()
        {

            //设置TabControl控件默认显示页
            tbhistory.SelectedIndex = 1;
        }


    }
}
