using System;

//using Ivi.Dmm.Interop;
//using Ivi.Driver.Interop;
//using Agilent.Agilent34410.Interop;
using Ivi.Dmm.Interop;
using Ivi.Driver.Interop;
using Agilent.Agilent34410.Interop;
using Ivi.ConfigServer.Interop;
using Modbus;

namespace TestBenchClass
{
    public class TestBench
    {
        /// <summary>
        /// Делегат функции вывода текста в диалоговое окно
        /// </summary>
        private Modbus.Modbus.void_StrColDelegate write;
        private Modbus.Modbus.void_StrColDelegate protocolWrite;
        public Modbus.Modbus.void_StrColDelegate Write
        {
            get { return write; }
            set { write = value; modbus.Write = value; }
        }
        public Modbus.Modbus.void_StrColDelegate ProtocolWrite
        {
            get { return protocolWrite; }
            set { protocolWrite = value; modbus.ProtocolWrite = value; }
        }
        private Modbus.Modbus.void_StrStrColDelegate writeSerialLog;
        public Modbus.Modbus.void_StrStrColDelegate WriteSerialLog
        {
            get { return writeSerialLog; }
            set { writeSerialLog = value; modbus.WriteSerialLog = value; }
        }
        public Modbus.Modbus modbus;
        private Agilent.Agilent34410.Interop.Agilent34410 dmm;
        public enum MeasurementFunctions { Voltage, Current, Resistance, Capacitance, Frequency, Period, Withstanding, Resistance4Wire, Temperature, Diode };
        private MeasurementFunctions measurementFunction;
        public MeasurementFunctions MeasurementFunction
        {
            get { return measurementFunction; }
            set
            {
                if (value != measurementFunction)
                {
                    measurementFunction = value;
                    UpdateMeasurementSettings();
                }
            }
        }
        public enum MeasurementCurrentTypes { AC, DC };
        private MeasurementCurrentTypes measurementCurrentType;
        public MeasurementCurrentTypes MeasurementCurrentType
        {
            get { return measurementCurrentType; }
            set
            {
                if (value != measurementCurrentType)
                {
                    measurementCurrentType = value;
                    //UpdateMeasurementSettings();
                }
            }
        }
        private double resistanceMeasurementCurrent;
        public double ResistanceMeasurementCurrent
        {
            get { return resistanceMeasurementCurrent; }
            set
            {
                if (value != resistanceMeasurementCurrent)
                {
                    resistanceMeasurementCurrent = value;
                    //UpdateMeasurementSettings();
                }
            }
        }
        private double measurementRange;
        public double MeasurementRange
        {
            get { return measurementRange; }
            set
            {
                if (value != measurementRange)
                {
                    measurementRange = value;
                    //UpdateMeasurementSettings();
                }
            }
        }
        private void UpdateMeasurementSettings()
        {
            switch (measurementFunction)
            {
                case MeasurementFunctions.Voltage:
                    switch (measurementCurrentType)
                    {
                        case MeasurementCurrentTypes.AC:
                            dmm.Measurement.MeasurementFunction = Agilent34410MeasurementFunctionEnum.Agilent34410MeasurementFunctionACVoltage;
                            break;
                        case MeasurementCurrentTypes.DC:
                            dmm.Measurement.MeasurementFunction = Agilent34410MeasurementFunctionEnum.Agilent34410MeasurementFunctionDCVoltage;
                            break;
                    }
                    break;
                case MeasurementFunctions.Current:
                    switch (measurementCurrentType)
                    {
                        case MeasurementCurrentTypes.AC:
                            dmm.Measurement.MeasurementFunction = Agilent34410MeasurementFunctionEnum.Agilent34410MeasurementFunctionACCurrent;
                            break;
                        case MeasurementCurrentTypes.DC:
                            dmm.Measurement.MeasurementFunction = Agilent34410MeasurementFunctionEnum.Agilent34410MeasurementFunctionDCCurrent;
                            break;
                    }
                    break;
                case MeasurementFunctions.Resistance:
                    dmm.Measurement.MeasurementFunction = Agilent34410MeasurementFunctionEnum.Agilent34410MeasurementFunctionResistance;
                    break;
                case MeasurementFunctions.Resistance4Wire:
                    dmm.Measurement.MeasurementFunction = Agilent34410MeasurementFunctionEnum.Agilent34410MeasurementFunctionDCVoltage;
                    break;
                case MeasurementFunctions.Capacitance:
                    dmm.Measurement.MeasurementFunction = Agilent34410MeasurementFunctionEnum.Agilent34410MeasurementFunctionCapacitance;
                    break;
                case MeasurementFunctions.Frequency:
                    dmm.Measurement.MeasurementFunction = Agilent34410MeasurementFunctionEnum.Agilent34410MeasurementFunctionFrequency;
                    break;
                case MeasurementFunctions.Period:
                    dmm.Measurement.MeasurementFunction = Agilent34410MeasurementFunctionEnum.Agilent34410MeasurementFunctionPeriod;
                    break;
                case MeasurementFunctions.Temperature:
                    dmm.Measurement.MeasurementFunction = Agilent34410MeasurementFunctionEnum.Agilent34410MeasurementFunctionTemperature;
                    break;
            };
        }
        public double Measure()
        {
            double value = double.NaN;
            switch (measurementFunction)
            {
                case MeasurementFunctions.Voltage:
                    switch (measurementCurrentType)
                    {
                        case MeasurementCurrentTypes.AC:
                            value = dmm.Voltage.ACVoltage.Measure(measurementRange, Agilent34410ResolutionEnum.Agilent34410ResolutionDefault);
                            break;
                        case MeasurementCurrentTypes.DC:
                            value = dmm.Voltage.DCVoltage.Measure(measurementRange, Agilent34410ResolutionEnum.Agilent34410ResolutionDefault);
                            break;
                    }
                    break;
                case MeasurementFunctions.Current:
                    switch (measurementCurrentType)
                    {
                        case MeasurementCurrentTypes.AC:
                            value = dmm.Current.ACCurrent.Measure(measurementRange, Agilent34410ResolutionEnum.Agilent34410ResolutionDefault);
                            break;
                        case MeasurementCurrentTypes.DC:
                            value = dmm.Current.DCCurrent.Measure(measurementRange, Agilent34410ResolutionEnum.Agilent34410ResolutionDefault);
                            break;
                    }
                    break;
                case MeasurementFunctions.Resistance:
                    value = dmm.Resistance.Measure(measurementRange, Agilent34410ResolutionEnum.Agilent34410ResolutionDefault);
                    break;
                case MeasurementFunctions.Resistance4Wire:
                    value = dmm.Voltage.DCVoltage.Measure(measurementRange, Agilent34410ResolutionEnum.Agilent34410ResolutionDefault);
                    break;
                case MeasurementFunctions.Capacitance:
                    value = dmm.Capacitance.Measure(measurementRange, Agilent34410ResolutionEnum.Agilent34410ResolutionDefault);
                    break;
                case MeasurementFunctions.Frequency:
                    value = dmm.Frequency.Measure(measurementRange, Agilent34410ResolutionEnum.Agilent34410ResolutionDefault);
                    break;
                case MeasurementFunctions.Period:
                    value = dmm.Period.Measure(measurementRange, Agilent34410ResolutionEnum.Agilent34410ResolutionDefault);
                    break;
                case MeasurementFunctions.Temperature:
                    value = dmm.Temperature.RTD.Measure(Agilent34410ResolutionEnum.Agilent34410ResolutionDefault);
                    break;
            };
            return value;
        }
        public bool commentsEnable;
        public delegate void void_void();
        public event void_void RXcomplete;
        public static TestBench testBench;
        public TestBench()
        {
            commentsEnable = false;
            modbus = new Modbus.Modbus();
            modbus.RXcomplete += modbus_RXcomplete;
            modbus.TimeOut += modbus_TimeOut;
            InitDmm();
            testBench = this;
        }
        void modbus_RXcomplete()
        {
            RXcomplete();
        }
        void modbus_TimeOut()
        {
            //GlobalMode = GlobalModes.Idle;
        }
        private void InitDmm()
        {
            dmm = new Agilent34410();
            Ivi.Visa.Interop.ResourceManager rm = new Ivi.Visa.Interop.ResourceManager();
            String[] resources = null;
            try
            {
                resources = rm.FindRsrc("USB?*");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Ошибка подключения к цифровому мультиметру");
            }
            if (resources != null)
            {
                if (resources.Length > 0)
                {
                    dmm.Initialize(resources[0], false, true);
                }
            }
        }
        /// <summary>
        /// Функция полного сброса коммутатора
        /// </summary>
        public void RK104FullReset()
        {
            if (commentsEnable)
                Write("Общий сброс.\n", System.Drawing.Color.Aqua);
            ushort[] registers = { 0xf0 };
            modbus.Function0x10(0, 1, registers);
        }
        /// <summary>
        /// Функция включения реле в коммутаторе РК104
        /// </summary>
        /// <param name="Point">Номер точки (нумерация с 1)</param>
        /// <param name="Section">Атрибут шин битовый (A-1, B-2, C-4, D-8)</param>
        public void RK104SetPoint(byte Point, byte Buses)
        {
            if (commentsEnable)
                Write("Замыкание контактов точки " + Point + " на шинах " + Buses + ".\n", System.Drawing.Color.Aqua);
            ushort[] registers = { 1, Point, Buses };
            modbus.Function0x10(0, 3, registers);
        }
        /// <summary>
        /// Функция выключения реле в коммутаторе РК104
        /// </summary>
        /// <param name="Point">Номер точки (нумерация с 1)</param>
        /// <param name="Section">Атрибут шин битовый (A-1, B-2, C-4, D-8)</param>
        public void RK104ResetPoint(byte Point, byte Buses)
        {
            if (commentsEnable)
                Write("Размыкание контактов точки " + Point + " на шинах " + Buses + ".\n", System.Drawing.Color.Aqua);
            ushort[] Regs = { 4, Point, Buses };
            modbus.Function0x10(0, 3, Regs);
        }
        /// <summary>
        /// Функция загрузки реле в коммутаторе РК104
        /// </summary>
        /// <param name="Point">Номер точки (нумерация с 1)</param>
        /// <param name="Section">Атрибут шин битовый (A-1, B-2, C-4, D-8)</param>
        public void RK104LoadPoint(byte Point, byte Buses)
        {
            if (commentsEnable)
                Write("Замыкание контактов точки " + Point + " на шинах " + Buses + " и размыкание остальных контактов точки.\n", System.Drawing.Color.Aqua);
            ushort[] Regs = { 8, Point, Buses };
            modbus.Function0x10(0, 3, Regs);
        }

    }
}