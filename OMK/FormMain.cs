using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Xml;
using NodeClasses;
using TestBenchClass;

namespace OMK
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }
        TestBench testBench;
        private void FormMain_Load(object sender, EventArgs e)
        {
            testBench = new TestBench();
            testBench.Write = Write;
            testBench.WriteSerialLog = WriteSerialLog;
            CommandsLib.Commands.formMain = this;
        }
        private void treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                ContextMenuStrip = UniversalNode.GetContextMenuStrip(e.Node);
                //String S = "";
                //foreach (ToolStripItem item in ContextMenuStrip.Items)
                //    S += item.Text + "\n";
                //MessageBox.Show(S);
                ContextMenuStrip.Show((Control)sender, e.Location);
            }

        }
        private System.Windows.Forms.TabPage CreateTab()
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
            tabControl.SelectedTab = tabPage;
            treeView.NodeMouseClick += treeView_NodeMouseClick;
            treeView.BeforeSelect += treeView_BeforeSelect;
            return tabPage;
        }
        delegate void void_StrColDelegate(string S, Color Color);
        public void Write(string S, Color Color)
        {
            if (InvokeRequired)
                Invoke(new void_StrColDelegate(Write), new object[] { S, Color });
            else
            {
                richTextBoxLog.SelectionStart = richTextBoxLog.TextLength;
                richTextBoxLog.SelectionColor = Color;
                System.Drawing.Font OldFont = richTextBoxLog.SelectionFont;
                System.Drawing.Font NewFont = new System.Drawing.Font(OldFont, FontStyle.Bold);
                richTextBoxLog.SelectionFont = NewFont;
                richTextBoxLog.AppendText(S);
                richTextBoxLog.SelectionFont = OldFont;
                richTextBoxLog.ScrollToCaret();
            }
        }
        delegate void void_StrStrColDelegate(string s1, string s2, Color Color);
        public void WriteSerialLog(string s1, string s2, Color color)
        {
            if (InvokeRequired)
                Invoke(new void_StrStrColDelegate(WriteSerialLog), new object[] { s1, s2, color });
            else
            {
                richTextBoxLog.SelectionStart = richTextBoxLog.TextLength;
                richTextBoxLog.SelectionColor = color;
                System.Drawing.Font OldFont = richTextBoxLog.SelectionFont;
                System.Drawing.Font NewFont = new System.Drawing.Font(OldFont, FontStyle.Bold);
                richTextBoxLog.SelectionFont = NewFont;
                richTextBoxLog.AppendText(s1);
                richTextBoxLog.SelectionFont = OldFont;
                richTextBoxLog.AppendText(s2);
                richTextBoxLog.ScrollToCaret();
            }
        }

        void treeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            System.Windows.Forms.PropertyGrid propertyGrid = (System.Windows.Forms.PropertyGrid)e.Node.TreeView.Parent.Controls.Find("propertyGrid", false)[0];
            propertyGrid.SelectedObject = e.Node.Tag;
        }
        private void создатьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Windows.Forms.TabPage tabPage = CreateTab();
                TabPageSatellite tabPageSatellite = new TabPageSatellite(saveFileDialog.FileName, TabPageSatellite.ConstructorMode.New, tabPage);
            }
         }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Windows.Forms.TabPage tabPage = CreateTab();
                TabPageSatellite tabPageSatellite = new TabPageSatellite(openFileDialog.FileName, TabPageSatellite.ConstructorMode.Open, tabPage);
            }
        }

        private void tESTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*dmm.Measurement.MeasurementFunction = Agilent34410MeasurementFunctionEnum.Agilent34410MeasurementFunctionContinuity;
            System.Threading.Thread.Sleep(100);
            dmm.Measurement.Read();
            System.Threading.Thread.Sleep(100);
            dmm.Measurement.Read();
            System.Threading.Thread.Sleep(100);
            dmm.Measurement.Read();
            System.Threading.Thread.Sleep(100);
            dmm.Measurement.Read();
            System.Threading.Thread.Sleep(100);
            dmm.Measurement.Read();
            System.Threading.Thread.Sleep(100);
            //MessageBox.Show(dmm.Voltage.DCVoltage.Measure(10, Agilent34410ResolutionEnum.Agilent34410ResolutionDefault).ToString());
            dmm.Display.ClearDisplay(1);
            dmm.Display.DisplayText[1] = "   MAWA   ";
            dmm.Display.DisplayText[2] = " UBETO4EK ";
            dmm.Close();*/
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPageSatellite tabPageSatellite = (TabPageSatellite)tabControl.SelectedTab.Tag;
            tabPageSatellite.xmlDocument.Save(tabPageSatellite.fileName);
        }
        public TextBox protocolWriteTextBox;
        public void ProtocolWrite(string S, Color Color)
        {
            if (InvokeRequired)
                Invoke(new void_StrColDelegate(ProtocolWrite), new object[] { S, Color });
            else
            {
                protocolWriteTextBox.AppendText(S);
                protocolWriteTextBox.ScrollToCaret();
            }
        }

    }
    public class TabPageSatellite : Control
    {
        public Boolean modified;
        public String fileName;
        public XmlDocument xmlDocument;
        XmlElement xmlRoot;
        public System.Windows.Forms.TabPage tabPage;
        public System.Windows.Forms.TreeView treeView;
        public System.Windows.Forms.TextBox textBox;
        public enum ConstructorMode { New, Open, OpenAndTranslate}
        public TabPageSatellite(String fileName, ConstructorMode constructorMode, System.Windows.Forms.TabPage tabPage)
        {
            tabPage.Tag = this;
            this.tabPage = tabPage;
            Control.ControlCollection controlCollection = (Control.ControlCollection)typeof(System.Windows.Forms.TabPage).GetProperty("Controls").GetValue(tabPage);
            treeView = (System.Windows.Forms.TreeView)controlCollection.OfType<System.Windows.Forms.TreeView>().First<System.Windows.Forms.TreeView>();
            textBox = (System.Windows.Forms.TextBox)controlCollection.OfType<System.Windows.Forms.TextBox>().First<System.Windows.Forms.TextBox>();
            this.fileName = fileName;
            switch (constructorMode)
            {
                case ConstructorMode.New:
                    XmlTextWriter textWriter = new XmlTextWriter(fileName, Encoding.UTF8);
                    textWriter.WriteStartDocument();
                    textWriter.WriteStartElement("Project");
                    textWriter.WriteEndElement();
                    textWriter.Close();
                    xmlDocument = new XmlDocument();
                    xmlDocument.Load(fileName);
                    xmlRoot = xmlDocument.DocumentElement;
                    {
                        XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("name");
                        xmlAttribute.Value = "Проект";
                        xmlRoot.Attributes.Append(xmlAttribute);
                    }
                    UniversalNode.LoadFromXmlRoot(xmlRoot, treeView);
                    break;
                case ConstructorMode.Open:
                    xmlDocument = new XmlDocument();
                    xmlDocument.Load(fileName);
                    xmlRoot = xmlDocument.DocumentElement;
                    {
                        XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("name");
                        xmlAttribute.Value = "Проект";
                        xmlRoot.Attributes.Append(xmlAttribute);
                    }
                    UniversalNode.LoadFromXmlRoot(xmlRoot, treeView);
                    break;
                case ConstructorMode.OpenAndTranslate:
                    break;
            }
        }

    }
}
