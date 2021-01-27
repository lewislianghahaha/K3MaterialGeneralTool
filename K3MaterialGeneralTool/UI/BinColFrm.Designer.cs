namespace K3MaterialGeneralTool.UI
{
    partial class BinColFrm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Menu = new System.Windows.Forms.MenuStrip();
            this.tmBind = new System.Windows.Forms.ToolStripMenuItem();
            this.tmremovebind = new System.Windows.Forms.ToolStripMenuItem();
            this.tmCreateExcelCol = new System.Windows.Forms.ToolStripMenuItem();
            this.tmclose = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.gvk3bind = new System.Windows.Forms.DataGridView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.comtypelist = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.gvbind = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.gvexcelBind = new System.Windows.Forms.DataGridView();
            this.Menu.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvk3bind)).BeginInit();
            this.panel3.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvbind)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvexcelBind)).BeginInit();
            this.SuspendLayout();
            // 
            // Menu
            // 
            this.Menu.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.Menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmBind,
            this.tmremovebind,
            this.tmCreateExcelCol,
            this.tmclose});
            this.Menu.Location = new System.Drawing.Point(0, 0);
            this.Menu.Name = "Menu";
            this.Menu.Size = new System.Drawing.Size(1046, 25);
            this.Menu.TabIndex = 0;
            // 
            // tmBind
            // 
            this.tmBind.Name = "tmBind";
            this.tmBind.Size = new System.Drawing.Size(44, 21);
            this.tmBind.Text = "绑定";
            // 
            // tmremovebind
            // 
            this.tmremovebind.Name = "tmremovebind";
            this.tmremovebind.Size = new System.Drawing.Size(68, 21);
            this.tmremovebind.Text = "清除绑定";
            // 
            // tmCreateExcelCol
            // 
            this.tmCreateExcelCol.Name = "tmCreateExcelCol";
            this.tmCreateExcelCol.Size = new System.Drawing.Size(97, 21);
            this.tmCreateExcelCol.Text = "新增Excel字段";
            // 
            // tmclose
            // 
            this.tmclose.Name = "tmclose";
            this.tmclose.Size = new System.Drawing.Size(44, 21);
            this.tmclose.Text = "关闭";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1046, 581);
            this.panel1.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(320, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(319, 579);
            this.panel2.TabIndex = 3;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panel4);
            this.groupBox2.Controls.Add(this.panel3);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(319, 579);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "K3绑定字段";
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.gvk3bind);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(3, 43);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(313, 533);
            this.panel4.TabIndex = 1;
            // 
            // gvk3bind
            // 
            this.gvk3bind.AllowUserToAddRows = false;
            this.gvk3bind.AllowUserToDeleteRows = false;
            this.gvk3bind.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvk3bind.Location = new System.Drawing.Point(0, 0);
            this.gvk3bind.Name = "gvk3bind";
            this.gvk3bind.ReadOnly = true;
            this.gvk3bind.RowTemplate.Height = 23;
            this.gvk3bind.Size = new System.Drawing.Size(311, 531);
            this.gvk3bind.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.comtypelist);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(3, 17);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(313, 26);
            this.panel3.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "类型:";
            // 
            // comtypelist
            // 
            this.comtypelist.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comtypelist.FormattingEnabled = true;
            this.comtypelist.Location = new System.Drawing.Point(51, 3);
            this.comtypelist.Name = "comtypelist";
            this.comtypelist.Size = new System.Drawing.Size(169, 20);
            this.comtypelist.TabIndex = 2;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.gvbind);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupBox3.Location = new System.Drawing.Point(639, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(405, 579);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "绑定结果";
            // 
            // gvbind
            // 
            this.gvbind.AllowUserToAddRows = false;
            this.gvbind.AllowUserToDeleteRows = false;
            this.gvbind.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvbind.Location = new System.Drawing.Point(3, 17);
            this.gvbind.Name = "gvbind";
            this.gvbind.ReadOnly = true;
            this.gvbind.RowTemplate.Height = 23;
            this.gvbind.Size = new System.Drawing.Size(399, 559);
            this.gvbind.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.gvexcelBind);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(320, 579);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Excel绑定字段";
            // 
            // gvexcelBind
            // 
            this.gvexcelBind.AllowUserToAddRows = false;
            this.gvexcelBind.AllowUserToDeleteRows = false;
            this.gvexcelBind.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvexcelBind.Location = new System.Drawing.Point(3, 17);
            this.gvexcelBind.Name = "gvexcelBind";
            this.gvexcelBind.ReadOnly = true;
            this.gvexcelBind.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.gvexcelBind.RowTemplate.Height = 23;
            this.gvexcelBind.Size = new System.Drawing.Size(314, 559);
            this.gvexcelBind.TabIndex = 0;
            // 
            // BinColFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1046, 606);
            this.ControlBox = false;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.Menu);
            this.MainMenuStrip = this.Menu;
            this.Name = "BinColFrm";
            this.Text = "绑定字段";
            this.Menu.ResumeLayout(false);
            this.Menu.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvk3bind)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvbind)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvexcelBind)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip Menu;
        private System.Windows.Forms.ToolStripMenuItem tmBind;
        private System.Windows.Forms.ToolStripMenuItem tmremovebind;
        private System.Windows.Forms.ToolStripMenuItem tmCreateExcelCol;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView gvexcelBind;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DataGridView gvbind;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comtypelist;
        private System.Windows.Forms.DataGridView gvk3bind;
        private System.Windows.Forms.ToolStripMenuItem tmclose;
    }
}