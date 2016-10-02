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
        [Browsable(false)]
        public abstract String[] contextMenuStripItems
        {
            get;
        }
        [Browsable(false)]
        public abstract String[] addContextMenuStripItemDropDownItems
        {
            get;
        }
        //public static System.Windows.Forms.TreeView treeView;
        public System.Windows.Forms.TreeNode treeNode;
        public XmlNode xmlNode;
        [Browsable(false)]
        public XmlDocument xmlDocument
        {
            get
            {
                System.Windows.Forms.TreeView treeView = treeNode.TreeView;
                System.Windows.Forms.TabPage tabPage = (System.Windows.Forms.TabPage)treeView.Parent;
                OMK.TabPageSatellite tabPageSatellite = (OMK.TabPageSatellite)tabPage.Tag;
                return tabPageSatellite.xmlDocument;
            }
        }
        protected String name;
        public static System.Windows.Forms.ContextMenuStrip GetContextMenuStrip(System.Windows.Forms.TreeNode treeNode)
        {
            Commands.contextMenuTreeNode = treeNode;
            UniversalNode universalNode = (UniversalNode)treeNode.Tag;
            Type nodeClass = universalNode.GetType();
            System.Reflection.PropertyInfo propertyInfo = nodeClass.GetProperty("contextMenuStripItems");
            String[] contextMenuStripItems = (String[])propertyInfo.GetValue(universalNode);
            propertyInfo = nodeClass.GetProperty("addContextMenuStripItemDropDownItems");
            String[] addContextMenuStripItemDropDownItems = (String[])propertyInfo.GetValue(universalNode);
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
            String nodeClassName = "NodeClasses.Node" + xmlRoot.Name.ToUpper();
            Type nodeClass = Type.GetType(nodeClassName, false, false);
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
        /// <summary>
        /// Конструктор универсального узла, подгружает дочерние узлы
        /// </summary>
        /// <param name="xmlNode">Узел XML, из которого создается данный узел</param>
        /// <param name="treeNode">Узел дерева проекта, созданный для данного узла</param>
        public UniversalNode(XmlNode xmlNode, System.Windows.Forms.TreeNode treeNode)
        {
            this.treeNode = treeNode;
            treeNode.Tag = this;
            this.xmlNode = xmlNode;
            foreach (XmlNode iteratorNode in xmlNode.ChildNodes)
            {
                String nodeClassName = "NodeClasses.Node" + iteratorNode.Name.ToUpper();
                Type nodeClass = Type.GetType(nodeClassName, false, false);
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
        }
        /// <summary>
        /// Конструктор универсального узла по имени его типа
        /// </summary>
        /// <param name="parentNode">Родительский универсальный узел</param>
        /// <param name="type">Тип создаваемого узла</param>
        public UniversalNode(UniversalNode parentNode, String type)
        {
            treeNode = new System.Windows.Forms.TreeNode(type.ToUpper());
            parentNode.treeNode.Nodes.Add(treeNode);
            xmlNode = parentNode.xmlDocument.CreateElement(type.ToUpper());
            parentNode.xmlNode.AppendChild(xmlNode);
        }
        /// <summary>
        /// Добавление универсального узла по имени его типа
        /// </summary>
        /// <param name="parentNode">Родительский узел дерева проекта</param>
        /// <param name="type">Тип добавляемого узла</param>
        public static void AddNode(System.Windows.Forms.TreeNode parentNode, String type)
        {
            String nodeClassName = "NodeClasses.Node" + type.ToUpper();
            Type nodeClass = Type.GetType(nodeClassName, false, false);
            //System.Windows.Forms.MessageBox.Show("UniversalNode findes class NodeClasses.Node" + iteratorNode.Name.ToUpper());
            if (nodeClass != null)
            {
                //System.Windows.Forms.MessageBox.Show("nodeClass = " + nodeClass.ToString());
                Type[] construcotPrametersTypes = new Type[] { typeof(UniversalNode), typeof(String) };
                System.Reflection.ConstructorInfo constructorInfo = nodeClass.GetConstructor(construcotPrametersTypes);
                object[] constructorParameters = new object[] { parentNode.Tag, type };
                UniversalNode newNode = (UniversalNode)constructorInfo.Invoke(constructorParameters);
            }
            else
                System.Windows.Forms.MessageBox.Show("Неизвестный тип узла: " + type.ToUpper());

        }
    }
    public class NodePROJECT : UniversalNode
    {
        public override String[] contextMenuStripItems
        {
            get
            {
                return new String[]
                {
                    "Run",
                    "Rename",
                    "Close"
                };
            }
        }
        public override String[] addContextMenuStripItemDropDownItems
        {
            get
            {
                return new String[]
                {
                    "AddConnectTable",
                    "AddGroup",
                    "AddProgram"
                };
            }
        }
        static NodePROJECT()
        {
        }
        public NodePROJECT(XmlNode xmlNode, System.Windows.Forms.TreeNode treeNode)
            : base(xmlNode, treeNode)
        {
        }
        public NodePROJECT(UniversalNode parentNode, String type)
            : base(parentNode, type)
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
        public override String[] contextMenuStripItems
        {
            get
            {
                return new String[]
                {
                    "Run",
                    "Rename"
                };
            }
        }
        public override String[] addContextMenuStripItemDropDownItems
        {
            get
            {
                return new String[]
                {
                    "AddBlock",
                    "AddVariable",
                    "AddFor",
                    "AddIf",
                    "AddCommutator",
                    "AddMeasure"
                };
            }
        }
        static NodePROGRAM()
        {
        }
        public NodePROGRAM(XmlNode xmlNode, System.Windows.Forms.TreeNode treeNode)
            : base(xmlNode, treeNode)
        {
        }
        public NodePROGRAM(UniversalNode parentNode, String type)
            : base(parentNode, type)
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