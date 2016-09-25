using System;
using System.Windows.Forms;
using System.Xml;

namespace CommandsLib
{
    public static class Commands
    {
        public static System.Collections.Generic.Dictionary<String, ToolStripItem> toolStripItems;
        public static TreeNode contextMenuTreeNode;
        private static void Add(String name, String text, System.EventHandler target)
        {
            System.Windows.Forms.ToolStripMenuItem toolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem.Name = name + "ToolStripMenuItem";
            toolStripMenuItem.Text = text;
            toolStripMenuItem.Click += target;
            toolStripItems.Add(name, toolStripMenuItem);
        }
        static Commands()
        {
            toolStripItems = new System.Collections.Generic.Dictionary<String, ToolStripItem>();
            Add("AddConnectTable", "Таблицу подключения", new System.EventHandler(AddConnectTableToolStripMenuItem_Click));
            Add("AddConnector", "Разъём", new System.EventHandler(AddConnectorToolStripMenuItem_Click));
            Add("AddAdapter", "Переходное устройство", new System.EventHandler(AddAdapterToolStripMenuItem_Click));
            Add("AddMachineConnector", "Разъём для машины", new System.EventHandler(AddMachineConnectorToolStripMenuItem_Click));
            Add("AddGroup", "Группу", new System.EventHandler(AddGroupToolStripMenuItem_Click));
            Add("AddProgram", "Программу", new System.EventHandler(AddProgramToolStripMenuItem_Click));
            Add("AddBlock", "Блок", new System.EventHandler(AddBlockToolStripMenuItem_Click));
            Add("AddVariable", "Выражение", new System.EventHandler(AddVariableToolStripMenuItem_Click));
            Add("AddFor", "Цикл", new System.EventHandler(AddForToolStripMenuItem_Click));
            Add("AddIf", "Условие", new System.EventHandler(AddIfToolStripMenuItem_Click));
            Add("AddCommutator", "Команду коммутатора", new System.EventHandler(AddCommutatorToolStripMenuItem_Click));
            Add("AddMeasure", "Измерение", new System.EventHandler(AddMeasureToolStripMenuItem_Click));
            Add("Run", "Выполнить", new System.EventHandler(RunToolStripMenuItem_Click));
            Add("Rename", "Переименовать", new System.EventHandler(RenameToolStripMenuItem_Click));
            Add("Close", "Закрыть", new System.EventHandler(CloseToolStripMenuItem_Click));
        }
        public static void AddConnectTable(TreeNode treeNode)
        {
            /*XmlNode xmlNode = XmlInfrastructure.xmlDoc.CreateElement("ConnectorTable");
            TreeNodeSatellite.GetXmlNode(treeNode).AppendChild(xmlNode);
            TreeNodeSatellite node = new TreeNodeSatellite(AbstractNodeType.ConnectTable, xmlNode);
            TreeNode connectorTableNode = new TreeNode("Таблица подключения");
            connectorTableNode.Tag = node;
            connectorTableNode.ImageIndex = 1;
            connectorTableNode.SelectedImageIndex = 1;
            treeNode.Nodes.Add(connectorTableNode);
            treeNode.Expand();*/
        }
        public static void AddConnectTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddConnectTable(contextMenuTreeNode);
        }
        public static void AddConnector(TreeNode treeNode)
        {
            /*XmlNode xmlNode = XmlInfrastructure.xmlDoc.CreateElement("Connector");
            XmlAttribute xmlAttribute = XmlInfrastructure.xmlDoc.CreateAttribute("name");
            xmlAttribute.Value = "Разъём";
            xmlNode.Attributes.Append(xmlAttribute);
            xmlAttribute = XmlInfrastructure.xmlDoc.CreateAttribute("type");
            xmlAttribute.Value = "Тип";
            xmlNode.Attributes.Append(xmlAttribute);
            xmlAttribute = XmlInfrastructure.xmlDoc.CreateAttribute("adapter");
            xmlAttribute.Value = "Адаптер";
            xmlNode.Attributes.Append(xmlAttribute);
            TreeNodeSatellite.GetXmlNode(treeNode).AppendChild(xmlNode);
            TreeNodeSatellite node = new TreeNodeSatellite(AbstractNodeType.Connector, xmlNode);
            TreeNode connectorNode = new TreeNode("Разъём");
            connectorNode.Tag = node;
            connectorNode.ImageIndex = 2;
            connectorNode.SelectedImageIndex = 2;
            treeNode.Nodes.Add(connectorNode);
            treeNode.Expand();*/
        }
        public static void AddConnectorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddConnector(contextMenuTreeNode);
        }
        public static void AddAdapter(TreeNode treeNode)
        {
            /*XmlNode xmlNode = XmlInfrastructure.xmlDoc.CreateElement("Adapter");
            XmlAttribute xmlAttribute = XmlInfrastructure.xmlDoc.CreateAttribute("name");
            xmlAttribute.Value = "Адаптер";
            xmlNode.Attributes.Append(xmlAttribute);
            xmlAttribute = XmlInfrastructure.xmlDoc.CreateAttribute("id");
            xmlAttribute.Value = "";
            xmlNode.Attributes.Append(xmlAttribute);
            TreeNodeSatellite.GetXmlNode(treeNode).AppendChild(xmlNode);
            TreeNodeSatellite node = new TreeNodeSatellite(AbstractNodeType.Adapter, xmlNode);
            TreeNode adapterNode = new TreeNode("Адаптер");
            adapterNode.Tag = node;
            adapterNode.ImageIndex = 3;
            adapterNode.SelectedImageIndex = 3;
            treeNode.Nodes.Add(adapterNode);
            treeNode.Expand();*/
        }
        public static void AddAdapterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddAdapter(contextMenuTreeNode);
        }
        public static void AddMachineConnector(TreeNode treeNode)
        {
            /*XmlNode xmlNode = XmlInfrastructure.xmlDoc.CreateElement("MachineConnector");
            XmlAttribute xmlAttribute = XmlInfrastructure.xmlDoc.CreateAttribute("name");
            xmlAttribute.Value = "Машинный разъём";
            xmlNode.Attributes.Append(xmlAttribute);
            TreeNodeSatellite.GetXmlNode(treeNode).AppendChild(xmlNode);
            TreeNodeSatellite node = new TreeNodeSatellite(AbstractNodeType.MachineConnector, xmlNode);
            TreeNode machineConnectorNode = new TreeNode("Машинный разъём");
            machineConnectorNode.Tag = node;
            machineConnectorNode.ImageIndex = 4;
            machineConnectorNode.SelectedImageIndex = 4;
            treeNode.Nodes.Add(machineConnectorNode);
            treeNode.Expand();*/
        }
        public static void AddMachineConnectorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddMachineConnector(contextMenuTreeNode);
        }
        public static void AddGroup(TreeNode treeNode)
        {
            /*XmlNode xmlNode = XmlInfrastructure.xmlDoc.CreateElement("Group");
            XmlAttribute xmlAttribute = XmlInfrastructure.xmlDoc.CreateAttribute("name");
            xmlAttribute.Value = "Группа";
            xmlNode.Attributes.Append(xmlAttribute);
            xmlAttribute = XmlInfrastructure.xmlDoc.CreateAttribute("comment");
            xmlAttribute.Value = "";
            xmlNode.Attributes.Append(xmlAttribute);
            TreeNodeSatellite.GetXmlNode(treeNode).AppendChild(xmlNode);
            TreeNodeSatellite node = new TreeNodeSatellite(AbstractNodeType.Group, xmlNode);
            TreeNode groupNode = new TreeNode("Группа");
            groupNode.Tag = node;
            groupNode.ImageIndex = 5;
            groupNode.SelectedImageIndex = 5;
            treeNode.Nodes.Add(groupNode);
            treeNode.Expand();*/
        }
        public static void AddGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddGroup(contextMenuTreeNode);
        }
        public static void AddProgram(TreeNode treeNode)
        {
            /*XmlNode xmlNode = XmlInfrastructure.xmlDoc.CreateElement("Program");
            XmlAttribute xmlAttribute = XmlInfrastructure.xmlDoc.CreateAttribute("name");
            xmlAttribute.Value = "Программа";
            xmlNode.Attributes.Append(xmlAttribute);
            xmlAttribute = XmlInfrastructure.xmlDoc.CreateAttribute("comment");
            xmlAttribute.Value = "";
            xmlNode.Attributes.Append(xmlAttribute);
            TreeNodeSatellite.GetXmlNode(treeNode).AppendChild(xmlNode);
            TreeNodeSatellite node = new TreeNodeSatellite(AbstractNodeType.Program, xmlNode);
            TreeNode programNode = new TreeNode("Программа");
            programNode.Tag = node;
            programNode.ImageIndex = 6;
            programNode.SelectedImageIndex = 6;
            treeNode.Nodes.Add(programNode);
            treeNode.Expand();*/
        }
        public static void AddProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddProgram(contextMenuTreeNode);
        }
        public static void AddBlock(TreeNode treeNode)
        {
            /*XmlNode xmlNode = XmlInfrastructure.xmlDoc.CreateElement("Block");
            XmlAttribute xmlAttribute = XmlInfrastructure.xmlDoc.CreateAttribute("name");
            xmlAttribute.Value = "Блок";
            xmlNode.Attributes.Append(xmlAttribute);
            xmlAttribute = XmlInfrastructure.xmlDoc.CreateAttribute("comment");
            xmlAttribute.Value = "";
            xmlNode.Attributes.Append(xmlAttribute);
            TreeNodeSatellite.GetXmlNode(treeNode).AppendChild(xmlNode);
            TreeNodeSatellite node = new TreeNodeSatellite(AbstractNodeType.Block, xmlNode);
            TreeNode blockNode = new TreeNode("Блок");
            blockNode.Tag = node;
            blockNode.ImageIndex = 7;
            blockNode.SelectedImageIndex = 7;
            treeNode.Nodes.Add(blockNode);
            treeNode.Expand();*/
        }
        public static void AddBlockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddBlock(contextMenuTreeNode);
        }
        public static void AddVariable(TreeNode treeNode)
        {
            /*XmlNode xmlNode = XmlInfrastructure.xmlDoc.CreateElement("Variable");
            XmlAttribute xmlAttribute = XmlInfrastructure.xmlDoc.CreateAttribute("name");
            xmlAttribute.Value = "Переменная";
            xmlNode.Attributes.Append(xmlAttribute);
            xmlAttribute = XmlInfrastructure.xmlDoc.CreateAttribute("initializer");
            xmlAttribute.Value = "";
            xmlNode.Attributes.Append(xmlAttribute);
            xmlAttribute = XmlInfrastructure.xmlDoc.CreateAttribute("comment");
            xmlAttribute.Value = "";
            xmlNode.Attributes.Append(xmlAttribute);
            TreeNodeSatellite.GetXmlNode(treeNode).AppendChild(xmlNode);
            TreeNodeSatellite node = new TreeNodeSatellite(AbstractNodeType.Variable, xmlNode);
            TreeNode variableNode = new TreeNode("Переменная");
            variableNode.Tag = node;
            variableNode.ImageIndex = 8;
            variableNode.SelectedImageIndex = 8;
            treeNode.Nodes.Add(variableNode);
            treeNode.Expand();*/
        }
        public static void AddVariableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddVariable(contextMenuTreeNode);
        }
        public static void AddForToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
        public static void AddIfToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
        public static void AddCommutatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
        public static void AddMeasureToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
        public static void Run(TreeNode treeNode)
        {
            MessageBox.Show(treeNode.Text);
        }
        public static void RunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Run(contextMenuTreeNode);
        }
        public static void Rename(TreeNode treeNode)
        {
            treeNode.BeginEdit();
        }
        public static void RenameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Rename(contextMenuTreeNode);
        }
        public static void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
    }

}
