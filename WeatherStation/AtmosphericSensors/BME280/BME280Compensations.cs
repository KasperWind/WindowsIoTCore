using AtmosphericSensors.HardwareIO;
using System;
using System.Text;

namespace AtmosphericSensors.BME280
{
    public class BME280Compensations : IBME280Compensations
    {
        public long[] Temperature { get; private set; } = new long[4];
        public long[] Pressure { get; private set; } = new long[10];
        public long[] Humidity { get; private set; } = new long[7];
        public int FineTemperature { get; private set; } = 0;

        public BME280Compensations(II2cSensor bme280sensor)
        {
            ReadTemperatureCompensation(bme280sensor);
            ReadHumitidyCompensation(bme280sensor);
            ReadPressureCompensation(bme280sensor);
        }

        public void ReadTemperatureCompensation(II2cSensor bme280sensor)
        {
            Temperature[1] = bme280sensor.ReadUInt16Register((byte)BME280Registers.DigT1);
            Temperature[2] = bme280sensor.ReadInt16Register((byte)BME280Registers.DigT2);
            Temperature[3] = bme280sensor.ReadInt16Register((byte)BME280Registers.DigT3);
        }

        public void ReadPressureCompensation(II2cSensor bme280sensor)
        {
            Pressure[1] = bme280sensor.ReadUInt16Register((byte)BME280Registers.DigP1);
            for (int i = 2; i < 10; i++)
            {
                Pressure[i] = bme280sensor.ReadInt16Register((byte)((int)BME280Registers.DigP2 + ((i - 2) * 2)));
            }
        }

        public void ReadHumitidyCompensation(II2cSensor bme280sensor)
        {
            Humidity[1] = bme280sensor.ReadRegister((byte)BME280Registers.DigH1);
            Humidity[2] = (Int16)bme280sensor.ReadUInt16Register((byte)BME280Registers.DigH2);
            Humidity[3] = bme280sensor.ReadRegister((byte)BME280Registers.DigH3);
            Humidity[4] = (Int16)((bme280sensor.ReadRegister((byte)BME280Registers.DigH4) << 4) | (bme280sensor.ReadRegister((byte)BME280Registers.DigH4 + 1) & 0x0F));
            Humidity[5] = (Int16)((bme280sensor.ReadRegister((byte)BME280Registers.DigH5 + 1) << 4) | ((bme280sensor.ReadRegister((byte)BME280Registers.DigH5) >> 4) & 0x0F));
            Humidity[6] = (sbyte)bme280sensor.ReadRegister((byte)BME280Registers.DigH6);
        }

        public double CalculateTemperature(int rawTemperature)
        {
            CalculateFineTemperature(rawTemperature);
            return FineTemperature / 5120.0;
        }

        public void CalculateFineTemperature(int rawTemperature)
        {
            var var1 = ((rawTemperature / 16384.0) - (Temperature[1] / 1024.0)) * Temperature[2];
            var var2 = ((rawTemperature / 131072.0) - (Temperature[1] / 8192.0)) * Temperature[3];
            FineTemperature = (int)(var1 + var2);
        }

        public double CalculatePressure(int rawPressure)
        {
            double var1, var2, pressure;
            var1 = (FineTemperature / 2.0) - 64000.0;
            var2 = var1 * var1 * (Pressure[6] / 32768.0);
            var2 = var2 + var1 * Pressure[5] * 2.0;
            var2 = (var2 / 4.0) + (Pressure[4] * 65536.0);
            var1 = (Pressure[3] * var1 * var1 / 524288.0 + Pressure[2] * var1) / 524288.0;
            var1 = (1.0 + var1 / 32768.0) * Pressure[1];
            if (var1 == 0)
                return 0.0;
            pressure = 1048576.0 - rawPressure;
            pressure = (pressure - (var2 / 4096.0)) * 6250.0 / var1;
            var1 = Pressure[9] * pressure * pressure / 2147483648.0;
            var2 = pressure * Pressure[8] / 32768.0;
            pressure = pressure + (var1 + var2 + Pressure[7]) / 16.0;
            return pressure / 100.0;
        }

        public double CalculateHumidity(int rawHumidity)
        {
            double var;
            var = FineTemperature - 76800.0;
            var = (rawHumidity - (Humidity[4] * 64.0 + Humidity[5] / 16384.0 * var)) *
                (Humidity[2] / 65536.0 * (1.0 + Humidity[6] / 67108864.0 * var * (1.0 + Humidity[3] / 67108864.0 * var)));
            var = var * (1.0 - Humidity[1] * var / 524288.0);
            if (var > 100.0)
                var = 100.0;
            else if (var < 0.0)
                var = 0.0;
            return var;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            FineTemperatureToString(sb);
            TemperatureToString(sb);
            HumidityToString(sb);
            PressureToString(sb);
            return sb.ToString();
        }

        private void PressureToString(StringBuilder sb)
        {
            for (int i = 0; i < Pressure.Length; i++)
            {
                CreateArrayEntry("Pressure", sb, i, Pressure[i]);
            }
        }

        private void HumidityToString(StringBuilder sb)
        {
            for (int i = 0; i < Humidity.Length; i++)
            {
                CreateArrayEntry("Humidity", sb, i, Humidity[i]);
            }
        }

        private void TemperatureToString(StringBuilder sb)
        {
            for (int i = 0; i < Temperature.Length; i++)
            {
                CreateArrayEntry("Temperature", sb, i, Temperature[i]);
            }
        }

        private void FineTemperatureToString(StringBuilder sb)
        {
            sb.Append("{FineTemperature:\"");
            sb.Append(FineTemperature.ToString());
            sb.Append("\";");
        }

        private void CreateArrayEntry(string variable, StringBuilder sb, int iterationValue, long actualValue)
        {
            sb.Append(variable);
            sb.Append("[");
            sb.Append(iterationValue.ToString());
            sb.Append("]:\"");
            sb.Append(actualValue.ToString());
            sb.Append("\";");
        }
    }
}
