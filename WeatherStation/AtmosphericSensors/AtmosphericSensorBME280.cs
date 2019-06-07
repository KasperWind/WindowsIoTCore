using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using AtmosphericSensors.HardwareIO;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AtmosphericSensors.tests")]
namespace AtmosphericSensors
{
    public class AtmosphericSensorBME280 : ITemperatureSensor, IBarometricPressureSensor, IHumiditySensor
    {
        private readonly II2cSensor bme280sensor;
        private readonly AtmosphericSensorBME280Compensations compensations;

        private DateTimeOffset lastReading = DateTimeOffset.MinValue;
        private const int minTimeBetweenReadings = 500;

        private long rawTemperature = 0L;
        private long rawHumidity = 0L;
        private long rawPressure = 0L;
        
        public AtmosphericSensorBME280(II2cSensor bme280sensor) //0x77
        {
            this.bme280sensor = bme280sensor;
            compensations = new AtmosphericSensorBME280Compensations(bme280sensor);
        }

        public double GetTemperature()
        {
            throw new NotImplementedException();
        }

        public double GetBarometricPressure()
        {
            throw new NotImplementedException();
        }

        public double GetHumidity()
        {
            throw new NotImplementedException();
        }

        internal void TryBurstRead()
        {
            try
            {
                BurstReadParameters();
                lastReading = DateTimeOffset.Now;
            }
            catch (Exception)
            {

            }
        }

        internal void BurstReadParameters()
        {
            ReadRawTemperature();
            ReadRawHumidity();
            ReadRawPressure();
        }

        internal void ReadRawTemperature()
        {
            rawTemperature = bme280sensor.Read20bitRegister(0xFB, 0xFA, 0xFC);
        }

        internal void ReadRawHumidity()
        {
            rawHumidity = bme280sensor.Read16bitRegister(0xFE, 0xFD);
        }

        internal void ReadRawPressure()
        {
            rawPressure = bme280sensor.Read20bitRegister(0xF8, 0xF7, 0xF9);
        }

    }
}
