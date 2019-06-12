using AtmosphericSensors.HardwareIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtmosphericSensors.BME280
{
    public class AtmosphericSensorBME280Compensations
    {

        public long[] Temperature { get; private set; } = new long[4];
        public long[] Pressure { get; private set; } = new long[10];
        public long[] Humidity { get; private set; } = new long[7];
        public long FineTemperature { get; private set; } = 0;

        public AtmosphericSensorBME280Compensations(II2cSensor bme280sensor)
        {
            ReadTemperatureCompensation(bme280sensor);
            ReadHumitidyCompensation(bme280sensor);
            ReadPressureCompensation(bme280sensor);
        }

        public void ReadTemperatureCompensation(II2cSensor bme280sensor)
        {
            Temperature[1] = bme280sensor.Read16bitRegister(0x88, 0x89);
            Temperature[2] = bme280sensor.Read16bitRegister(0x8a, 0x8b);
            Temperature[3] = bme280sensor.Read16bitRegister(0x8C, 0x8D);
        }

        public void ReadPressureCompensation(II2cSensor bme280sensor)
        {
            for (int i = 1; i < 10; i++)
            {
                Pressure[i] = bme280sensor.Read16bitRegister((byte)(0x8E + i - 1), (byte)(0x8F + 0x01 + i - 1));
            }
        }

        public void ReadHumitidyCompensation(II2cSensor bme280sensor)
        {
            Humidity[1] = bme280sensor.ReadRegister(0xA1);
            Humidity[2] = bme280sensor.Read16bitRegister(0xE1, 0xE2);
            Humidity[3] = bme280sensor.ReadRegister(0xE3);
            Humidity[4] = (bme280sensor.ReadRegister(0xE4) << 4) + (bme280sensor.ReadRegister(0xE5) & 0x0F);
            Humidity[5] = (bme280sensor.ReadRegister(0xE6) << 4) + ((bme280sensor.ReadRegister(0xE5) >> 4) & 0x0F);
            Humidity[6] = bme280sensor.ReadRegister(0xE7);
        }
        
        public double CalculateTemperature(long rawTemperature)
        {
            var step1 = (((rawTemperature >> 3) - (Temperature[1] << 1)) * Temperature[2]) >> 11;
            var step2 = (((((rawTemperature >> 4) - Temperature[1]) * ((rawTemperature >> 4) - Temperature[1])) >> 12) * Temperature[3]) >> 14;
            FineTemperature = step1 + step2;
            return ((FineTemperature * 5 + 128) >> 8) / 100.0;
        }
        public double CalculatePressure(long rawPressure)
        {
            long step1 = FineTemperature - 128000;
            long step2 = step1 * step1 * Pressure[6];
            long step3 = step2 + (step1 * (Pressure[5] << 17));
            long step4 = step3 + (Pressure[4] << 35);
            long step5 = ((step1 * step1 * Pressure[3]) >> 8) + ((step1 * Pressure[2]) << 12);
            long step6 = (((1L << 47) + step5) * Pressure[1]) >> 33;
            if (step6 == 0)
                return 0;
            long pressure = 1048576L - rawPressure;
            pressure = (((pressure << 31) - step4) * 3125L) / step6;
            long step7 = (Pressure[9] * (pressure >> 13) * (pressure >> 13)) >> 25;
            long step8 = (Pressure[8] * pressure) >> 19;
            pressure = ((pressure + step7 + step8) >> 8) + (Pressure[7] << 4);
            return (pressure / 256.0);
        }

        public double CalculateHumidity(long rawHumidityInput)
        {
            long step1 = FineTemperature - 76800;
            long step2 = (((rawHumidityInput << 14) - ((Humidity[4]) << 20) - (Humidity[5] * step1) + (16384L)) >> 15) * ((((((((step1 * (Humidity[6])) >> 10) * (((step1 * (Humidity[3])) >> 11) + (32768L))) >> 10) + (2097152L)) * Humidity[2]) + 8192L) >> 14);
            long step3 = step2 - (((((step2 >> 15) * (step2 >> 15)) >> 7) * (Humidity[1])) >> 4);
            long step4 = step3 < 0 ? 0 : step3;
            long step5 = step4 > 419430400 ? 419430400 : step4;
            return (step5 >> 12) / 1024.0;
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
                CreateArrayEntry("Temperature",sb, i, Temperature[i]);
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
