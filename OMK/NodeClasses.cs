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
        public String GetStringXmlAttribute(String attributeName)
        {
            XmlAttribute xmlAttribute = (XmlAttribute)xmlNode.Attributes.GetNamedItem(attributeName);
            if (xmlAttribute != null)
                return xmlAttribute.Value;
            else
                return "";
        }
        public void SetStringXmlAttribute(String attributeName, String Value)
        {
            XmlAttribute xmlAttribute = (XmlAttribute)xmlNode.Attributes.GetNamedItem(attributeName);
            if (xmlAttribute == null)
            {
                xmlAttribute = xmlDocument.CreateAttribute(attributeName);
                xmlNode.Attributes.Append(xmlAttribute);
            }
            xmlAttribute.Value = Value;
        }
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
            treeNode.Tag = this;
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
                    "AddDialog",
                    "AddMeasure",
                    "AddCommutator",
                    "AddRK104Close",
                    "AddStop",
                    "AddPause",
                    "AddWrite",
                    "AddRK104Reset",
                    "AddRK104Open",
                    "AddMessage",
                    "AddIf",
                    "AddFor"
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
    public class NodeFOR : UniversalNode
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
                    "AddDialog",
                    "AddMeasure",
                    "AddCommutator",
                    "AddRK104Close",
                    "AddStop",
                    "AddPause",
                    "AddWrite",
                    "AddRK104Reset",
                    "AddRK104Open",
                    "AddMessage",
                    "AddIf",
                    "AddFor"
                };
            }
        }
        static NodeFOR()
        {
        }
        public NodeFOR(XmlNode xmlNode, System.Windows.Forms.TreeNode treeNode)
            : base(xmlNode, treeNode)
        {
        }
        public NodeFOR(UniversalNode parentNode, String type)
            : base(parentNode, type)
        {
        }
        [DisplayName("Название")]
        [Description("Название цикла")]
        [Category("Основные")]
        public String Name
        {
            get { return GetStringXmlAttribute("name"); }
            set { SetStringXmlAttribute("name", value); }
        }
        [DisplayName("Примечание")]
        [Description("Примечание к циклу")]
        [Category("Основные")]
        public String Comment
        {
            get { return GetStringXmlAttribute("comment"); }
            set { SetStringXmlAttribute("comment", value); }
        }
        [DisplayName("Идентификатор")]
        [Description("Идентификатор итератора цикла")]
        [Category("Основные")]
        public String Identifier
        {
            get { return GetStringXmlAttribute("identifier"); }
            set { SetStringXmlAttribute("identifier", value); }
        }
        [DisplayName("Инициализатор")]
        [Description("Выражения для присвоения начального значения итератору")]
        [Category("Основные")]
        public String Initializer
        {
            get { return GetStringXmlAttribute("initializer"); }
            set { SetStringXmlAttribute("initializer", value); }
        }
        [DisplayName("Окончание")]
        [Description("Выражение условия авершения цикла, цикл завершается при превышении итератором данного значения")]
        [Category("Основные")]
        public String End
        {
            get { return GetStringXmlAttribute("end"); }
            set { SetStringXmlAttribute("end", value); }
        }
        [DisplayName("Шаг")]
        [Description("Выражение, значение которого прибавляется к итератору после каждого прохода цикла")]
        [Category("Основные")]
        public String Step
        {
            get { return GetStringXmlAttribute("step"); }
            set { SetStringXmlAttribute("step", value); }
        }
    }
    public class NodeIF : UniversalNode
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
                    "AddDialog",
                    "AddMeasure",
                    "AddCommutator",
                    "AddRK104Close",
                    "AddStop",
                    "AddPause",
                    "AddWrite",
                    "AddRK104Reset",
                    "AddRK104Open",
                    "AddMessage",
                    "AddIf",
                    "AddFor"
                };
            }
        }
        static NodeIF()
        {
        }
        public NodeIF(XmlNode xmlNode, System.Windows.Forms.TreeNode treeNode)
            : base(xmlNode, treeNode)
        {
        }
        public NodeIF(UniversalNode parentNode, String type)
            : base(parentNode, type)
        {
        }
        [DisplayName("Название")]
        [Description("Название условия")]
        [Category("Основные")]
        public String Name
        {
            get { return GetStringXmlAttribute("name"); }
            set { SetStringXmlAttribute("name", value); }
        }
        [DisplayName("Примечание")]
        [Description("Примечание к условию")]
        [Category("Основные")]
        public String Comment
        {
            get { return GetStringXmlAttribute("comment"); }
            set { SetStringXmlAttribute("comment", value); }
        }
        [DisplayName("Условие")]
        [Description("Выражение условия. Оператор выполняется если вычисленное значение отлично от нуля")]
        [Category("Основные")]
        public String Expression
        {
            get { return GetStringXmlAttribute("expression"); }
            set { SetStringXmlAttribute("expression", value); }
        }
    }
    public class NodeVARIABLE : UniversalNode
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
                };
            }
        }
        static NodeVARIABLE()
        {
        }
        public NodeVARIABLE(XmlNode xmlNode, System.Windows.Forms.TreeNode treeNode)
            : base(xmlNode, treeNode)
        {
        }
        public NodeVARIABLE(UniversalNode parentNode, String type)
            : base(parentNode, type)
        {
        }
        [DisplayName("Название")]
        [Description("Название переменной")]
        [Category("Основные")]
        public String Name
        {
            get { return GetStringXmlAttribute("name"); }
            set { SetStringXmlAttribute("name", value); }
        }
        [DisplayName("Примечание")]
        [Description("Примечание к переменной")]
        [Category("Основные")]
        public String Comment
        {
            get { return GetStringXmlAttribute("comment"); }
            set { SetStringXmlAttribute("comment", value); }
        }
        [DisplayName("Идентификатор")]
        [Description("Идентификатор переменной")]
        [Category("Основные")]
        public String Identifier
        {
            get { return GetStringXmlAttribute("identifier"); }
            set { SetStringXmlAttribute("identifier", value); }
        }
        [DisplayName("Инициализатор")]
        [Description("Выражения для присвоения начального значения переменной")]
        [Category("Основные")]
        public String Initializer
        {
            get { return GetStringXmlAttribute("initializer"); }
            set { SetStringXmlAttribute("initializer", value); }
        }
    }
    public class NodePAUSE : UniversalNode
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
                };
            }
        }
        static NodePAUSE()
        {
        }
        public NodePAUSE(XmlNode xmlNode, System.Windows.Forms.TreeNode treeNode)
            : base(xmlNode, treeNode)
        {
        }
        public NodePAUSE(UniversalNode parentNode, String type)
            : base(parentNode, type)
        {
        }
        [DisplayName("Название")]
        [Description("Название задержки")]
        [Category("Основные")]
        public String Name
        {
            get { return GetStringXmlAttribute("name"); }
            set { SetStringXmlAttribute("name", value); }
        }
        [DisplayName("Примечание")]
        [Description("Примечание к задержке")]
        [Category("Основные")]
        public String Comment
        {
            get { return GetStringXmlAttribute("comment"); }
            set { SetStringXmlAttribute("comment", value); }
        }
        [DisplayName("Задержка")]
        [Description("Значение выражения задает величину задержки в мс (по умолчанию 1000)")]
        [Category("Основные")]
        public String Interval
        {
            get { return GetStringXmlAttribute("interval"); }
            set { SetStringXmlAttribute("interval", value); }
        }
    }
    public class NodeMESSAGE : UniversalNode
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
                };
            }
        }
        static NodeMESSAGE()
        {
        }
        public NodeMESSAGE(XmlNode xmlNode, System.Windows.Forms.TreeNode treeNode)
            : base(xmlNode, treeNode)
        {
        }
        public NodeMESSAGE(UniversalNode parentNode, String type)
            : base(parentNode, type)
        {
        }
        [DisplayName("Название")]
        [Description("Название сообщения")]
        [Category("Основные")]
        public String Name
        {
            get { return GetStringXmlAttribute("name"); }
            set { SetStringXmlAttribute("name", value); }
        }
        [DisplayName("Примечание")]
        [Description("Примечание к сообщению")]
        [Category("Основные")]
        public String Comment
        {
            get { return GetStringXmlAttribute("comment"); }
            set { SetStringXmlAttribute("comment", value); }
        }
        [DisplayName("Текст")]
        [Description("Текст сообщения")]
        [Category("Основные")]
        public String Text
        {
            get { return GetStringXmlAttribute("text"); }
            set { SetStringXmlAttribute("text", value); }
        }
    }
    public class NodeSTOP : UniversalNode
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
                };
            }
        }
        static NodeSTOP()
        {
        }
        public NodeSTOP(XmlNode xmlNode, System.Windows.Forms.TreeNode treeNode)
            : base(xmlNode, treeNode)
        {
        }
        public NodeSTOP(UniversalNode parentNode, String type)
            : base(parentNode, type)
        {
        }
        [DisplayName("Название")]
        [Description("Название останова")]
        [Category("Основные")]
        public String Name
        {
            get { return GetStringXmlAttribute("name"); }
            set { SetStringXmlAttribute("name", value); }
        }
        [DisplayName("Примечание")]
        [Description("Примечание к останову")]
        [Category("Основные")]
        public String Comment
        {
            get { return GetStringXmlAttribute("comment"); }
            set { SetStringXmlAttribute("comment", value); }
        }
    }
    public class NodeCOMMUTATOR : UniversalNode
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
                };
            }
        }
        static NodeCOMMUTATOR()
        {
        }
        public NodeCOMMUTATOR(XmlNode xmlNode, System.Windows.Forms.TreeNode treeNode)
            : base(xmlNode, treeNode)
        {
        }
        public NodeCOMMUTATOR(UniversalNode parentNode, String type)
            : base(parentNode, type)
        {
        }
        [DisplayName("Название")]
        [Description("Название команды коммутатора")]
        [Category("Основные")]
        public String Name
        {
            get { return GetStringXmlAttribute("name"); }
            set { SetStringXmlAttribute("name", value); }
        }
        [DisplayName("Примечание")]
        [Description("Примечание к команде коммутатора")]
        [Category("Основные")]
        public String Comment
        {
            get { return GetStringXmlAttribute("comment"); }
            set { SetStringXmlAttribute("comment", value); }
        }
        [DisplayName("Разъём")]
        [Description("Номер машинного разъёма")]
        [Category("Основные")]
        public String Addr
        {
            get { return GetStringXmlAttribute("addr"); }
            set { SetStringXmlAttribute("addr", value); }
        }
        [DisplayName("Точка")]
        [Description("Номер точки")]
        [Category("Основные")]
        public String Point
        {
            get { return GetStringXmlAttribute("point"); }
            set { SetStringXmlAttribute("point", value); }
        }
        [DisplayName("Шины")]
        [Description("Сумма кодов шин (A - 1, B - 2, C - 4, D - 8)")]
        [Category("Основные")]
        public String Buses
        {
            get { return GetStringXmlAttribute("buses"); }
            set { SetStringXmlAttribute("buses", value); }
        }
        [DisplayName("Действие")]
        [Description("Замкнуть - CLOSE, разомкнуть - OPEN")]
        [Category("Основные")]
        public String State
        {
            get { return GetStringXmlAttribute("state"); }
            set { SetStringXmlAttribute("state", value); }
        }
    }
    public class NodeMEASURE : UniversalNode
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
                };
            }
        }
        static NodeMEASURE()
        {
        }
        public NodeMEASURE(XmlNode xmlNode, System.Windows.Forms.TreeNode treeNode)
            : base(xmlNode, treeNode)
        {
        }
        public NodeMEASURE(UniversalNode parentNode, String type)
            : base(parentNode, type)
        {
        }
        [DisplayName("Название")]
        [Description("Название измерения")]
        [Category("Основные")]
        public String Name
        {
            get { return GetStringXmlAttribute("name"); }
            set { SetStringXmlAttribute("name", value); }
        }
        [DisplayName("Примечание")]
        [Description("Примечание к измерению")]
        [Category("Основные")]
        public String Comment
        {
            get { return GetStringXmlAttribute("comment"); }
            set { SetStringXmlAttribute("comment", value); }
        }
        [DisplayName("Функция")]
        [Description("Функция измерения (VOLTAGE, CURRENT, RESISTANCE, RESISTANCE4WIRE, CAPACITANCE, FREQUENCY, PERIOD, TEMPERATURE, DIODE, WITHSTANDING)")]
        [Category("Основные")]
        public String Function
        {
            get { return GetStringXmlAttribute("function"); }
            set { SetStringXmlAttribute("function", value); }
        }
        [DisplayName("Род тока")]
        [Description("Переменный - AC, постоянный - DC")]
        [Category("Основные")]
        public String CurrentType
        {
            get { return GetStringXmlAttribute("currenttype"); }
            set { SetStringXmlAttribute("currenttype", value); }
        }
        [DisplayName("Измерительный ток")]
        [Description("Задание тока для измерения сопротивления по 4-х проводной схеме")]
        [Category("Основные")]
        public String MeasurementCurrent
        {
            get { return GetStringXmlAttribute("measurementcurrent"); }
            set { SetStringXmlAttribute("measurementcurrent", value); }
        }
        [DisplayName("Диапазон")]
        [Description("Диапазон измеряемой величины")]
        [Category("Основные")]
        public String Range
        {
            get { return GetStringXmlAttribute("range"); }
            set { SetStringXmlAttribute("range", value); }
        }
    }
    public class NodeWRITE : UniversalNode
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
                };
            }
        }
        static NodeWRITE()
        {
        }
        public NodeWRITE(XmlNode xmlNode, System.Windows.Forms.TreeNode treeNode)
            : base(xmlNode, treeNode)
        {
        }
        public NodeWRITE(UniversalNode parentNode, String type)
            : base(parentNode, type)
        {
        }
        [DisplayName("Название")]
        [Description("Название печати")]
        [Category("Основные")]
        public String Name
        {
            get { return GetStringXmlAttribute("name"); }
            set { SetStringXmlAttribute("name", value); }
        }
        [DisplayName("Примечание")]
        [Description("Примечание к печати")]
        [Category("Основные")]
        public String Comment
        {
            get { return GetStringXmlAttribute("comment"); }
            set { SetStringXmlAttribute("comment", value); }
        }
        [DisplayName("Текст до")]
        [Description("Текст до выражения")]
        [Category("Основные")]
        public String TextBefore
        {
            get { return GetStringXmlAttribute("textbefore"); }
            set { SetStringXmlAttribute("textbefore", value); }
        }
        [DisplayName("Текст после")]
        [Description("Текст после выражения")]
        [Category("Основные")]
        public String TextAfter
        {
            get { return GetStringXmlAttribute("textafter"); }
            set { SetStringXmlAttribute("textafter", value); }
        }
        [DisplayName("Выражение")]
        [Description("Выражение, значение которого будет выведено в протокол")]
        [Category("Основные")]
        public String Expression
        {
            get { return GetStringXmlAttribute("expression"); }
            set { SetStringXmlAttribute("expression", value); }
        }
        [DisplayName("Перевод строки")]
        [Description("Нужен - TRUE, не нужен - FALSE")]
        [Category("Основные")]
        public String NewLine
        {
            get { return GetStringXmlAttribute("newline"); }
            set { SetStringXmlAttribute("newline", value); }
        }
    }


}