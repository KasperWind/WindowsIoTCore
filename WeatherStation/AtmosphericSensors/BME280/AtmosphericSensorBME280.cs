using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using AtmosphericSensors.HardwareIO;
using System.Runtime.CompilerServices;
using System.Diagnostics;

[assembly: InternalsVisibleTo("AtmosphericSensors.tests")]
namespace AtmosphericSensors.BME280
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
            TryBurstRead();
            Debug.WriteLine("AthomspericSensorFinishedLoading:");
            Debug.Write("\t");
            Debug.WriteLine(compensations.ToString());
        }

        public double GetTemperature()
        {
            TryBurstRead();
            return compensations.CalculateTemperature(rawTemperature);
        }

        public double GetBarometricPressure()
        {
            TryBurstRead();
            return compensations.CalculatePressure(rawPressure);
        }

        public double GetHumidity()
        {
            TryBurstRead();
            return compensations.CalculateHumidity(rawHumidity);
        }

        internal void TryBurstRead()
        {
            try
            {
                BurstReadParameters();
                lastReading = DateTimeOffset.Now;
            }
            catch (Exception ex) 
            {
                Debug.WriteLine("Error burst read: {0}", ex.Message);
                lastReading = DateTimeOffset.MinValue;
            }
        }

        internal void BurstReadParameters()
        {
            ReadRawTemperature();
            ReadRawHumidity();
            ReadRawPressure();
            compensations.CalculateTemperature(rawTemperature);
        }

        internal void ReadRawTemperature()
        {
            rawTemperature = bme280sensor.Read20bitRegister(0xFA, 0xFB, 0xFC);
            Debug.WriteLine("Raw Temperature: {0}", rawTemperature);
        }

        internal void ReadRawHumidity()
        {
            rawHumidity = bme280sensor.Read16bitRegister(0xFE, 0xFD);
            Debug.WriteLine("Raw Humidity: {0}", rawHumidity);
        }

        internal void ReadRawPressure()
        {
            rawPressure = bme280sensor.Read20bitRegister(0xF7, 0xF8, 0xF9);
            Debug.WriteLine("Raw Pressure: {0}", rawPressure);
        }

    }
}
