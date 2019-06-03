using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using AtmosphericSensors.HardwareIO;

namespace AtmosphericSensors
{
    public class AtmosphericSensorBME280 //: ITemperatureSensor, IBarometricPressureSensor, IHumiditySensor
    {
        private II2cSensor bme280sensor;

        private long[] temperatureCompensation = new long[4];
        private long[] pressureCompensation = new long[10];
        private long[] humitidyCompensation = new long[7];

        private long fineTemperature = 0;

        private DateTimeOffset lastReading = DateTimeOffset.MinValue;
        private const int minTimeBetweenReadings = 500;
        
        public AtmosphericSensorBME280(II2cSensor bme280sensor) //0x77
        {
            this.bme280sensor = bme280sensor;
            ReadCompensation();
        }

        private void ReadCompensation()
        {
            ReadTemperatureCompensation();
            ReadHumidityCompensation();
            ReadPressureCompensation();
        }

        private void ReadTemperatureCompensation()
        {
            temperatureCompensation[1] = bme280sensor.Read16bitRegister(0x88, 0x89);
            temperatureCompensation[2] = bme280sensor.Read16bitRegister(0x8a, 0x8b);
            temperatureCompensation[3] = bme280sensor.Read16bitRegister(0x8C, 0x8D);
        }        

        private void ReadPressureCompensation()
        {
            for (int i = 1; i < 10; i++)
            {
                pressureCompensation[i] = bme280sensor.Read16bitRegister((byte)(0x8E + i - 1), (byte)(0x8F + 0x01 + i - 1));
            }
        }

        private void ReadHumidityCompensation()
        {
            humitidyCompensation[1] = bme280sensor.ReadRegister(0xA1);
            humitidyCompensation[2] = bme280sensor.Read16bitRegister(0xE1, 0xE2);
            humitidyCompensation[3] = bme280sensor.ReadRegister(0xE3);
            humitidyCompensation[4] = (bme280sensor.ReadRegister(0xE4) << 4) + (bme280sensor.ReadRegister(0xE5) & 0x0F);
            humitidyCompensation[5] = (bme280sensor.ReadRegister(0xE6) << 4) + ((bme280sensor.ReadRegister(0xE5) >> 4) & 0x0F);
            humitidyCompensation[6] = bme280sensor.ReadRegister(0xE7);
        }

        private double CalculateTemperatureCompensation(long rawTemperature)
        {
            var step1 = (((rawTemperature >> 3) - (temperatureCompensation[1] << 1)) * temperatureCompensation[2]) >> 11;
            var step2 = ((( ( (rawTemperature >> 4) - temperatureCompensation[1] ) * ( (rawTemperature >> 4) - temperatureCompensation[1]) ) >> 12) * temperatureCompensation[3]) >> 14;
            fineTemperature = step1 + step2;
            return ((fineTemperature * 5 + 128) >> 8) / 100.0;
        }

        private double CalculatePressureCompensation(long rawPressure)
        {
            var step1 = fineTemperature - 128000;
            var step2 = step1 * step1 * pressureCompensation[6];
            var step3 = step2 + (step1 * (pressureCompensation[5] << 17));
            var step4 = step3 + (pressureCompensation[4] << 35);
            var step5 = ((step1 * step1 * pressureCompensation[3]) >> 8) + ((step1 * pressureCompensation[2]) << 12);
            var step6 = ((1 << 47) + step5) * (pressureCompensation[1] >> 33);
            if (step5 == 0)
                return 0;
            var pressure = 1048576 - rawPressure;
            pressure = (((pressure << 31) - step4) * 3125) / step6;
            var step7 = (pressureCompensation[9] * (pressure >> 13) * (pressure >> 13)) >> 25;
            var step8 = (pressureCompensation[8] * pressure) >> 19;
            pressure = ((pressure + step7 + step8) >> 8) + (pressureCompensation[7] << 4);
            return (double)pressure / 256;
        }

        private long ReadRawHumidity()
        {
            return bme280sensor.Read16bitRegister(0xFE, 0xFD);
        }

        private long ReadRawPressure()
        {
            return bme280sensor.Read20bitRegister(0xF8, 0xF7, 0xF9);
        }

        private long ReadRawTemperature()
        {
            return bme280sensor.Read20bitRegister(0xFB, 0xFA, 0xFC);
        }
    }
}
