using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OMK
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
        }
        private void CreateTab()
        {
            System.Windows.Forms.TabPage tabPage;
            System.Windows.Forms.TreeView treeView;
            System.Windows.Forms.Splitter splitter1;
            System.Windows.Forms.PropertyGrid propertyGrid;
            System.Windows.Forms.Splitter splitter2;
            System.Windows.Forms.TextBox textBox;
            tabPage = new System.Windows.Forms.TabPage();
            treeView = new System.Windows.Forms.TreeView();
            splitter1 = new System.Windows.Forms.Splitter();
            propertyGrid = new System.Windows.Forms.PropertyGrid();
            splitter2 = new System.Windows.Forms.Splitter();
            textBox = new System.Windows.Forms.TextBox();
            tabControl.Controls.Add(tabPage);
            tabPage.SuspendLayout();
            tabPage.Controls.Add(textBox);
            tabPage.Controls.Add(splitter2);
            tabPage.Controls.Add(propertyGrid);
            tabPage.Controls.Add(splitter1);
            tabPage.Controls.Add(treeView);
            tabPage.Name = "tabPage";
            tabPage.Padding = new System.Windows.Forms.Padding(3);
            tabPage.TabIndex = 0;
            tabPage.Text = "tabPage";
            tabPage.UseVisualStyleBackColor = true;
            treeView.Dock = System.Windows.Forms.DockStyle.Left;
            treeView.Name = "treeView";
            treeView.Width = 245;
            treeView.TabIndex = 0;
            splitter1.Name = "splitter1";
            splitter1.TabIndex = 1;
            splitter1.TabStop = false;
            propertyGrid.Dock = System.Windows.Forms.DockStyle.Left;
            propertyGrid.Name = "propertyGrid";
            propertyGrid.Width = 266;
            propertyGrid.TabIndex = 2;
            splitter2.Name = "splitter2";
            splitter2.TabIndex = 3;
            splitter2.TabStop = false;
            textBox.Dock = System.Windows.Forms.DockStyle.Fill;
            textBox.Multiline = true;
            textBox.Name = "textBox";
            textBox.TabIndex = 4;
            textBox.Text = "Text\r\nText\r\nText";
            tabPage.ResumeLayout(false);
            tabPage.PerformLayout();
        }
        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateTab();
        }
    }
}
