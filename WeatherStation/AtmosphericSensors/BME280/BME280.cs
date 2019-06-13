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
    public class BME280 : ITemperatureSensor, IBarometricPressureSensor, IHumiditySensor
    {
        private readonly II2cSensor bme280sensor;
        private readonly BME280Compensations compensations;

        private DateTimeOffset lastReading = DateTimeOffset.MinValue;
        private const int minTimeBetweenReadings = 500;

        private long rawTemperature = 0L;
        private long rawHumidity = 0L;
        private long rawPressure = 0L;
        
        public BME280(II2cSensor bme280sensor) //0x77
        {
            this.bme280sensor = bme280sensor;
            compensations = new BME280Compensations(bme280sensor);
            writeControl();
            writeHumidityControl();
            TryBurstRead();
            Debug.WriteLine("AthomspericSensorFinishedLoading: {0}", bme280sensor.ReadRegister((byte)BME280Registers.ChipId));
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

        internal void writeControl()
        {
            bme280sensor.WriteRegister((byte)BME280Registers.Control, new byte[] { 0x3F });
            System.Threading.Thread.Sleep(20);
        }

        internal void writeHumidityControl()
        {
            bme280sensor.WriteRegister((byte)BME280Registers.ControlHumid, new byte[] { 0x03 });
            System.Threading.Thread.Sleep(20);
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
            rawTemperature = bme280sensor.Read20bitRegister((byte)BME280Registers.TemperatureDataMSB);
            Debug.WriteLine("Raw Temperature: {0}", rawTemperature);
        }

        internal void ReadRawHumidity()
        {
            rawHumidity = bme280sensor.ReadInt16Register((byte)BME280Registers.HumidityDataMSB);
            Debug.WriteLine("Raw Humidity: {0}", rawHumidity);
        }

        internal void ReadRawPressure()
        {
            rawPressure = bme280sensor.Read20bitRegister((byte)BME280Registers.PressureDataMSB);
            Debug.WriteLine("Raw Pressure: {0}", rawPressure);
        }

    }
}
