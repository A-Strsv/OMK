using System;
//using System.Collections.Generic;
using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
using System.Xml;

using CommandsLib;

namespace NodeClasses
{
    public abstract class UniversalNode
    {
        public static String[] contextMenuStripItems;
        public static String[] addContextMenuStripItemDropDownItems;
        //public static System.Windows.Forms.TreeView treeView;
        protected String name;
        public static System.Windows.Forms.ContextMenuStrip GetContextMenuStrip(System.Windows.Forms.TreeNode treeNode)
        {
            Commands.contextMenuTreeNode = treeNode;
            UniversalNode universalNode = (UniversalNode)treeNode.Tag;
            Type nodeClass = universalNode.GetType();
            System.Reflection.FieldInfo fieldInfo = nodeClass.GetField("contextMenuStripItems");
            String[] contextMenuStripItems = (String[])fieldInfo.GetValue(universalNode);
            fieldInfo = nodeClass.GetField("addContextMenuStripItemDropDownItems");
            String[] addContextMenuStripItemDropDownItems = (String[])fieldInfo.GetValue(universalNode);
            System.Windows.Forms.ContextMenuStrip contextMenuStrip = new System.Windows.Forms.ContextMenuStrip();
            contextMenuStrip.Name = "contextMenuStrip";
            if (addContextMenuStripItemDropDownItems.Length > 0)
            {
                System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
                addToolStripMenuItem.Name = "addToolStripMenuItem";
                addToolStripMenuItem.Text = "Добавить";
                foreach (String command in addContextMenuStripItemDropDownItems)
                {
                    addToolStripMenuItem.DropDownItems.Add(Commands.toolStripItems[command]);
                }
                contextMenuStrip.Items.Add(addToolStripMenuItem);
            }
            foreach (String command in contextMenuStripItems)
            {
                contextMenuStrip.Items.Add(Commands.toolStripItems[command]);
            }
            return contextMenuStrip;
        }
        public static void LoadFromXmlRoot(XmlElement xmlRoot, System.Windows.Forms.TreeView treeView)
        {
            String nodeTypeName = "NodeClasses.Node" + xmlRoot.Name.ToUpper();
            Type nodeClass = Type.GetType(nodeTypeName, false, false);
            //System.Windows.Forms.MessageBox.Show("LoadFromXmlRoot findes class NodeClasses.Node" + xmlRoot.Name.ToUpper());
            if (nodeClass != null)
            {
                //System.Windows.Forms.MessageBox.Show("nodeClass = " + nodeClass.ToString());
                System.Windows.Forms.TreeNode treeNode = new System.Windows.Forms.TreeNode(xmlRoot.Name.ToUpper());
                treeView.Nodes.Add(treeNode);
                Type[] construcotPrametersTypes = new Type[] { typeof(XmlNode), typeof(System.Windows.Forms.TreeNode) };
                System.Reflection.ConstructorInfo constructorInfo = nodeClass.GetConstructor(construcotPrametersTypes);
                object[] constructorParameters = new object[] { xmlRoot, treeNode };
                UniversalNode newNode = (UniversalNode)constructorInfo.Invoke(constructorParameters);
                //UniversalNode newNode =
                //    (UniversalNode)nodeClass.GetConstructor(new Type[] { typeof(XmlNode), typeof(System.Windows.Forms.TreeNode) }).Invoke(new object[] { xmlRoot, treeNode });
                //String nodeName = (String)newNode.GetType().GetProperty("Name").GetValue(newNode);
            }
            else
                System.Windows.Forms.MessageBox.Show("Неизвестный тип узла: " + xmlRoot.Name.ToUpper());
        }
        public UniversalNode(XmlNode xmlNode, System.Windows.Forms.TreeNode treeNode)
        {
            foreach ( XmlNode iteratorNode in xmlNode.ChildNodes )
            {
                String nodeTypeName = "NodeClasses.Node" + iteratorNode.Name.ToUpper();
                Type nodeClass = Type.GetType(nodeTypeName, false, false);
                //System.Windows.Forms.MessageBox.Show("UniversalNode findes class NodeClasses.Node" + iteratorNode.Name.ToUpper());
                if (nodeClass != null)
                {
                    //System.Windows.Forms.MessageBox.Show("nodeClass = " + nodeClass.ToString());
                    System.Windows.Forms.TreeNode newTreeNode = new System.Windows.Forms.TreeNode(iteratorNode.Name.ToUpper());
                    treeNode.Nodes.Add(newTreeNode);
                    Type[] construcotPrametersTypes = new Type[] { typeof(XmlNode), typeof(System.Windows.Forms.TreeNode) };
                    System.Reflection.ConstructorInfo constructorInfo = nodeClass.GetConstructor(construcotPrametersTypes);
                    object[] constructorParameters = new object[] { iteratorNode, newTreeNode };
                    UniversalNode newNode = (UniversalNode)constructorInfo.Invoke(constructorParameters);
                }
                else
                    System.Windows.Forms.MessageBox.Show("Неизвестный тип узла: " + iteratorNode.Name.ToUpper());
            }
            treeNode.Tag = this;
        }
    }
    public class NodePROJECT : UniversalNode
    {
        public static String[] contextMenuStripItems;
        public static String[] addContextMenuStripItemDropDownItems;
        static NodePROJECT()
        {
            contextMenuStripItems = new String[]
            {
                "Run",
                "Rename", 
                "Close",
            };
            addContextMenuStripItemDropDownItems = new String[]
            {
                "AddConnectTable", 
                "AddGroup", 
                "AddProgram"
            };
        }
        public NodePROJECT(XmlNode xmlNode, System.Windows.Forms.TreeNode treeNode)
            : base(xmlNode, treeNode)
        {

        }
        [DisplayName("Название")]
        [Description("Название проекта")]
        [Category("Основные")]
        public String Name
        {
            get { return name; }
            set { name = value; }
        }
    }
    public class NodePROGRAM : UniversalNode
    {
        public static String[] contextMenuStripItems;
        public static String[] addContextMenuStripItemDropDownItems;
        static NodePROGRAM()
        {
            contextMenuStripItems = new String[]
            {
                "Run",
                "Rename",
            };
            addContextMenuStripItemDropDownItems = new String[]
            {
                "AddBlock",
                "AddVariable",
                "AddFor",
                "AddIf",
                "AddCommutator",
                "AddMeasure"
            };
        }
        public NodePROGRAM(XmlNode xmlNode, System.Windows.Forms.TreeNode treeNode)
            : base(xmlNode, treeNode)
        {

        }
        [DisplayName("Название")]
        [Description("Название программы")]
        [Category("Основные")]
        public String Name
        {
            get { return name; }
            set { name = value; }
        }
        [DisplayName("Примечание")]
        [Description("Примечание к программе")]
        [Category("Основные")]
        public String Comment
        {
            get { return "comment"; }
            set { }
        }
    }
}