using System;
using System.Collections.Generic;
using System.Xml;

using TestBenchClass;

namespace Interpreter
{
    /// <summary>
    /// Класс интерпретатора программ
    /// </summary>
    public class Interpreter
    {
        public OMK.TabPageSatellite tabPageSatellite;
        /// <summary>
        /// XML элемент корневого выполняемого оператора
        /// </summary>
        XmlNode executingRootXmlNode;
        /// <summary>
        /// XML элемент текущего оператора
        /// </summary>
        XmlNode currentXmlNode;
        private class Variable
        {
            private double internalValue;
            public int IntValue
            {
                get { return Convert.ToInt32(System.Math.Truncate(internalValue)); }
                set { internalValue = value; }
            }
            public byte ByteValue
            {
                get { return Convert.ToByte(internalValue); }
                set { internalValue = value; }
            }
            public double DoubleValue
            {
                get { return internalValue; }
                set { internalValue = value; }
            }
            public string StringValue
            {
                get { return internalValue.ToString("F"); }
                set { internalValue = double.Parse(value); }
            }
            public Variable() { internalValue = 0; }
            public Variable(int value) { internalValue = value; }
            public Variable(byte value) { internalValue = value; }
            public Variable(double value) { internalValue = value; }
            public Variable(string value) { internalValue = double.Parse(value); }
            public static Variable operator +(Variable argument1, Variable argument2)
            {
                Variable result = new Variable();
                result.internalValue = argument1.internalValue + argument2.internalValue;
                return result;
            }
            public static Variable operator -(Variable argument1, Variable argument2)
            {
                Variable result = new Variable();
                result.internalValue = argument1.internalValue - argument2.internalValue;
                return result;
            }
            public static Variable operator *(Variable argument1, Variable argument2)
            {
                Variable result = new Variable();
                result.internalValue = argument1.internalValue * argument2.internalValue;
                return result;
            }
            public static Variable operator /(Variable argument1, Variable argument2)
            {
                Variable result = new Variable();
                result.internalValue = argument1.internalValue / argument2.internalValue;
                return result;
            }
            public static Variable operator %(Variable argument1, Variable argument2)
            {
                Variable result = new Variable();
                result.internalValue = argument1.IntValue % argument2.IntValue;
                return result;
            }
            public static Variable operator <(Variable argument1, Variable argument2)
            {
                Variable result = new Variable();
                result.internalValue = argument1.internalValue < argument2.internalValue ? 1 : 0;
                return result;
            }
            public static Variable operator ==(Variable argument1, Variable argument2)
            {
                Variable result = new Variable();
                result.internalValue = argument1.internalValue == argument2.internalValue ? 1 : 0;
                return result;
            }
            public static Variable operator !=(Variable argument1, Variable argument2)
            {
                Variable result = new Variable();
                result.internalValue = argument1.internalValue == argument2.internalValue ? 0 : 1;
                return result;
            }
            public static Variable operator >(Variable argument1, Variable argument2)
            {
                Variable result = new Variable();
                result.internalValue = argument1.internalValue > argument2.internalValue ? 1 : 0;
                return result;
            }
            public static Variable operator !(Variable argument)
            {
                Variable result = new Variable();
                result.internalValue = argument.internalValue == 0 ? 1 : 0;
                return result;
            }
            public static Variable operator &(Variable argument1, Variable argument2)
            {
                Variable result = new Variable();
                result.internalValue = (argument1.internalValue != 0) && (argument2.internalValue != 0) ? 1 : 0;
                return result;
            }
            public static Variable operator |(Variable argument1, Variable argument2)
            {
                Variable result = new Variable();
                result.internalValue = (argument1.internalValue != 0) || (argument2.internalValue != 0) ? 1 : 0;
                return result;
            }
            public static Variable operator ^(Variable argument1, Variable argument2)
            {
                Variable result = new Variable();
                result.internalValue = (argument1.internalValue != 0) == (argument2.internalValue != 0) ? 0 : 1;
                return result;
            }
        }
        /// <summary>
        /// Словарь для хранения идентификаторов переменных и их значений
        /// </summary>
        Dictionary<string, Variable> variables;
        /// <summary>
        /// Экземпляр класса испытательного стенда
        /// </summary>
        TestBench testBench;
        public delegate void void_void();
        public void_void OnProgramEnd;
        public delegate void void_Str(string S);
        public void_Str WriteInstruction;
        /// <summary>
        /// Конструктор класса интерпретатора
        /// </summary>
        /// <param name="FileName">Имя файла программы</param>
        /// <param name="TestBench">Испытательный стенд</param>
        public Interpreter(XmlNode nodeToExecute, TestBench testBench)
        {
            try
            {
                //System.Windows.Forms.MessageBox.Show("node: " + nodeToExecute.Name + " ec: " + testBench.modbus.errorCode.ToString());
                executingRootXmlNode = nodeToExecute;
                currentXmlNode = executingRootXmlNode;
                variables = new Dictionary<string, Variable>();
                variables.Add("TestResult", new Variable(0));
                variables.Add("MeasureResult", new Variable(0));
                variables.Add("", new Variable(double.NaN));
                this.testBench = testBench;
                testBench.RXcomplete += RxComplete;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
        public void RxComplete()
        {
            //switch (GlobalMode)
            //{
                //case GlobalModes.Interpreting:
                    EventHandler();
                    //break;
            //}
        }
        /// <summary>
        /// Функция обработки событий, запускающих продолжение интерпретации текущей программы
        /// </summary>
        public void EventHandler()
        {
            //TestBench.Write("_EH", Color.AntiqueWhite);
            for (; Interpretation(); ) ;
        }
        /// <summary>
        /// Функция парсит выражение во входной строке
        /// </summary>
        /// <param name="S">Выражение</param>
        /// <returns>Значение выражения</returns>
        private Variable ParseVariable(string S)
        {
            if (S.IndexOf('(') >= 0)
            {
                int LeftParenthesisIndex = S.IndexOf('(');
                int RightParenthesisIndex = S.LastIndexOf(')');
                int CommaIndex = -1;
                int ParenthesisLevel = 0;
                for (int i = LeftParenthesisIndex + 1; i < RightParenthesisIndex; i++)
                {
                    if (S[i] == '(') ParenthesisLevel++;
                    if (S[i] == ')') ParenthesisLevel--;
                    if (ParenthesisLevel == 0)
                        if (S[i] == ',')
                        {
                            CommaIndex = i;
                            break;
                        }
                }
                string Func = S.Substring(0, LeftParenthesisIndex).Trim();
                string argument1String = S.Substring(LeftParenthesisIndex + 1, CommaIndex - LeftParenthesisIndex - 1).Trim();
                string argument2String = S.Substring(CommaIndex + 1, RightParenthesisIndex - CommaIndex - 1).Trim();
                Variable argument1 = ParseVariable(argument1String);
                Variable argument2 = ParseVariable(argument2String);
                switch (Func)
                {
                    case "ADD":
                    case "SUM":
                        return argument1 + argument2;
                    case "SUB":
                        return argument1 - argument2;
                    case "MUL":
                        return argument1 * argument2;
                    case "DIV":
                        return argument1 / argument2;
                    case "MOD":
                        return argument1 % argument2;
                    case "LT":
                        return argument1 < argument2;
                    case "EQU":
                        return argument1 == argument2;
                    case "GT":
                        return argument1 > argument2;
                    case "NOT":
                        return !argument1;
                    case "AND":
                        return argument1 & argument2;
                    case "OR":
                        return argument1 | argument2;
                    case "XOR":
                        return argument1 ^ argument2;
                    case "REG":
                        if ((argument1.IntValue < testBench.modbus.receivedRegisters.Length) & (argument1.IntValue >= 0))
                            return new Variable(testBench.modbus.receivedRegisters[argument1.IntValue]);
                        else
                            return new Variable(-1);
                    case "EC":
                        return new Variable(testBench.modbus.errorCode);
                    default:
                        return new Variable();
                }
            }
            else
                try
                {
                    return new Variable(S);
                }
                catch (Exception)
                {
                    return variables[S];
                }
        }
        /// <summary>
        /// Функция нахождения следующего оператора, реализует логику циклов и условий
        /// </summary>
        /// <returns>XML элемент следующего оператора</returns>
        private XmlNode NextOpNode()
        {
            XmlNode Node = currentXmlNode;
            switch (Node.Name)
            {
                case "FOR":
                    XmlAttribute Identifier = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("identifier");
                    XmlAttribute Initializer = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("initializer");
                    XmlAttribute End = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("end");
                    XmlAttribute Step = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("step");
                    if (Identifier != null)
                        if (End != null)
                            if (variables[Identifier.Value].DoubleValue <= ParseVariable(End.Value).DoubleValue)
                                return currentXmlNode.FirstChild; //Уходим на очередной проход тела оператора цикла
                    break;
                case "IF":
                    XmlAttribute Expression = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("expression");
                    if (Expression != null)
                        if (ParseVariable(Expression.Value).DoubleValue != 0)
                            return currentXmlNode.FirstChild; //Условие выполнено, выполняем тело условного оператора
                    break;
                case "PROGRAM":
                    return currentXmlNode.FirstChild;
            }
            while (true)
            {
                if (Node == executingRootXmlNode)
                    return null;
                if (Node.NextSibling != null) //Будет пустым, если конец блока
                    return Node.NextSibling;
                else
                {
                    Node = Node.ParentNode;
                    switch (Node.Name)
                    {
                        case "FOR":
                            XmlAttribute Identifier = (XmlAttribute)Node.Attributes.GetNamedItem("identifier");
                            //XmlAttribute Initializer = (XmlAttribute)Node.Attributes.GetNamedItem("initializer");
                            XmlAttribute End = (XmlAttribute)Node.Attributes.GetNamedItem("end");
                            XmlAttribute Step = (XmlAttribute)Node.Attributes.GetNamedItem("step");
                            if (Identifier != null)
                            {
                                //MessageBox.Show("Test");
                                if (End != null)
                                {
                                    //MessageBox.Show("Variables[Identifier.Value] = " + Variables[Identifier.Value] + " ParseIntVar(End.Value) = " + ParseIntVar(End.Value));
                                    if (Step != null)
                                        variables[Identifier.Value] += ParseVariable(Step.Value);
                                    else
                                        variables[Identifier.Value].DoubleValue += 1;
                                    if (variables[Identifier.Value].DoubleValue <= ParseVariable(End.Value).DoubleValue)
                                        return Node.FirstChild;
                                }
                            }
                            break;
                        case "IF":
                            break;
                        case "PROGRAM":
                            break;
                        default:
                            return null;
                    }
                }
            }
        }
        private Modbus.Modbus.void_StrColDelegate protocolWrite;
        public Modbus.Modbus.void_StrColDelegate ProtocolWrite
        {
            get { return protocolWrite; }
            set { protocolWrite = value;}
        }
        /// <summary>
        /// Функция интерпретации очередного оператора текущей программы
        /// </summary>
        /// <returns>Необходимость перезапуска функции</returns>
        bool Interpretation()
        {
            bool NeedRestart = false;
            XmlAttribute Param1;
            XmlAttribute Param2;
            XmlAttribute Point;
            XmlAttribute Bus;
            XmlAttribute Identifier;
            XmlAttribute Initializer;
            if (currentXmlNode != null)
            {
                //TestBench.Write("_XN=" + XMLNode.Name + " ", Color.AntiqueWhite);
                if (WriteInstruction != null)
                    WriteInstruction(currentXmlNode.Name);
                 //ProtocolWrite(currentXmlNode.Name, System.Drawing.Color.Black);
                switch (currentXmlNode.Name)
                {
                    case "PROGRAM":
                        NeedRestart = true;
                        break;
                    case "FOR": //<FOR identifier="идентификатор" 'initializer="выражение" end="выражение" 'step="выражение"></FOR>
                        Identifier = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("identifier");
                        Initializer = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("initializer");
                        //XmlAttribute End = (XmlAttribute)XMLNode.Attributes.GetNamedItem("end");
                        //XmlAttribute Step = (XmlAttribute)XMLNode.Attributes.GetNamedItem("step");
                        if (Identifier != null)
                        {
                            if (variables.ContainsKey(Identifier.Value))
                            {
                                if (Initializer != null)
                                    variables[Identifier.Value] = ParseVariable(Initializer.Value);
                            }
                            else
                                if (Initializer != null)
                                    variables.Add(Identifier.Value, ParseVariable(Initializer.Value));
                                else
                                    variables.Add(Identifier.Value, new Variable(0));
                        }
                        NeedRestart = true;
                        break;
                    case "IF": //<IF expression="выражение"></IF>
                        NeedRestart = true;
                        break;
                    /*case "TEST": //<TEST expected="CLOSE|OPEN" mode="2|4"/>
                        XmlAttribute Expected = (XmlAttribute)XMLNode.Attributes.GetNamedItem("expected");
                        XmlAttribute Mode = (XmlAttribute)XMLNode.Attributes.GetNamedItem("mode");
                        if (Mode != null)
                        {
                            if (Mode.Value == "2") testBench.MeasurementFunction = TestBench.TestBench.MeasurementFunctions.Resistance;
                            if (Mode.Value == "4") testBench.MeasurementFunction = TestBench.TestBench.MeasurementFunctions.Resistance4Wire;
                        }

                        if (Expected != null)
                        {
                            switch (Expected.Value)
                            {
                                case "CLOSE":
                                    Variables["TestResult"] = testBench.Measure() ? 1 : 0;
                                    break;
                                case "OPEN":
                                    Variables["TestResult"] = testBench.Measure() ? 1 : 0;
                                    break;
                            }
                        }
                        NeedRestart = true;
                        break;*/
                    case "MEASURE": //<MEASURE function="VOLTAGE|CURRENT|RESISTANCE|..." currenttype="AC|DC" measurementcurrent="выражение" range="выражение"/>
                        XmlAttribute function = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("function");
                        XmlAttribute currenttype = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("currenttype");
                        XmlAttribute measurementcurrent = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("measurementcurrent");
                        XmlAttribute range = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("range");
                        if (function != null)
                        {
                            switch (function.Value.ToUpper())
                            {
                                case "VOLTAGE":
                                    testBench.MeasurementFunction = TestBench.MeasurementFunctions.Voltage;
                                    break;
                                case "CURRENT":
                                    testBench.MeasurementFunction = TestBench.MeasurementFunctions.Current;
                                    break;
                                case "RESISTANCE":
                                    testBench.MeasurementFunction = TestBench.MeasurementFunctions.Resistance;
                                    break;
                                case "RESISTANCE4WIRE":
                                    testBench.MeasurementFunction = TestBench.MeasurementFunctions.Resistance4Wire;
                                    break;
                                case "CAPACITANCE":
                                    testBench.MeasurementFunction = TestBench.MeasurementFunctions.Capacitance;
                                    break;
                                case "FREQUENCY":
                                    testBench.MeasurementFunction = TestBench.MeasurementFunctions.Frequency;
                                    break;
                                case "PERIOD":
                                    testBench.MeasurementFunction = TestBench.MeasurementFunctions.Period;
                                    break;
                                case "TEMPERATURE":
                                    testBench.MeasurementFunction = TestBench.MeasurementFunctions.Temperature;
                                    break;
                                case "WITHSTANDING":
                                    testBench.MeasurementFunction = TestBench.MeasurementFunctions.Withstanding;
                                    break;
                                case "DIODE":
                                    testBench.MeasurementFunction = TestBench.MeasurementFunctions.Diode;
                                    break;
                            }
                        }
                        if (currenttype != null)
                        {
                            switch (currenttype.Value.ToUpper())
                            {
                                case "AC":
                                    testBench.MeasurementCurrentType = TestBench.MeasurementCurrentTypes.AC;
                                    break;
                                case "DC":
                                    testBench.MeasurementCurrentType = TestBench.MeasurementCurrentTypes.DC;
                                    break;
                            }
                        }
                        if (measurementcurrent != null)
                            testBench.ResistanceMeasurementCurrent = ParseVariable(measurementcurrent.Value).DoubleValue;
                        if (range != null)
                            testBench.MeasurementRange = ParseVariable(range.Value).DoubleValue;
                        variables["MeasureResult"] = new Variable(testBench.Measure());
                        NeedRestart = true;
                        break;
                    case "MODBUSADDR": //<MODBUSADDR addr="выражение"/>
                        Param1 = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("addr");
                        if (Param1 != null)
                        {
                            testBench.modbus.Addr = ParseVariable(Param1.Value).ByteValue;
                        }
                        NeedRestart = true;
                        break;
                    case "RK104RESET": //<RK104RESET 'addr="выражение"/>
                        Param1 = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("addr");
                        if (Param1 != null)
                            testBench.modbus.Addr = ParseVariable(Param1.Value).ByteValue;
                        testBench.RK104FullReset();
                        break;
                    case "RK104CLOSE": //<RK104CLOSE 'addr="выражение" point="выражение" buses="выражение"/>
                        Point = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("point");
                        Bus = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("buses");
                        Param1 = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("addr");
                        if ((Point != null) & (Bus != null))
                        {
                            if (Param1 != null)
                                testBench.modbus.Addr = ParseVariable(Param1.Value).ByteValue;
                            testBench.RK104SetPoint(ParseVariable(Point.Value).ByteValue, ParseVariable(Bus.Value).ByteValue);
                        }
                        break;
                    case "RK104OPEN": //<RK104OPEN 'addr="выражение" point="выражение" buses="выражение"/>
                        Point = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("point");
                        Bus = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("buses");
                        Param1 = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("addr");
                        if ((Point != null) & (Bus != null))
                        {
                            if (Param1 != null)
                                testBench.modbus.Addr = ParseVariable(Param1.Value).ByteValue;
                            testBench.RK104ResetPoint(ParseVariable(Point.Value).ByteValue, ParseVariable(Bus.Value).ByteValue);
                        }
                        break;
                    case "RK104LOAD": //<RK104LOAD 'addr="выражение" point="выражение" buses="выражение"/>
                        Point = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("point");
                        Bus = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("buses");
                        Param1 = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("addr");
                        if ((Point != null) & (Bus != null))
                        {
                            if (Param1 != null)
                                testBench.modbus.Addr = ParseVariable(Param1.Value).ByteValue;
                            testBench.RK104LoadPoint(ParseVariable(Point.Value).ByteValue, ParseVariable(Bus.Value).ByteValue);
                        }
                        break;
                    case "COMMUTATOR": //<COMMUTATOR 'addr="выражение" point="выражение" buses="выражение" state="OPEN|CLOSE"/>
                        Point = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("point");
                        Bus = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("buses");
                        Param1 = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("state");
                        Param2 = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("addr");
                        if ((Point != null) & (Bus != null) & (Param1 != null))
                        {
                            if (Param2 != null)
                                testBench.modbus.Addr = ParseVariable(Param2.Value).ByteValue;
                            switch (Param1.Value.ToUpper())
                            {
                                case "OPEN":
                                    testBench.RK104ResetPoint(ParseVariable(Point.Value).ByteValue, ParseVariable(Bus.Value).ByteValue);
                                    break;
                                case "CLOSE":
                                    testBench.RK104SetPoint(ParseVariable(Point.Value).ByteValue, ParseVariable(Bus.Value).ByteValue);
                                    break;
                            }
                        }

                        break;
                    case "PAUSE": //<PAUSE 'interval="выражение"/>
                        Param1 = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("interval");
                        if (Param1 != null)
                            System.Threading.Thread.Sleep(ParseVariable(Param1.Value).IntValue);
                        else
                            System.Threading.Thread.Sleep(1000);
                        NeedRestart = true;
                        break;
                    case "STOP": //<STOP/>
                        break;
                    case "BEEP": //<BEEP/>
                        //System.Media.SystemSounds.Beep.Play();
                        Console.Beep();
                        NeedRestart = true;
                        break;
                    case "ECHO": //<ECHO>текст</ECHO>
                        testBench.Write(currentXmlNode.InnerText + "\n", System.Drawing.Color.Aquamarine);
                        NeedRestart = true;
                        break;
                    case "VAR": //<VAR identifier="идентификатор" initializer="выражение">
                        Identifier = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("identifier");
                        Initializer = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("initializer");
                        if (Identifier != null)
                        {
                            if (variables.ContainsKey(Identifier.Value))
                            {
                                if (Initializer != null)
                                    variables[Identifier.Value] = ParseVariable(Initializer.Value);
                            }
                            else
                                if (Initializer != null)
                                    variables.Add(Identifier.Value, ParseVariable(Initializer.Value));
                                else
                                    variables.Add(Identifier.Value, new Variable(0));
                        }
                        NeedRestart = true;
                        break;
                    case "MESSAGE": //<MESSAGE text="текст">текст</MESSAGE>
                        {
                            XmlAttribute Text = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("text");
                            if (Text != null)
                                System.Windows.Forms.MessageBox.Show(Text.Value + currentXmlNode.InnerText);
                            else
                                System.Windows.Forms.MessageBox.Show(currentXmlNode.InnerText);
                            NeedRestart = true;
                        }
                        break;
                    case "WRITE": //  <WRITE expression="выражение" textbefore="текст до" textafter="текст после" newline="TRUE|FALSE"/>
                        {
                            XmlAttribute TextBefore = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("textbefore");
                            if (TextBefore != null)
                                ProtocolWrite(TextBefore.Value, System.Drawing.Color.Aquamarine);
                            XmlAttribute Expression = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("expression");
                            if (Expression != null)
                                ProtocolWrite(ParseVariable(Expression.Value).StringValue, System.Drawing.Color.Aquamarine);
                            XmlAttribute TextAfter = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("textafter");
                            if (TextAfter != null)
                                ProtocolWrite(TextAfter.Value, System.Drawing.Color.Aquamarine);
                            ProtocolWrite(currentXmlNode.InnerText, System.Drawing.Color.Aquamarine);
                            XmlAttribute NewLine = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("newline");
                            if (NewLine != null)
                            {
                                if (NewLine.Value == "TRUE")
                                    ProtocolWrite("\n", System.Drawing.Color.Aquamarine);
                            }
                            else
                                ProtocolWrite("\n", System.Drawing.Color.Aquamarine);
                            NeedRestart = true;
                        }
                        break;
                    case "DIALOG": //  <DIALOG expression="выражение" textbefore="текст до" textafter="текст после"/>
                        {
                            String dialogText = "";
                            XmlAttribute TextBefore = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("textbefore");
                            if (TextBefore != null)
                                dialogText += TextBefore.Value;
                            XmlAttribute Expression = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("expression");
                            if (Expression != null)
                                dialogText += ParseVariable(Expression.Value).StringValue;
                            XmlAttribute TextAfter = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("textafter");
                            if (TextAfter != null)
                                dialogText += TextAfter.Value;
                            dialogText += currentXmlNode.InnerText;
                            OMK.FormInterpreterDialog dialog = new OMK.FormInterpreterDialog();
                            dialog.dialogText = dialogText;
                            Initializer = (XmlAttribute)currentXmlNode.Attributes.GetNamedItem("initializer");
                            if (Initializer != null)
                                dialog.Value = ParseVariable(Initializer.Value).DoubleValue;
                            dialog.ShowDialog();
                            NeedRestart = true;
                        }
                        break;
                    /*case "ERROR": //  <ERROR expression="выражение" textbefore="текст до" textafter="текст после" newline="TRUE|FALSE"/>
                        {
                            XmlAttribute TextBefore = (XmlAttribute)XMLNode.Attributes.GetNamedItem("textbefore");
                            if (TextBefore != null)
                                testBench.ErrorString += TextBefore.Value;
                            XmlAttribute Expression = (XmlAttribute)XMLNode.Attributes.GetNamedItem("expression");
                            if (Expression != null)
                                testBench.ErrorString += ParseVariable(Expression.Value).StringValue;
                            XmlAttribute TextAfter = (XmlAttribute)XMLNode.Attributes.GetNamedItem("textafter");
                            if (TextAfter != null)
                                testBench.ErrorString += TextAfter.Value;
                            testBench.ErrorString += XMLNode.InnerText;
                            XmlAttribute NewLine = (XmlAttribute)XMLNode.Attributes.GetNamedItem("newline");
                            if (NewLine != null)
                            {
                                if (NewLine.Value == "TRUE")
                                    testBench.ErrorString += "\n";
                            }
                            else
                                testBench.ErrorString += "\n";
                            NeedRestart = true;
                        }
                        break;*/
                }
                //ProtocolWrite(" finish, next ", System.Drawing.Color.Black);
                currentXmlNode = NextOpNode();
                //ProtocolWrite(currentXmlNode.Name + ", needRestart=" + NeedRestart.ToString() + "\n", System.Drawing.Color.Black);
                return NeedRestart;
            }
            else
            {
                //TestBench.Write("Конец программы.\n", Color.Aqua);
                OnProgramEnd();
                return false;
            }
        }
    }
}
