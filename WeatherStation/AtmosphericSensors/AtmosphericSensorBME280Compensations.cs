using AtmosphericSensors.HardwareIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtmosphericSensors
{
    public class AtmosphericSensorBME280Compensations
    {

        public long[] Temperature { get; private set; } = new long[4];
        public long[] Pressure { get; private set; } = new long[10];
        public long[] Humitidy { get; private set; } = new long[7];
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
            Humitidy[1] = bme280sensor.ReadRegister(0xA1);
            Humitidy[2] = bme280sensor.Read16bitRegister(0xE1, 0xE2);
            Humitidy[3] = bme280sensor.ReadRegister(0xE3);
            Humitidy[4] = (bme280sensor.ReadRegister(0xE4) << 4) + (bme280sensor.ReadRegister(0xE5) & 0x0F);
            Humitidy[5] = (bme280sensor.ReadRegister(0xE6) << 4) + ((bme280sensor.ReadRegister(0xE5) >> 4) & 0x0F);
            Humitidy[6] = bme280sensor.ReadRegister(0xE7);
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
            var step1 = FineTemperature - 128000;
            var step2 = step1 * step1 * Pressure[6];
            var step3 = step2 + (step1 * (Pressure[5] << 17));
            var step4 = step3 + (Pressure[4] << 35);
            var step5 = ((step1 * step1 * Pressure[3]) >> 8) + ((step1 * Pressure[2]) << 12);
            var step6 = ((1 << 47) + step5) * (Pressure[1] >> 33);
            if (step5 == 0)
                return 0;
            var pressure = 1048576 - rawPressure;
            pressure = (((pressure << 31) - step4) * 3125) / step6;
            var step7 = (Pressure[9] * (pressure >> 13) * (pressure >> 13)) >> 25;
            var step8 = (Pressure[8] * pressure) >> 19;
            pressure = ((pressure + step7 + step8) >> 8) + (Pressure[7] << 4);
            return (double)pressure / 256;
        }
    }
}
