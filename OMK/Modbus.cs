using System;

namespace Modbus
{
    public class Modbus : System.Windows.Forms.Control
    {
        public delegate void void_StrColDelegate(string s, System.Drawing.Color color);
        /// <summary>
        /// Делегат функции вывода текста в диалоговое окно
        /// </summary>
        public void_StrColDelegate Write;
        public void_StrColDelegate ProtocolWrite;
        public delegate void void_StrStrColDelegate(string s1, string s2, System.Drawing.Color color);
        /// <summary>
        /// Делегат функции вывода текста в лог обмена по последовательному интерфейсу
        /// </summary>
        public void_StrStrColDelegate WriteSerialLog;
        private byte addr;
        public byte Addr
        {
            get { return addr; }
            set { addr = value; }
        }
        private byte currentAddr;
        private byte currentFunction;
        private ushort currentStartRegister;
        private ushort currentRegisterCount;
        private ushort currentCRC16;
        private byte[] rxPacket;
        private ushort rxPacketIndex;
        public bool debugOutput;
        /// <summary>
        /// Массив для принятых значений регистров, MSB first
        /// </summary>
        public ushort[] receivedRegisters;
        private ushort receivingRegisterIndex;
        public int errorCode;
        public delegate void void_iii(int i, int j, int k);
        public event void_iii Error;
        public delegate void void_void();
        public event void_void RXcomplete;
        public event void_void TimeOut;
        private System.Windows.Forms.Timer timer;
        private System.IO.Ports.SerialPort serialPort;
        private char B2H(int b) //Целое в символьное представление 1-го 16-тиричного разряда
        {
            if (b < 10)
                return Convert.ToChar('0' + b);
            else
                return Convert.ToChar('A' + b - 10);
        }
        private string B2HS(byte b) //Байт в символьное представление 2-х 16-тиричных разрядов
        {
            string S = "";
            S += B2H(b / 16);
            S += B2H(b % 16);
            return S;
        }
        private string BA2HSx(byte[] byteArray) //Массив байт в символьное представление пар 16-тиричных разрядов с пробелами
        {
            string s = "";
            for (int i = 0; i < byteArray.Length; i++)
            {
                s += B2HS(byteArray[i]);
                if (i < (byteArray.Length - 1)) s += ' ';
            }
            return s;
        }
        public Modbus()
        {
            InitTimer();
            InitSerial();
        }
        private void InitTimer()
        {
            timer = new System.Windows.Forms.Timer();
            timer.Enabled = false;
            timer.Interval = 1000;
            timer.Tick += TimerTick;
        }
        delegate void void_IntDelegate(int i);
        private void TimerStart(int Interval)
        {
            if (InvokeRequired)
                Invoke(new void_IntDelegate(TimerStart), new object[] { Interval });
            else
            {
                timer.Interval = Interval;
                timer.Start();
            }
        }
        delegate void void_voidDelegate();
        private void TimerStop()
        {
            if (InvokeRequired)
                Invoke(new void_voidDelegate(TimerStop));
            else
            {
                timer.Stop();
            }
        }
        private void TimerTick(object sender, EventArgs e)
        {
            TimerStop();
            switch (transactionState)
            {
                case TransactionStates.Idle:
                case TransactionStates.Sending:
                    break;
                case TransactionStates.Wait4Answer:
                    transactionState = TransactionStates.Idle;
                    if (TimeOut != null) TimeOut();
                    Write("Истекло время ожидания ответа от устройства.\n", System.Drawing.Color.Red);
                    break;
            }
        }
        private void InitSerial()
        {
            String[] portNames = System.IO.Ports.SerialPort.GetPortNames();
            if (portNames != null)
                if (portNames.Length > 0)
                {
                    String portName = portNames[0];
                    serialPort = new System.IO.Ports.SerialPort(portName, 115200, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);
                    serialPort.DataReceived += serialPort_DataReceived;
                    serialPort.Open();
                }
                else
                    System.Windows.Forms.MessageBox.Show("Последовательные порты не обнаружены", "Ошибка инициализации Modbus");
            else
                System.Windows.Forms.MessageBox.Show("Не удалось получить список доступных портов", "Ошибка инициализации Modbus");
        }
        private void serialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            TimerStop();
            byte[] rxPacket = new byte[serialPort.BytesToRead];
            serialPort.Read(rxPacket, 0, rxPacket.Length);
            if (debugOutput)
                WriteSerialLog("Dev->Com: ", BA2HSx(rxPacket) + '\n', System.Drawing.Color.Yellow);
            transactionState = TransactionStates.Idle;
            DataReceived(rxPacket);
        }
        public enum ReceiveStates { IDLE, MANUAL, ADDR, FUNC, STARTREGH, STARTREGL, REGCNTH, REGCNTL, BYTECNT, REGH, REGL, CRCH, CRCL, ERRCODE };
        private ReceiveStates receiveState;
        /// <summary>
        /// Функция вычисляет CRC16 массива Buffer
        /// </summary>
        /// <param name="Buffer">Массив данных для вычисления CRC16</param>
        /// <returns>CRC16</returns>
        public ushort CRC16(byte[] Buffer)
        {
            ushort CRC = 0xFFFF;
            for (ushort i = 0; i < Buffer.Length; i++)
            {
                CRC ^= Buffer[i];
                for (byte j = 0; j < 8; j++)
                    if ((CRC & 0x0001) == 1)
                    {
                        CRC /= 2;
                        CRC ^= 0xA001;
                    }
                    else
                        CRC /= 2;
            }
            return CRC;
        }
        /// <summary>
        /// Функция вычисляет CRC16 первых LENGTH элементов массива Buffer
        /// </summary>
        /// <param name="LENGTH">Количество элементов от начала массива, участвующих в вычислении CRC16</param>
        /// <param name="Buffer">Массив данных для вычисления CRC16</param>
        /// <returns>CRC16</returns>
        public ushort CRC16(ushort LENGTH, byte[] Buffer)
        {
            ushort CRC = 0xFFFF;
            for (ushort i = 0; i < LENGTH; i++)
            {
                CRC ^= Buffer[i];
                for (byte j = 0; j < 8; j++)
                    if ((CRC & 0x0001) == 1)
                    {
                        CRC /= 2;
                        CRC ^= 0xA001;
                    }
                    else
                        CRC /= 2;
            }
            return CRC;
        }
        /// <summary>
        /// Функция вычисляет CRC16 элементов 0 .. (Buffer.Length - 3) массива Buffer и помещает её в последние два элемента, MSB first
        /// </summary>
        /// <param name="Buffer">Элементы 0 .. (Buffer.Length - 3) - массив данных для вычисления CRC16, последние два элемента - CRC16, MSB first</param>
        ushort CRC16_(byte LENGTH, byte[] ucBuffer)
        {
            byte i, j;
            ushort CRC = 0xFFFF;
            for (i = 0; i < LENGTH; i++)
            {
                CRC ^= ucBuffer[i + 1];
                for (j = 0; j < 8; j++)
                {
                    if ((CRC & 0x0001) == 1)
                    {
                        CRC /= 2;
                        CRC ^= 0xA001;
                    }
                    else
                    {
                        CRC /= 2;
                    }
                }
            }
            return CRC;
        }
        public void AppendCRC16(byte[] Buffer)
        {
            ushort CRC = 0xFFFF;
            for (ushort i = 0; i < (Buffer.Length - 2); i++)
            {
                CRC ^= Buffer[i];
                for (byte j = 0; j < 8; j++)
                    if ((CRC & 0x0001) == 1)
                    {
                        CRC /= 2;
                        CRC ^= 0xA001;
                    }
                    else
                        CRC /= 2;
            }
            Buffer[Buffer.Length - 2] = (byte)(CRC % (ushort)256);
            Buffer[Buffer.Length - 1] = (byte)(CRC / (ushort)256);
        }
        enum TransactionStates { Idle, Sending, Wait4Answer };
        TransactionStates transactionState;
        delegate bool bool_BaInt(byte[] ByteArray, int TimeOut);
        private bool SendData(byte[] byteArray, int timeOut)
        {
            if (InvokeRequired)
                return (bool)Invoke(new bool_BaInt(SendData), new object[] { byteArray, timeOut });
            else
            {
                while (transactionState != TransactionStates.Idle) ; //Ожидание окончания предидущей транзакции
                transactionState = TransactionStates.Sending;
                bool result = false;
                //if (serialPort1.IsOpen)
                {
                    try
                    {
                        if (serialPort.IsOpen)
                        {
                            serialPort.Write(byteArray, 0, byteArray.Length);
                            result = true;
                            if (debugOutput)
                                WriteSerialLog("Com->Dev: ", BA2HSx(byteArray) + '\n', System.Drawing.Color.GreenYellow);
                        }
                        else
                            System.Windows.Forms.MessageBox.Show("Последовательный порт закрыт", "Ошибка отправки данных по последовательному интерфейсу");
                    }
                    catch (Exception exc)
                    {
                        result = false;
                        Write("Ошибка отправки данных '" + BA2HSx(byteArray) + "': " + exc.ToString() + "\n", System.Drawing.Color.Red);
                    }
                }
                if (result)
                {
                    transactionState = TransactionStates.Wait4Answer;
                    TimerStart(timeOut);
                }
                else
                    transactionState = TransactionStates.Idle;
                return result;
            }
        }
        public void Transmit(byte[] txPacket)
        {
            if (SendData(txPacket, 1000) == false)
            {
                //GlobalMode = GlobalModes.Idle;
                //ButtonsEnable();
            }
        }
        /// <summary>
        /// Настраивает объект на начало приема пакета
        /// </summary>
        private void StartReceiving()
        {
            rxPacket = new byte[1024];
            rxPacketIndex = 0;
            errorCode = -1;
            receiveState = ReceiveStates.ADDR;
            if (currentFunction == 0x04)
            {
                receivedRegisters = new ushort[currentRegisterCount];
                receivingRegisterIndex = 0;
            }
        }
        /// <summary>
        /// Добавляет байт в массив принятого пакета
        /// </summary>
        /// <param name="B">Добавляемый байт</param>
        private void AppendRXbyte(byte b)
        {
            rxPacket[rxPacketIndex] = b;
            rxPacketIndex++;
        }
        /// <summary>
        /// Функция записи нескольких регистров (0x10)
        /// </summary>
        /// <param name="ADDR">MODBUS адрес устройства</param>
        /// <param name="StartReg">Номер первого регистра устройства</param>
        /// <param name="RegCnt">Количество регистров</param>
        /// <param name="Regs">Данные для регистров, MSB first</param>
        public void Function0x10(ushort startRegister, ushort registerCount, ushort[] registers)
        {
            currentAddr = addr;
            currentFunction = 0x10;
            currentStartRegister = startRegister;
            currentRegisterCount = registerCount;
            byte[] txPacket = new byte[9 + registerCount * 2];
            txPacket[0] = currentAddr;
            txPacket[1] = 0x10;
            txPacket[2] = (byte)(startRegister / (ushort)256);
            txPacket[3] = (byte)(startRegister % (ushort)256);
            txPacket[4] = (byte)(registerCount / (ushort)256);
            txPacket[5] = (byte)(registerCount % (ushort)256);
            txPacket[6] = (byte)(registerCount * 2u);
            for (int i = 0; i < registerCount; i++)
            {
                txPacket[7 + i * 2] = (byte)(registers[i] / (ushort)256);
                txPacket[8 + i * 2] = (byte)(registers[i] % (ushort)256);
            }
            AppendCRC16(txPacket);
            StartReceiving();
            Transmit(txPacket);
        }
        /// <summary>
        /// Функция чтения нескольких регистров (0x04)
        /// </summary>
        /// <param name="ADDR">MODBUS адрес устройства</param>
        /// <param name="StartReg">Номер первого регистра устройства</param>
        /// <param name="RegCnt">Количество регистров</param>
        public void Function0x04(ushort startRegister, ushort registerCount)
        {
            currentAddr = addr;
            currentFunction = 0x04;
            currentStartRegister = startRegister;
            currentRegisterCount = registerCount;
            byte[] txPacket = new byte[8];
            txPacket[0] = currentAddr;
            txPacket[1] = 0x04;
            txPacket[2] = (byte)(startRegister / (ushort)256);
            txPacket[3] = (byte)(startRegister % (ushort)256);
            txPacket[4] = (byte)(registerCount / (ushort)256);
            txPacket[5] = (byte)(registerCount % (ushort)256);
            AppendCRC16(txPacket);
            Transmit(txPacket);
            StartReceiving();
        }
        /// <summary>
        /// Функция побайтовой пересылки принятых данных в функцию-парсер
        /// </summary>
        /// <param name="Buffer">Принятые данные</param>
        public void DataReceived(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                ByteReceived(buffer[i]);
            }
        }
        /// <summary>
        /// Функция - парсер принятых данных
        /// </summary>
        /// <param name="RXbyte">Очередной принятый символ</param>
        private void ByteReceived(byte rxByte)
        {
            if ((receiveState != ReceiveStates.IDLE) && (receiveState != ReceiveStates.MANUAL))
                AppendRXbyte(rxByte);
            switch (receiveState)
            {
                case ReceiveStates.IDLE:
                    Error(1, 0, 0);
                    break;
                case ReceiveStates.MANUAL:
                    break;
                case ReceiveStates.ADDR:
                    if (rxByte == currentAddr)
                        receiveState = ReceiveStates.FUNC;
                    else
                        Error(2, rxByte, currentAddr);
                    break;
                case ReceiveStates.FUNC:
                    if ((rxByte & 0x80) != 0)
                    {
                        receiveState = ReceiveStates.ERRCODE;
                        break;
                    }
                    if (rxByte == currentFunction)
                        switch (currentFunction)
                        {
                            case 0x04:
                                receiveState = ReceiveStates.BYTECNT;
                                break;
                            case 0x10:
                                receiveState = ReceiveStates.STARTREGH;
                                break;
                            default:
                                receiveState = ReceiveStates.IDLE;
                                Error(3, currentFunction, 0);
                                break;
                        }
                    else
                    {
                        receiveState = ReceiveStates.IDLE;
                        Error(4, rxByte, currentFunction);
                    }
                    break;
                case ReceiveStates.ERRCODE:
                    errorCode = rxByte;
                    receiveState = ReceiveStates.CRCL;
                    break;
                case ReceiveStates.STARTREGH:
                    if (rxByte == (byte)(currentStartRegister / (ushort)256))
                        receiveState = ReceiveStates.STARTREGL;
                    else
                    {
                        receiveState = ReceiveStates.IDLE;
                        Error(5, 0, 0);
                    }
                    break;
                case ReceiveStates.STARTREGL:
                    if (rxByte == (byte)(currentStartRegister % (ushort)256))
                        receiveState = ReceiveStates.REGCNTH;
                    else
                    {
                        receiveState = ReceiveStates.IDLE;
                        Error(6, 0, 0);
                    }
                    break;
                case ReceiveStates.REGCNTH:
                    if (rxByte == (byte)(currentRegisterCount / (ushort)256))
                        receiveState = ReceiveStates.REGCNTL;
                    else
                    {
                        receiveState = ReceiveStates.IDLE;
                        Error(7, 0, 0);
                    }
                    break;
                case ReceiveStates.REGCNTL:
                    if (rxByte == (byte)(currentRegisterCount % (ushort)256))
                        receiveState = ReceiveStates.CRCL;
                    else
                    {
                        receiveState = ReceiveStates.IDLE;
                        Error(8, 0, 0);
                    }
                    break;
                case ReceiveStates.BYTECNT:
                    if (rxByte == (currentRegisterCount * 2))
                        receiveState = ReceiveStates.REGH;
                    else
                    {
                        receiveState = ReceiveStates.IDLE;
                        Error(9, 0, 0);
                    }
                    break;
                case ReceiveStates.REGH:
                    receivedRegisters[receivingRegisterIndex] = (ushort)(rxByte * (ushort)256);
                    receiveState = ReceiveStates.REGL;
                    break;
                case ReceiveStates.REGL:
                    receivedRegisters[receivingRegisterIndex] += rxByte;
                    receivingRegisterIndex++;
                    if (receivingRegisterIndex < currentRegisterCount)
                        receiveState = ReceiveStates.REGH;
                    else
                        receiveState = ReceiveStates.CRCL;
                    break;
                case ReceiveStates.CRCL:
                    currentCRC16 = CRC16((ushort)(rxPacketIndex - 1), rxPacket);
                    if (rxByte == (byte)(currentCRC16 % (ushort)256))
                    {
                        receiveState = ReceiveStates.CRCH;
                    }
                    else
                    {
                        receiveState = ReceiveStates.IDLE;
                        Error(12, 0, 0);
                    }
                    break;
                case ReceiveStates.CRCH:
                    if (rxByte == (byte)(currentCRC16 / (ushort)256))
                    {
                        receiveState = ReceiveStates.IDLE;
                        RXcomplete();
                    }
                    else
                    {
                        receiveState = ReceiveStates.IDLE;
                        Error(13, 0, 0);
                    }
                    break;
            }
        }
        public void SendManual(byte[] data)
        {
            byte[] txPacket = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                txPacket[i] = data[i];
            }
            receiveState = ReceiveStates.MANUAL;
            Transmit(txPacket);
        }
        public void SendManualWithCRC(byte[] data)
        {
            byte[] txPacket = new byte[data.Length + 2];
            for (int i = 0; i < data.Length; i++)
            {
                txPacket[i] = data[i];
            }
            AppendCRC16(txPacket);
            receiveState = ReceiveStates.MANUAL;
            Transmit(txPacket);
        }
    }
}