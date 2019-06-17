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

namespace AtmosphericSensors.BME280
{
    public class BME280 : IAtmosphericSensor
    {
        private readonly II2cSensor bme280sensor;
        private readonly IBME280Compensations compensations;

        private int rawTemperature = 0;
        private int rawHumidity = 0;
        private int rawPressure = 0;
        
        public BME280(II2cSensor bme280sensor, IBME280Compensations compensations) //0x77
        {
            this.bme280sensor = bme280sensor;
            this.compensations = compensations;
            WriteControl();
            WriteHumidityControl();
            TryBurstRead();
            Debug.WriteLine("AthomspericSensorFinishedLoading i2c device with ID: {0}", bme280sensor.ReadRegister((byte)BME280Registers.ChipId));
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

        internal void WriteControl()
        {
            bme280sensor.WriteRegister((byte)BME280Registers.Control, new byte[] { 0x3F });
            System.Threading.Thread.Sleep(5);
        }

        internal void WriteHumidityControl()
        {
            bme280sensor.WriteRegister((byte)BME280Registers.ControlHumid, new byte[] { 0x03 });
            System.Threading.Thread.Sleep(5);
        }

        internal void TryBurstRead()
        {
            try
            {
                BurstReadParameters();
                Debug.WriteLine("Uncompensated readings after burst reading Temperature: {0}; Humidity: {1}; Pressure: {2}", rawTemperature, rawHumidity, rawPressure);
            }
            catch (Exception ex) 
            {
                Debug.WriteLine("Error burst read: {0}", ex.Message);
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
            rawTemperature = bme280sensor.Read20bitRegister((byte)BME280Registers.TemperatureDataMSB);
        }

        internal void ReadRawHumidity()
        {
            byte msb = bme280sensor.ReadRegister((byte)BME280Registers.HumidityDataMSB);
            byte lsb = bme280sensor.ReadRegister((byte)BME280Registers.HumidityDataLSB);
            rawHumidity = (msb << 8) + lsb;
        }

        internal void ReadRawPressure()
        {
            rawPressure = bme280sensor.Read20bitRegister((byte)BME280Registers.PressureDataMSB);
        }

    }
}
